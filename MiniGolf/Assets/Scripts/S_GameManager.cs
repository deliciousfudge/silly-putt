using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_GameManager : MonoBehaviour
{
    [Header("Start Round UI")]
    public GameObject m_PanelStarting;
    public Button m_ButtonStartingStartLevel;
    public Button m_ButtonStartingQuitToMenu;
    public Text m_TextStartingHole;
    public Text m_TextStartingPar;

    [Header("End Round UI")]
    public GameObject m_PanelEnding;
    public Button m_ButtonEndingLeaderboard;
    public Button m_ButtonEndingNextLevel;
    public Button m_ButtonEndingReplayLevel;
    public Button m_ButtonEndingQuitToMenu;
    public Text m_TextEndingShotStatus;
    public Text m_TextEndingShotCount;

    [Header("In-Game UI")]
    public GameObject m_PanelInGame;
    public Text m_TextInGameShotsTaken;
    public Text m_TextInGamePar;
    public Image m_ImageInGamePowerButtonBorder;
    public Button m_ButtonInGamePower;
    public Canvas m_CanvasInGameProjectedPath;
    public Button m_ButtonInGameRestart;
    public Button m_ButtonInGameEndGame;
    private Image m_ImageInGamePowerButtonFill;
    private Image m_ImageInGameProjectedDirection;

    [Header("Gameplay Objects")]
    public GameObject m_ObjectBall;
    public GameObject m_ObjectCamera;
    public GameObject[] Bombs;

    [Header("Audio")]
    public AudioSource m_AudioPlayerMusic;
    public AudioClip m_AudioClipMusicInGame;
    public AudioClip m_AudioClipMusicEndGame;
    public AudioSource m_AudioPlayerSFX;
    public AudioClip m_AudioClipSFXButtonForward;
    public AudioClip m_AudioClipSFXButtonBack;
    public AudioClip m_AudioClipSFXHitBall;
    public AudioClip m_AudioClipSFXBallInHole;

    public int[] m_iParScores { get; } = { 3, 5, 9 };
    public float m_fPowerBarFillRate = 0.001f;
    public float m_fMinimumPowerRatio = 0.15f;

    private int m_iCurrentLevelNumber { get; set; } = 0;
    private int m_iRoundCount { get; set; } = 0;
    private bool m_bIsPowerButtonHeld = false;
    private bool m_bIsPowerButtonJustReleased = false;
    private bool m_bIsPowerFillFalling = false;
    private bool m_bHasRoundEnded = false;

    // Script references
    private S_Ball m_ScriptBall;
    private S_GameCamera m_ScriptCamera;


    private void Awake()
    {
        // Make the game manager persistent
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Set references to relevant game object scripts
        m_ScriptBall = m_ObjectBall.GetComponent<S_Ball>();
        m_ScriptCamera = m_ObjectCamera.GetComponent<S_GameCamera>();
        m_ImageInGameProjectedDirection = m_CanvasInGameProjectedPath.GetComponentInChildren<Image>();
        m_ImageInGamePowerButtonFill = m_ButtonInGamePower.GetComponent<Image>();

        m_ImageInGamePowerButtonFill.type = Image.Type.Filled;
        m_ImageInGamePowerButtonFill.fillAmount = 0.0f;

        // Set listeners for UI buttons
        m_ButtonStartingStartLevel.onClick.AddListener(() => { StartRound(); });
        m_ButtonStartingQuitToMenu.onClick.AddListener(() => { QuitToMainMenu(); });

#if UNITY_ANDROID
        m_ButtonEndingLeaderboard.onClick.AddListener(() => { ShowLeaderboard(); });
#endif

        m_ButtonEndingReplayLevel.onClick.AddListener(() => { RestartRound(); });
        m_ButtonEndingQuitToMenu.onClick.AddListener(() => { QuitToMainMenu(); });
        m_ButtonInGameRestart.onClick.AddListener(() => { ProcessPlayerPressingRestartButton(); });
        m_ButtonInGameEndGame.onClick.AddListener(() => { QuitToMainMenuInGame(); });

        m_iCurrentLevelNumber = PlayerPrefs.GetInt("CurrentLevel");

        if (m_iCurrentLevelNumber < m_iParScores.Length)
        {
            m_ButtonEndingNextLevel.onClick.AddListener(() => { GoToLevel(m_iCurrentLevelNumber + 1); });
            print("Set next level to " + m_iCurrentLevelNumber + 1);
        }
        else
        {
            m_ButtonEndingNextLevel.gameObject.SetActive(false);
        }

        SetHoleAndParText();

        m_AudioPlayerMusic.loop = true;
        m_AudioPlayerMusic.clip = m_AudioClipMusicEndGame;
        m_AudioPlayerMusic.Play();

        m_AudioPlayerSFX.loop = false;

        // Display the starting UI
        DisplayStartLevelUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ScriptBall.m_bHasHitBomb)
        {
            RestartRound(false);
            m_ScriptBall.m_bHasHitBomb = false;
        }
            
        // If the ball exists
        if (m_ScriptBall != null)
        {
            // Update the camera's reference to the ball transform
            m_ScriptCamera.SetBallTransform(m_ScriptBall.transform);

            // If the start and end of the round have both been triggered by the ball, but not yet in the game manager
            if (m_ScriptBall.m_bHasRoundStarted && m_ScriptBall.m_bHasRoundEnded && !m_bHasRoundEnded)
            {
                // Proceed with ending the round
                EndRound();
            }
            // If the start of the round has been triggered but not the end (ie The round is in progress)
            else if (m_ScriptBall.m_bHasRoundStarted && !m_ScriptBall.m_bHasRoundEnded)
            {
                // Update the shots taken text
                m_TextInGameShotsTaken.text = "Shots Taken: " + m_ScriptBall.m_iShotCount;

                if (m_ScriptBall.m_bIsBallMoving)
                {
                    m_ScriptCamera.SetBallDirection(m_ObjectBall.transform.forward);

                    // If the velocity of the ball has reduced to a significantly slow amount
                    if (m_ScriptBall.GetBallRigidbody().velocity.sqrMagnitude < 0.7f)
                    {
                        StartCoroutine(EnterShotSelectionDelayed());
                    }
                }
                else
                {
                    if (m_bIsPowerButtonHeld)
                    {
                        // If the power meter becomes full
                        if (m_ImageInGamePowerButtonFill.fillAmount > 0.99f)
                        {
                            // Instruct it to fall back towards empty
                            m_bIsPowerFillFalling = true;
                        }

                        // If the power meter is falling
                        if (m_bIsPowerFillFalling)
                        {
                            print("Power is falling");

                            // If the power meter has gone below the minimum threshold
                            if (m_ImageInGamePowerButtonFill.fillAmount <= m_fMinimumPowerRatio)
                            {
                                // Force the ball to be shot at a minimum amount of power
                                m_bIsPowerButtonJustReleased = true;
                                m_bIsPowerButtonHeld = false;

                            }
                            // If the power meter is still above the minimum threshold
                            else
                            {
                                // Continue to deduct power at the specified rate
                                m_ImageInGamePowerButtonFill.fillAmount -= (m_fPowerBarFillRate * Time.deltaTime);
                            }
                        }
                        // If the power meter is not yet full
                        else
                        {
                            // Continue to add power at the specified rate
                            m_ImageInGamePowerButtonFill.fillAmount += (m_fPowerBarFillRate * Time.deltaTime);
                        }

                    }
                    // If the power button has just been released
                    else if (m_bIsPowerButtonJustReleased)
                    {
                        // Hit the ball at the fill amount ratio of hitting power
                        // We also clamp this value so that the shot is not limp, even at the minimum amount of power
                        float fPowerRatio = Mathf.Clamp(m_ImageInGamePowerButtonFill.fillAmount, 0.27f, 1.0f);
                        print("Power ratio: " + fPowerRatio);
                        m_ScriptBall.PerformShot(m_ScriptCamera.transform.forward, fPowerRatio);
                        m_ScriptCamera.SetIsPlayerSelectingShot(false);
                        m_ScriptBall.SetProjectedDistance(0.0f);

                        // Reset the power meter
                        m_bIsPowerButtonJustReleased = false;
                        m_bIsPowerButtonHeld = false;
                        m_bIsPowerFillFalling = false;

                        // Hide the power meter
                        m_ImageInGamePowerButtonFill.fillAmount = 0.0f;
                        m_ImageInGamePowerButtonBorder.gameObject.SetActive(false);
                    }

                    // Find the forward direction of the camera on the horizontal plane
                    Vector3 CameraForward = m_ScriptCamera.GetCamera().transform.forward;

                    // Lift this forward direction upward so that the arrow will sit above the course
                    CameraForward.y = 1.0f;

                    // Add this forward vector to the ball position to get the new arrow position
                    Vector3 ProjectedPos = m_ScriptBall.transform.position + (CameraForward * 0.5f);

                    // Set the position of the arrow to the position calculated above, and the rotation to match the Y rotation of the camera
                    m_CanvasInGameProjectedPath.transform.position = ProjectedPos;
                    m_ImageInGameProjectedDirection.transform.rotation = Quaternion.Euler(new Vector3(0.0f, m_ScriptCamera.GetCamera().transform.eulerAngles.y, 0.0f));
                }
            }
        }
    }

    // Called at the beginning of each round
    // ie When the player first starts a level or has just restarted the level
    public void StartRound(bool _bShouldResetShotCount = true, bool _bPlayerPressedRestartButton = false)
    {
        // Increment the round count
        m_iRoundCount = PlayerPrefs.GetInt("RoundCount");
        m_iRoundCount++;
        PlayerPrefs.SetInt("RoundCount", m_iRoundCount);

        // Reset the ball state (set back to original position etc)
        m_ScriptBall.ResetState(_bShouldResetShotCount);
        m_ScriptCamera.ResetState();

        // Hide the starting panel
        m_PanelStarting.SetActive(false);

        m_PanelInGame.SetActive(true);
        
        m_ScriptBall.m_bHasRoundStarted = true;
        m_bHasRoundEnded = false;

        m_AudioPlayerMusic.Stop();
        m_AudioPlayerMusic.clip = m_AudioClipMusicInGame;
        m_AudioPlayerMusic.Play();

        if (_bPlayerPressedRestartButton)
        {
            foreach (GameObject Bomb in Bombs)
            {
                Bomb.GetComponent<S_Bomb>().SetHasBeenBlownUp(false);
                Bomb.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject Bomb in Bombs)
            {
                if (Bomb.GetComponent<S_Bomb>().GetHasBeenBlownUp() == false)
                {
                    Bomb.SetActive(true);
                }
            }
        }


        EnterShotSelection();
    }

    // Called at the end of each round
    // ie Every time the player completes the level or restarts the level
    public void EndRound(bool _ShouldShowUI = true, bool _HasFinishedRound = true)
    {
        m_bHasRoundEnded = true;


        // If the user has not purchased the remove ads option
        if (PlayerPrefs.GetString("ShouldGameDisplayAds") != "No")
        {
            //On every alternate round, display an ad to the screen
            if (m_iRoundCount % 2 == 0)
            {
                ShowAd();
            }
        }

#if UNITY_ANDROID
        switch (m_iCurrentLevelNumber)
        {
            case 1:
            {               
                // If the player has not completed the moving on up achievement (For completing level 1)
                if (PlayerPrefs.GetString("Moving On Up") != "Unlocked") 
                {
                    // Store that the player has completed the achievement
                    PlayerPrefs.SetString("Moving On Up", "Unlocked");
                    
                    // Tell Google Play that the player has completed the achievement
                    Social.ReportProgress(SillyPuttConstants.achievement_moving_on_up, 100, (bool _bSuccess) => { });
                }
            }
            break;

            case 2:
            {
                // If the player has not completed the steady as she goes achievement (For completing level 2)
                if (PlayerPrefs.GetString("Steady As She Goes") != "Unlocked") 
                {
                    // Store that the player has completed the achievement
                    PlayerPrefs.SetString("Steady As She Goes", "Unlocked");

                    // Tell Google Play that the player has completed the achievement
                    Social.ReportProgress(SillyPuttConstants.achievement_steady_as_she_goes, 100, (bool _bSuccess) => { });
                }
            }
            break;

            case 3:
            {
                // If the player has not completed the moving on up achievement (For completing level 3)
                if (PlayerPrefs.GetString("The End Of The Road") != "Unlocked") // 0 is true and 1 is false
                {
                    // Store that the player has completed the achievement
                    PlayerPrefs.SetString("The End Of The Road", "Unlocked");

                    // Tell Google Play that the player has completed the achievement
                    Social.ReportProgress(SillyPuttConstants.achievement_the_end_of_the_road, 100, (bool _bSuccess) => { });
                }
            }
            break;

            default:break;
        }
#endif

        if (_HasFinishedRound)
        {
            ProcessPlayerResults();
        }

        m_AudioPlayerMusic.Stop();
        m_AudioPlayerMusic.clip = m_AudioClipMusicEndGame;
        m_AudioPlayerMusic.Play();

        if (_ShouldShowUI)
        {
            DisplayEndLevelUI();
        }
    }

    public void ShowAd()
    {
        // Setting video as the placement ID allows the user to skip the ad after 5 seconds
        const string ksPlacementID = "video";

        // If an advertisement is available
        if (Advertisement.IsReady())
        {
            // Display the ad on the screen and return the view result
            Advertisement.Show(ksPlacementID, new ShowOptions() { resultCallback = AdViewResult });
        }
    }
    public void AdViewResult(ShowResult _Result)
    {
        switch (_Result)
        {
            case ShowResult.Finished:
                {
                    print("Player viewed complete ad");
                }
                break;

            case ShowResult.Skipped:
                {
                    print("Player skipped ad");
                }
                break;

            case ShowResult.Failed:
                {
                    print("Problem showing ad");
                }
                break;
        }
    }

    void DisplayStartLevelUI()
    {
        m_PanelStarting.SetActive(true);

        m_PanelEnding.SetActive(false);

        m_PanelInGame.SetActive(false);
    }

    void DisplayEndLevelUI()
    {
        m_PanelEnding.SetActive(true);

        m_PanelStarting.SetActive(false);

        m_PanelInGame.SetActive(false);
    }

    void QuitToMainMenu()
    {
        PlaySFX(m_AudioClipSFXButtonBack);

        SceneManager.LoadScene("MainMenu");
    }

    void RestartRound(bool _bShouldResetShotCount = true, bool _bPlayerPressedResetButton = false)
    {
        PlaySFX(m_AudioClipSFXButtonForward);

        m_PanelEnding.SetActive(false);

        StartRound(_bShouldResetShotCount, _bPlayerPressedResetButton);
    }

    private void SetHoleAndParText()
    {
        m_TextStartingHole.text = "Hole " + m_iCurrentLevelNumber;

        m_TextStartingPar.text = "Par " + m_iParScores[m_iCurrentLevelNumber - 1];

        m_TextInGamePar.text = "Par: " + m_iParScores[m_iCurrentLevelNumber - 1];
    }

    private void ProcessPlayerResults()
    {
        if (m_ScriptBall.m_iShotCount == 1)
        {
            m_TextEndingShotStatus.text = "Hole-in-one!";
            m_TextEndingShotCount.text = "You finished the hole in 1 shot";

            // If the player has not completed the High Achiever achievement (For scoring a hole in one)
            if (PlayerPrefs.GetString("High Achiever") != "Unlocked")
            {
                // Store that the player has completed the achievement
                PlayerPrefs.SetString("High Achiever", "Unlocked");

                //// Tell Google Play that the player has completed the achievement
                //Social.ReportProgress(SillyPuttConstants.achievement_high_achiever, 100, (bool _bSuccess) => { });
            }
        }
        else
        {
            if (m_iCurrentLevelNumber >= 0)
            {
                int iShotDifference = m_ScriptBall.m_iShotCount - m_iParScores[m_iCurrentLevelNumber - 1];
                switch (iShotDifference)
                {
                    case -3: { m_TextEndingShotStatus.text = "Albatross!"; } break;
                    case -2:
                        {
                            m_TextEndingShotStatus.text = "Eagle!";

                            // If the player has not completed the Flying High achievement (For scoring an eagle)
                            if (PlayerPrefs.GetString("Flying High") != "Unlocked")
                            {
                                // Store that the player has completed the achievement
                                PlayerPrefs.SetString("Flying High", "Unlocked");

#if UNITY_ANDROID
                                // Tell Google Play that the player has completed the achievement
                                Social.ReportProgress(SillyPuttConstants.achievement_flying_high, 100, (bool _bSuccess) => { });
#endif
                            }
                        }
                        break;

                    case -1: { m_TextEndingShotStatus.text = "Birdie!"; } break;
                    case 0: { m_TextEndingShotStatus.text = "Par!"; } break;
                    case 1: { m_TextEndingShotStatus.text = "Bogey"; } break;
                    case 2: { m_TextEndingShotStatus.text = "Double Bogey"; } break;
                    case 3: { m_TextEndingShotStatus.text = "Triple Bogey"; } break;
                    default: { m_TextEndingShotStatus.text = "You need to practice"; } break;
                }

                m_TextEndingShotCount.text = "You finished the hole in " + m_ScriptBall.m_iShotCount + " shots";
            }
        }
#if UNITY_ANDROID
        string sLeaderboardID = "";
        switch(m_iCurrentLevelNumber)
        {
            case 1: { sLeaderboardID = SillyPuttConstants.leaderboard_best_scores_level_1; } break;
            case 2: { sLeaderboardID = SillyPuttConstants.leaderboard_best_scores_level_2; } break;
            case 3: { sLeaderboardID = SillyPuttConstants.leaderboard_best_scores_level_3; } break;
        }

        Social.ReportScore(m_ScriptBall.m_iShotCount, sLeaderboardID, (bool _bSuccess) => { });
#endif
    }

    private IEnumerator EnterShotSelectionDelayed()
    {
        yield return new WaitForSeconds(1.2f);
        EnterShotSelection();
    }

    private void EnterShotSelection()
    {
        // Remove all velocity from the ball
        m_ScriptBall.RemoveVelocity();

        // Tell the camera that the next shot selection has commenced
        m_ScriptCamera.SetIsPlayerSelectingShot(true);

        // Restore the power meter
        m_ImageInGamePowerButtonBorder.gameObject.SetActive(true);

        // Restore the projected path indicator
        m_CanvasInGameProjectedPath.gameObject.SetActive(true);
    }

    public void OnPowerButtonPressed()
    {
        m_bIsPowerButtonHeld = true;
    }

    public void OnPowerButtonReleased()
    {
        m_bIsPowerButtonHeld = false;

        m_bIsPowerButtonJustReleased = true;

        m_CanvasInGameProjectedPath.gameObject.SetActive(false);

        PlaySFX(m_AudioClipSFXHitBall, m_ImageInGamePowerButtonFill.fillAmount);
    }

    public void GoToLevel(int _iLevelNumber)
    {
        string sLevelName = "Level" + _iLevelNumber;

        PlayerPrefs.SetInt("CurrentLevel", _iLevelNumber);

        SceneManager.LoadScene(sLevelName);
    }

    public void PlaySFX(AudioClip _ClipToPlay, float _NewPitch = 1.0f)
    {
        m_AudioPlayerSFX.Stop();
        m_AudioPlayerSFX.pitch = _NewPitch;
        m_AudioPlayerSFX.clip = _ClipToPlay;
        m_AudioPlayerSFX.Play();
    }

#if UNITY_ANDROID
    public void ShowLeaderboard()
    {
        Social.localUser.Authenticate((bool _bSuccess) =>
        {
            if (_bSuccess)
            {
                Debug.Log("You've successfully logged in");
                Social.ShowLeaderboardUI();
            }
            else
            {
                Debug.Log("Login failed for some reason");
            }
        });
    }
#endif

    public void ProcessBallHasEnteredHole()
    {
        PlaySFX(m_AudioClipSFXBallInHole);

        m_ScriptBall.m_bHasBallEnteredHole = false;

        m_ScriptCamera.m_bShouldFollowBall = false;
    }

    public void ProcessPlayerPressingRestartButton()
    {
        EndRound(false, false);

        RestartRound(true, true);
    }

    public void QuitToMainMenuInGame()
    {
        EndRound(false, false);
        QuitToMainMenu();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_GameManager : MonoBehaviour
{
    [Header("Start Round UI")]
    public GameObject StartingPanel;
    public Button StartingStartLevelButton;
    public Button StartingQuitToMenuButton;
    public Text StartingHoleText;
    public Text StartingParText;

    [Header("End Round UI")]
    public GameObject EndingPanel;
    public Button EndingNextLevelButton;
    public Button EndingReplayLevelButton;
    public Button EndingQuitToMenuButton;
    public Text EndingShotStatusText;
    public Text EndingShotCountText;

    [Header("In-Game UI")]
    public GameObject InGamePanel;
    public Text InGameShotsTakenText;
    public Text InGameParText;
    public GameObject InGamePowerSection;
    public Image InGamePowerBarFill;
    public Button InGamePowerButton;
    public Canvas InGameProjectedPathCanvas;
    private Image InGameProjectedPathArrow;

    [Header("Gameplay Objects")]
    public GameObject BallObject;
    public GameObject CameraObject;

    public int[] m_iParScores { get; } = { 3, 5, 7 };
    public float m_fPowerBarFillRate = 0.001f;
    public float m_fMinimumPowerRatio = 0.15f;

    private int m_iCurrentLevelNumber { get; set; } = 0;
    private int m_iRoundCount { get; set; } = 0;
    private bool m_bIsPowerButtonHeld = false;
    private bool m_bIsPowerButtonJustReleased = false;
    private bool m_bIsPowerFillFalling = false;

    // Script references
    private S_Ball BallScript;
    private S_GameCamera CameraScript;

    private void Awake()
    {
        // Make the game manager persistent
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Set references to relevant game object scripts
        BallScript = BallObject.GetComponent<S_Ball>();
        CameraScript = CameraObject.GetComponent<S_GameCamera>();
        InGameProjectedPathArrow = InGameProjectedPathCanvas.GetComponentInChildren<Image>();

        InGamePowerBarFill.type = Image.Type.Filled;
        InGamePowerBarFill.fillAmount = 0.0f;

        // Set listeners for UI buttons
        StartingStartLevelButton.onClick.AddListener(() => { StartRound(); });
        StartingQuitToMenuButton.onClick.AddListener(() => { QuitToMainMenu(); });
        EndingReplayLevelButton.onClick.AddListener(() => { RestartRound(); });
        EndingQuitToMenuButton.onClick.AddListener(() => { QuitToMainMenu(); });

        m_iCurrentLevelNumber = PlayerPrefs.GetInt("CurrentLevel");

        if (m_iCurrentLevelNumber < m_iParScores.Length)
        {
            EndingNextLevelButton.onClick.AddListener(() => { GoToLevel(m_iCurrentLevelNumber++); });
            print("Set next level to " + m_iCurrentLevelNumber++);
        }
        else
        {
            EndingNextLevelButton.gameObject.SetActive(false);
        }

        SetHoleAndParText();

        // Display the starting UI
        DisplayStartLevelUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the camera's reference to the ball transform
        CameraScript.SetBallTransform(BallScript.transform);

        // If the start and end of the round have both been triggered
        if (BallScript.m_bHasRoundStarted && BallScript.m_bHasRoundEnded)
        {
            // Proceed with ending the round
            EndRound();
        }
        // If the start of the round has been triggered but not the end (ie The round is in progress)
        else if (BallScript.m_bHasRoundStarted && !BallScript.m_bHasRoundEnded)
        {
            // Update the shots taken text
            InGameShotsTakenText.text = "Shots Taken: " + BallScript.m_iShotCount;

            if (BallScript.m_bIsBallMoving)
            {
                // If the velocity of the ball has reduced to a significantly slow amount
                if (BallScript.GetBallRigidbody().velocity.sqrMagnitude < 0.7f)
                {
                    StartCoroutine(EnterShotSelectionDelayed());
                }
            }
            else
            {
                if (m_bIsPowerButtonHeld)
                {
                    // If the power meter becomes full
                    if (InGamePowerBarFill.fillAmount > 0.99f)
                    {
                        // Instruct it to fall back towards empty
                        m_bIsPowerFillFalling = true;
                    }

                    // If the power meter is falling
                    if (m_bIsPowerFillFalling)
                    {
                        print("Power is falling");

                        // If the power meter has gone below the minimum threshold
                        if (InGamePowerBarFill.fillAmount <= m_fMinimumPowerRatio)
                        {
                            // Force the ball to be shot at a minimum amount of power
                            m_bIsPowerButtonJustReleased = true;
                            m_bIsPowerButtonHeld = false;

                        }
                        // If the power meter is still above the minimum threshold
                        else
                        {
                            // Continue to deduct power at the specified rate
                            InGamePowerBarFill.fillAmount -= (m_fPowerBarFillRate * Time.deltaTime);
                        }
                    }
                    // If the power meter is not yet full
                    else
                    {
                        Debug.Log("Add power");

                        // Continue to add power at the specified rate
                        InGamePowerBarFill.fillAmount += (m_fPowerBarFillRate * Time.deltaTime);
                    }

                    // Update the projected distance that the ball will travel so that this can be fed back to the player
                    //BallScript.SetProjectedDistance(InGamePowerBarFill.fillAmount * 10.0f);
                }
                // If the power button has just been released
                else if (m_bIsPowerButtonJustReleased)
                {
                    // Hit the ball at the fill amount (between 0 and 1) ratio of hitting power
                    BallScript.PerformShot(CameraScript.transform.forward, InGamePowerBarFill.fillAmount);
                    CameraScript.SetIsPlayerSelectingShot(false);
                    BallScript.SetProjectedDistance(0.0f);

                    // Reset the power meter
                    m_bIsPowerButtonJustReleased = false;
                    m_bIsPowerButtonHeld = false;
                    m_bIsPowerFillFalling = false;

                    // Hide the power meter
                    InGamePowerBarFill.fillAmount = 0.0f;
                    InGamePowerSection.SetActive(false);
                }

                Vector3 ProjectedPos = BallScript.transform.position + new Vector3(0.0f, 0.2f, 0.0f);
                InGameProjectedPathCanvas.transform.position = ProjectedPos;
                InGameProjectedPathArrow.transform.rotation = Quaternion.Euler(new Vector3(0.0f, CameraScript.GetCamera().transform.eulerAngles.y, 0.0f));
                InGameProjectedPathArrow.transform.localScale = new Vector3(0.5f, Mathf.Clamp(InGamePowerBarFill.fillAmount * 2.0f, 0.2f, 2.0f), 1.0f);
            }

            
        }
    }

    // Called at the beginning of each round
    // ie When the player first starts a level or has just restarted the level
    public void StartRound()
    {
        // Reset the ball state (set back to original position etc)
        BallScript.ResetState();
        CameraScript.ResetState();

        // Hide the starting panel
        StartingPanel.SetActive(false);
        InGamePanel.SetActive(true);
        
        BallScript.m_bHasRoundStarted = true;

        EnterShotSelection();
    }

    // Called at the end of each round
    // ie Every time the player completes the level or restarts the level
    public void EndRound()
    {
        // If the user has not purchased the remove ads option
        if (PlayerPrefs.GetString("ShouldGameDisplayAds") != "No")
        {
            // On every alternate round, display an ad to the screen
            if (m_iRoundCount % 2 == 0)
            {
                print("Show an ad");
                ShowAd();
            }
        }
        else
        {
            print("Shouldn't show ads");
        }

        // Increment the round count
        m_iRoundCount = PlayerPrefs.GetInt("RoundCount");
        m_iRoundCount++;
        PlayerPrefs.SetInt("RoundCount", m_iRoundCount);

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

        ProcessPlayerResults();

        DisplayEndLevelUI();
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
        switch(_Result)
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
        StartingPanel.SetActive(true);
        EndingPanel.SetActive(false);
        InGamePanel.SetActive(false);
    }

    void DisplayEndLevelUI()
    {
        EndingPanel.SetActive(true);
        StartingPanel.SetActive(false);
        InGamePanel.SetActive(false);
    }

    void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void RestartRound()
    {
        EndingPanel.SetActive(false);
        StartRound();
    }

    private void SetHoleAndParText()
    {
        StartingHoleText.text = "Hole " + m_iCurrentLevelNumber;
        StartingParText.text = "Par " + m_iParScores[m_iCurrentLevelNumber - 1];
        InGameParText.text = "Par: " + m_iParScores[m_iCurrentLevelNumber - 1];
    }

    private void ProcessPlayerResults()
    {
        if (BallScript.m_iShotCount == 1)
        {
            EndingShotStatusText.text = "Hole-in-one!";
            EndingShotCountText.text = "You finished the hole in 1 shot";

            // If the player has not completed the High Achiever achievement (For scoring a hole in one)
            if (PlayerPrefs.GetString("High Achiever") != "Unlocked")
            {
                // Store that the player has completed the achievement
                PlayerPrefs.SetString("High Achiever", "Unlocked");

                // Tell Google Play that the player has completed the achievement
                Social.ReportProgress(SillyPuttConstants.achievement_high_achiever, 100, (bool _bSuccess) => { });
            }
        }
        else
        {
            int iShotDifference = BallScript.m_iShotCount - m_iParScores[m_iCurrentLevelNumber - 1];
            switch (iShotDifference)
            {
                case -3:
                {
                    EndingShotStatusText.text = "Albatross!";
                }
                break;

                case -2:
                {
                    EndingShotStatusText.text = "Eagle!";

                    // If the player has not completed the Flying High achievement (For scoring an eagle)
                    if (PlayerPrefs.GetString("Flying High") != "Unlocked")
                    {
                        // Store that the player has completed the achievement
                        PlayerPrefs.SetString("Flying High", "Unlocked");

                        // Tell Google Play that the player has completed the achievement
                        Social.ReportProgress(SillyPuttConstants.achievement_flying_high, 100, (bool _bSuccess) => { });
                    }
                }
                break;

                case -1:
                {
                    EndingShotStatusText.text = "Birdie!";
                }
                break;

                case 0:
                {
                    EndingShotStatusText.text = "Par!";
                }
                break;

                case 1:
                {
                    EndingShotStatusText.text = "Bogey";
                }
                break;

                case 2:
                {
                    EndingShotStatusText.text = "Double Bogey";
                }
                break;

                case 3:
                {
                    EndingShotStatusText.text = "Triple Bogey";
                }
                break;

                default:
                {
                    EndingShotStatusText.text = "You need to practice";
                }
                break;
            }

            EndingShotCountText.text = "You finished the hole in " + BallScript.m_iShotCount + " shots";
        }


    }

    private IEnumerator EnterShotSelectionDelayed()
    {
        yield return new WaitForSeconds(1.2f);
        EnterShotSelection();
    }

    private void EnterShotSelection()
    {
        // Remove all velocity from the ball
        BallScript.RemoveVelocity();

        // Tell the camera that the next shot selection has commenced
        CameraScript.SetIsPlayerSelectingShot(true);

        // Restore the power meter
        InGamePowerSection.SetActive(true);

        // Restore the projected path indicator
        InGameProjectedPathCanvas.gameObject.SetActive(true);
    }

    public void OnPowerButtonPressed()
    {
        print("Power is pressed");
        m_bIsPowerButtonHeld = true;
    }

    public void OnPowerButtonReleased()
    {
        print("Power is released");
        m_bIsPowerButtonHeld = false;
        m_bIsPowerButtonJustReleased = true;
        InGameProjectedPathCanvas.gameObject.SetActive(false);
    }

    public void GoToLevel(int _iLevelNumber)
    {
        string sLevelName = "Level" + _iLevelNumber;
        PlayerPrefs.SetInt("CurrentLevel", _iLevelNumber);
        SceneManager.LoadScene(sLevelName);
    }
}

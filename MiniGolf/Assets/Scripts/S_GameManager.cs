using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_GameManager : MonoBehaviour
{
    public GameObject StartingPanel;
    public Button StartingStartLevelButton;
    public Button StartingQuitToMenuButton;
    public Text StartingHoleText;
    public Text StartingParText;

    public GameObject EndingPanel;
    public Button EndingReplayLevelButton;
    public Button EndingQuitToMenuButton;
    public Text EndingShotStatusText;
    public Text EndingShotCountText;

    public GameObject InGamePanel;
    public Text InGameShotsTakenText;
    public Text InGameParText;

    public GameObject BallObject;
    public GameObject CameraObject;

    private int m_iCurrentLevelID;
    private int m_iRoundCount;
    private int[] m_iParScores = { 3, 5, 7 };

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

        // Set listeners for UI buttons
        StartingStartLevelButton.onClick.AddListener(() => { StartRound(); });
        StartingQuitToMenuButton.onClick.AddListener(() => { QuitToMainMenu(); });
        EndingReplayLevelButton.onClick.AddListener(() => { RestartRound(); });
        EndingQuitToMenuButton.onClick.AddListener(() => { QuitToMainMenu(); });

        m_iCurrentLevelID = PlayerPrefs.GetInt("CurrentLevel");

        SetHoleAndParText();

        // Display the starting UI
        DisplayStartLevelUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (BallScript.m_bHasRoundStarted && BallScript.m_bHasRoundEnded)
        {
            EndRound();
        }
        else if (BallScript.m_bHasRoundStarted && !BallScript.m_bHasRoundEnded)
        {
            InGameShotsTakenText.text = "Shots Taken: " + BallScript.m_iShotCount;
        }
    }

    // Called at the beginning of each round
    // ie When the player first starts a level or has just restarted the level
    public void StartRound()
    {
        BallScript.m_iShotCount = 0;
        BallScript.m_bHasRoundStarted = false;
        BallScript.m_bHasRoundEnded = false;

        // Increment the round count
        m_iRoundCount = PlayerPrefs.GetInt("RoundCount");
        m_iRoundCount++;
        PlayerPrefs.SetInt("RoundCount", m_iRoundCount);

        // Reset the ball state (set back to original position etc)
        BallScript.ResetState();

        CameraScript.ResetState();

        // Hide the starting panel
        StartingPanel.SetActive(false);
        InGamePanel.SetActive(true);

        BallScript.m_bHasRoundStarted = true;
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

        switch(m_iCurrentLevelID)
        {
            case 1:
            {               
                // If the player has not completed the moving on up achievement (For completing level 1)
                if (PlayerPrefs.GetString("Moving On Up") != "Unlocked") 
                {
                    // Store that the player has completed the achievement
                    PlayerPrefs.SetString("Moving On Up", "Unlocked");
                    
                    // Tell Google Play that the player has completed the achievement
                    Social.ReportProgress(SillyPuttAchievements.achievement_moving_on_up, 100, (bool _bSuccess) => { });
                }
            }
            break;

            case 2:
            {
                // If the player has not completed the steady as she goes achievement (For completing level 2)
                if (PlayerPrefs.GetString("Steady As She Goes") != "Unlocked") // 0 is true and 1 is false
                {
                    // Store that the player has completed the achievement
                    PlayerPrefs.SetString("Steady As She Goes", "Unlocked");

                    // Tell Google Play that the player has completed the achievement
                    Social.ReportProgress(SillyPuttAchievements.achievement_steady_as_she_goes, 100, (bool _bSuccess) => { });
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
                    Social.ReportProgress(SillyPuttAchievements.achievement_the_end_of_the_road, 100, (bool _bSuccess) => { });
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
        StartingHoleText.text = "Hole " + m_iCurrentLevelID;
        StartingParText.text = "Par " + m_iParScores[m_iCurrentLevelID - 1];
        InGameParText.text = "Par: " + m_iParScores[m_iCurrentLevelID - 1];
    }

    private void ProcessPlayerResults()
    {
        if (BallScript.m_iShotCount == 1)
        {
            EndingShotStatusText.text = "Hole-in-one!";
            EndingShotCountText.text = "You finished the hole in 1 shot";
        }
        else
        {
            int iShotDifference = BallScript.m_iShotCount - m_iParScores[m_iCurrentLevelID - 1];
            switch (iShotDifference)
            {
                case -3:
                {

                }
                break;

                case -2:
                {
                    EndingShotStatusText.text = "Albatross!";
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
}

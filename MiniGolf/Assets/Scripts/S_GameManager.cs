﻿using System.Collections;
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

    public GameObject EndingPanel;
    public Button EndingReplayLevelButton;
    public Button EndingQuitToMenuButton;

    public GameObject Ball;
    public GameObject Camera;

    private int m_iRoundCount;

    // Script references
    private S_Ball BallScript;
    private S_GameCamera CameraScript;

    // Advertisements
    private bool m_bShouldGameDisplayAds = true;

    private void Awake()
    {
        // Make the game manager persistent
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Determine if the user has purchased the remove ads option
        if (PlayerPrefs.GetInt("ShouldGameDisplayAds") == 0) // 0 is false and 1 is true
        {
            m_bShouldGameDisplayAds = false;
        }
        else
        {
            m_bShouldGameDisplayAds = true; 
        }

        // Set references to relevant game object scripts
        BallScript = Ball.GetComponent<S_Ball>();
        CameraScript = Camera.GetComponent<S_GameCamera>();

        // Set listeners for UI buttons
        StartingStartLevelButton.onClick.AddListener(() => { StartRound(); });
        StartingQuitToMenuButton.onClick.AddListener(() => { QuitToMainMenu(); });
        EndingReplayLevelButton.onClick.AddListener(() => { RestartRound(); });
        EndingQuitToMenuButton.onClick.AddListener(() => { QuitToMainMenu(); });

        // Display the starting UI
        DisplayStartLevelUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called at the beginning of each round
    // ie When the player first starts a level or has just restarted the level
    public void StartRound()
    {
        print("Start the round");

        // Increment the round count
        m_iRoundCount = PlayerPrefs.GetInt("RoundCount");
        m_iRoundCount++;
        print("New round count is " + m_iRoundCount);
        PlayerPrefs.SetInt("RoundCount", m_iRoundCount);

        // Reset the ball state (set back to original position etc)
        BallScript.ResetState();

        CameraScript.ResetState();

        // Hide the starting panel
        StartingPanel.SetActive(false);

        BallScript.m_bIsRoundInProgress = true;
    }

    // Called at the end of each round
    // ie Every time the player completes the level or restarts the level
    public void EndRound()
    {
        BallScript.m_bIsRoundInProgress = false;

        // If the user has not purchased the remove ads option
        if (m_bShouldGameDisplayAds)
        {
            // On every alternate round, display an ad to the screen
            if (m_iRoundCount % 2 == 0)
            {
                ShowAd();
            }
        }

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
    }

    void DisplayEndLevelUI()
    {
        EndingPanel.SetActive(true);
        StartingPanel.SetActive(false);
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;

using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class S_MainMenu : MonoBehaviour
{
    // Main Menu Objects
    public GameObject MMContainer;
    public Button MMPlayButton;
    public Button MMAchievementsButton;
    public Button MMExitButton;
    public Button MMTwitterButton;

    // Level Select Objects
    public GameObject LSContainer;
    public Button LSLevel1Button;
    public Button LSLevel2Button;
    public Button LSLevel3Button;

    // Achievements Menu Objects
    public GameObject AMContainer;
    public Button AMBackButton;

    // Google Play integration
    bool m_bIsUserAuthenticated = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = true;

        GoToMainMenu();

        // Add listener to main menu buttons
		MMPlayButton.onClick.AddListener(() => {GoToLevelSelectMenu();});
        MMAchievementsButton.onClick.AddListener(() => { OpenAchievements(); });
        MMExitButton.onClick.AddListener(() => {ExitGame();});
        MMTwitterButton.onClick.AddListener(() => { OpenTwitter(); });

        // Add listener to level select buttons
        LSLevel1Button.onClick.AddListener(() => { GoToLevel(1); });
        LSLevel2Button.onClick.AddListener(() => { GoToLevel(2); });
        LSLevel3Button.onClick.AddListener(() => { GoToLevel(3); });
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bIsUserAuthenticated)
        {
            Social.localUser.Authenticate((bool _bSuccess) =>
            {
                if (_bSuccess)
                {
                    Debug.Log("You've successfully logged in");
                    m_bIsUserAuthenticated = true;
                }
                else
                {
                    Debug.Log("Login failed for some reason");
                }
            });
        }
    }
	
	void GoToLevelSelectMenu()
	{
		print("LevelSelect");
        MMContainer.SetActive(false);
        LSContainer.SetActive(true);
        AMContainer.SetActive(false);
    }
	
	void ExitGame()
	{
        print("Bye everybody!");
		Application.Quit();
	}

    void GoToMainMenu()
    {
        MMContainer.SetActive(true);
        LSContainer.SetActive(false);
        AMContainer.SetActive(false);
    }

    void GoToLevel(int _iLevelNumber)
    {
        string sLevelName = "Level" + _iLevelNumber;
        PlayerPrefs.SetInt("CurrentLevel", _iLevelNumber);
        SceneManager.LoadScene(sLevelName);
    }

    public void OpenTwitter()
    {
        string sTwitterAddress = "http://twitter.com/intent/tweet";
        string sMessage = "GET YO PUTT ON WITH SILLY PUTT"; // The tweet content to display
        string sAppStoreDescription = "You can find the game at: "; // The title of the game
        string sAppStoreLink = "https://play.google.com/store/apps/details?id=com.growlgamesstudio.pizZapMani"; // The Play Store link of the game

        Application.OpenURL(sTwitterAddress + "?text=" +  UnityWebRequest.EscapeURL(sMessage + "\n\n" + sAppStoreDescription + sAppStoreLink));
    }

    public void OpenAchievements()
    {
        Social.localUser.Authenticate((bool _bSuccess) => {
            if (_bSuccess)
            {
                Debug.Log("You've successfully logged in");
                Social.ShowAchievementsUI();
            }
            else
            {
                Debug.Log("Login failed for some reason");
            }
        });
    }
}

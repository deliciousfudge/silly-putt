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
    [Header("Main Menu Objects")]
    public GameObject m_MainMenuContainer;
    public Button m_Button_Play;
    public Button m_Button_Achievements;
    public Button m_Button_Exit;
    public Button m_Button_Twitter;
    public Button m_Button_IAP; // In App Purchases

    [Header("Level Select Objects")]
    public GameObject m_LevelSelectContainer;
    public Button m_Button_Level1;
    public Button m_Button_Level2;
    public Button m_Button_Level3;
    public Button m_Button_LevelSelectToMainMenu;

    [Header("IAP Objects")]
    public GameObject m_IAPContainer;
    public Button m_Button_IAPToMainMenu;
    public Button m_Button_PurchaseNoAds;

    // Google Play integration
    bool m_bIsUserAuthenticated = false;

    private S_IAPManager m_IAPManager;

    // Start is called before the first frame update
    void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = true;

        m_IAPManager = m_IAPContainer.GetComponent<S_IAPManager>();

        GoToMainMenu();

        // Add listener to main menu buttons
        m_Button_Play.onClick.AddListener(() => { OpenLevelSelectMenu(); });
        m_Button_Achievements.onClick.AddListener(() => { OpenAchievements(); });
        m_Button_Exit.onClick.AddListener(() => { ExitGame(); });
        m_Button_Twitter.onClick.AddListener(() => { OpenTwitter(); });
        m_Button_IAP.onClick.AddListener(() => { OpenIAPMenu(); });

        // Add listener to level select buttons
        m_Button_Level1.onClick.AddListener(() => { GoToLevel(1); });
        m_Button_Level2.onClick.AddListener(() => { GoToLevel(2); });
        m_Button_Level3.onClick.AddListener(() => { GoToLevel(3); });
        m_Button_LevelSelectToMainMenu.onClick.AddListener(() => { GoToMainMenu(); });

        // Add listener to IAP buttons
        m_Button_IAPToMainMenu.onClick.AddListener(() => { GoToMainMenu(); });
        m_Button_PurchaseNoAds.onClick.AddListener(() => { ProcessPurchaseNoAds(); });
}

    // Update is called once per frame
    void Update()
    {
        if (!m_bIsUserAuthenticated)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("You've successfully logged in");
                    m_bIsUserAuthenticated = true;
                }
                else
                {
                    Debug.Log("Login attempt was unsuccessful");
                }
            });
        }
    }
	
	public void OpenLevelSelectMenu()
	{
		print("LevelSelect");
        m_MainMenuContainer.SetActive(false);
        m_LevelSelectContainer.SetActive(true);
        m_IAPContainer.SetActive(false);
    }
	
	public void ExitGame()
	{
        print("Bye everybody!");
		Application.Quit();
	}

    public void GoToMainMenu()
    {
        m_MainMenuContainer.SetActive(true);
        m_LevelSelectContainer.SetActive(false);
        m_IAPContainer.SetActive(false);
    }

    public void GoToLevel(int _iLevelNumber)
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

    public void OpenIAPMenu()
    {
        m_MainMenuContainer.SetActive(false);
        m_LevelSelectContainer.SetActive(false);
        m_IAPContainer.SetActive(true);
    }

    private void ProcessPurchaseNoAds()
    {
        // Attempt to purchase the consumable which disables ads
        m_IAPManager.BuyProductID("removeads");

        // If the purchase attempt succeeded
        if (PlayerPrefs.GetString("ShouldGameDisplayAds") == "No")
        {
            // Disable the option to purchase the consumable
            m_Button_IAP.gameObject.SetActive(false);
        }
        
    }
}

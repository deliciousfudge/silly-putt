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
    public Button m_ButtonPlay;
    public Button m_ButtonAchievements;
    public Button m_ButtonLeaderboard;
    public Button m_ButtonExit;
    public Button m_ButtonTwitter;
    public Button m_ButtonIAP; // In App Purchases

    [Header("Level Select Objects")]
    public GameObject m_LevelSelectContainer;
    public Button m_ButtonLevel1;
    public Button m_ButtonLevel2;
    public Button m_ButtonLevel3;
    public Button m_ButtonLevelSelectToMainMenu;

    [Header("IAP Objects")]
    public GameObject m_IAPContainer;
    public Button m_ButtonIAPToMainMenu;
    public Button m_ButtonPurchaseRemoveAds;
    public Text m_TextRemoveAdsDescription;

    [Header("Audio")]
    public AudioSource SFXPlayer;
    public AudioClip SFXButtonForward;
    public AudioClip SFXButtonBack;
    public AudioClip SFXPurchaseSuccessful;

    // Google Play integration
    private bool m_bIsUserAuthenticated = false;
    private S_IAPManager m_IAPManager;

    // Start is called before the first frame update
    void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = true;

        m_IAPManager = m_IAPContainer.GetComponent<S_IAPManager>();

        // If the RemoveAds purchase was made in a previous playthrough
        if (PlayerPrefs.GetString("ShouldGameDisplayAds") == "No")
        {
            // Show that the purchase was completed
            m_TextRemoveAdsDescription.text = "Remove Ads: Purchased";

            // Disable the option to purchase the consumable
            m_ButtonPurchaseRemoveAds.interactable = false;
        }
        else
        {
            // Otherwise display that the purchase was not completed
            m_TextRemoveAdsDescription.text = "Remove Ads: Unpurchased";
        }

        GoToMainMenu(false);

        // Add listener to main menu buttons
        m_ButtonPlay.onClick.AddListener(() => { OpenLevelSelectMenu(); });
        m_ButtonAchievements.onClick.AddListener(() => { OpenAchievements(); });
        m_ButtonLeaderboard.onClick.AddListener(() => { OpenLeaderboard(); });
        m_ButtonExit.onClick.AddListener(() => { ExitGame(); });
        m_ButtonTwitter.onClick.AddListener(() => { OpenTwitter(); });
        m_ButtonIAP.onClick.AddListener(() => { OpenIAPMenu(); });

        // Add listener to level select buttons
        m_ButtonLevel1.onClick.AddListener(() => { GoToLevel(1); });
        m_ButtonLevel2.onClick.AddListener(() => { GoToLevel(2); });
        m_ButtonLevel3.onClick.AddListener(() => { GoToLevel(3); });
        m_ButtonLevelSelectToMainMenu.onClick.AddListener(() => { GoToMainMenu(); });

        // Add listener to IAP buttons
        m_ButtonIAPToMainMenu.onClick.AddListener(() => { GoToMainMenu(); });
        m_ButtonPurchaseRemoveAds.onClick.AddListener(() => { ProcessPurchaseRemoveAds(); });
}

    // Update is called once per frame
    void Update()
    {
        if (!m_bIsUserAuthenticated)
        {
            Social.localUser.Authenticate((bool _success) =>
            {
                if (_success)
                {
                    Debug.Log("You've successfully logged in");
                    m_bIsUserAuthenticated = true;

                    m_IAPManager.InitializePurchasing();
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
        PlaySFX(SFXButtonForward);

        m_MainMenuContainer.SetActive(false);
        m_LevelSelectContainer.SetActive(true);
        m_IAPContainer.SetActive(false);
    }
	
	public void ExitGame()
	{
        PlaySFX(SFXButtonForward);

		Application.Quit();
	}

    public void GoToMainMenu(bool _bShouldPlaySound = true)
    {
        if (_bShouldPlaySound)
        {
            PlaySFX(SFXButtonBack);
        }

        m_MainMenuContainer.SetActive(true);
        m_LevelSelectContainer.SetActive(false);
        m_IAPContainer.SetActive(false);
    }

    public void GoToLevel(int _iLevelNumber)
    {
        PlaySFX(SFXButtonForward);

        string sLevelName = "Level" + _iLevelNumber;
        PlayerPrefs.SetInt("CurrentLevel", _iLevelNumber);
        SceneManager.LoadScene(sLevelName);
    }

    public void OpenTwitter()
    {
        PlaySFX(SFXButtonForward);

        string sTwitterAddress = "http://twitter.com/intent/tweet";
        string sMessage = "GET YO PUTT ON WITH SILLY PUTT"; // The tweet content to display
        string sAppStoreDescription = "You can find the game at: "; // The title of the game
        string sAppStoreLink = "https://play.google.com/store/apps/details?id=com.JokersWild.SillyPutt"; // The Play Store link of the game

        Application.OpenURL(sTwitterAddress + "?text=" +  UnityWebRequest.EscapeURL(sMessage + "\n\n" + sAppStoreDescription + sAppStoreLink));
    }

    public void OpenAchievements()
    {
        Social.localUser.Authenticate((bool _bSuccess) => {
            if (_bSuccess)
            {
                Debug.Log("You've successfully logged in");
                Social.ShowAchievementsUI();

                PlaySFX(SFXButtonForward);
            }
            else
            {
                Debug.Log("Login failed for some reason");
            }
        });
    }

    public void OpenLeaderboard()
    {
        Social.localUser.Authenticate((bool _bSuccess) =>
        {
            if (_bSuccess)
            {
                Debug.Log("You've successfully logged in");
                Social.ShowLeaderboardUI();

                PlaySFX(SFXButtonForward);
            }
            else
            {
                Debug.Log("Login failed for some reason");
            }
        });
    }
    public void OpenIAPMenu()
    {
        PlaySFX(SFXButtonForward);

        m_MainMenuContainer.SetActive(false);
        m_LevelSelectContainer.SetActive(false);
        m_IAPContainer.SetActive(true);
    }

    private void ProcessPurchaseRemoveAds()
    {
        // If the RemoveAds purchase has not already been made
        if (PlayerPrefs.GetString("ShouldGameDisplayAds") != "No")
        {
            m_IAPManager.BuyProductID("removeads");

            if (PlayerPrefs.GetString("ShouldGameDisplayAds") == "No")
            {
                ProcessRemoveAdsPurchased();
            }
        }
    }

    public void ProcessRemoveAdsPurchased()
    {
        PlaySFX(SFXPurchaseSuccessful);

        // Disable the option to purchase the consumable
        m_ButtonPurchaseRemoveAds.interactable = false;

        // Provide feedback to the user that the purchase was successful
        m_TextRemoveAdsDescription.text = "Remove Ads: Purchased";
    }

    public void PlaySFX(AudioClip _ClipToPlay)
    {
        SFXPlayer.Stop();
        SFXPlayer.clip = _ClipToPlay;
        SFXPlayer.Play();
    }
}
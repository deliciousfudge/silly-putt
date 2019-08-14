using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class S_MainMenu : MonoBehaviour
{
    // Main Menu Objects
    public GameObject MainMenu;
    public Button PlayButton;
    public Button AchievementsButton;
    public Button ExitButton;
    public Button TwitterButton;

    // Level Select Objects
    public GameObject LevelSelectMenu;
    public Button Level1Button;

    // Achievements Menu Objects
    public GameObject AchievementsMenu;

    // Start is called before the first frame update
    void Start()
    {
        GoToMainMenu();

        // Add listener to main menu buttons
		PlayButton.onClick.AddListener(() => {GoToLevelSelectMenu();});
        AchievementsButton.onClick.AddListener(() => {GoToAchievementsMenu();});
        ExitButton.onClick.AddListener(() => {ExitGame();});
        TwitterButton.onClick.AddListener(() => { OpenTwitter(); });

        // Add listener to level select buttons
        Level1Button.onClick.AddListener(() => { GoToLevel(1); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void GoToLevelSelectMenu()
	{
		print("LevelSelect");
        MainMenu.SetActive(false);
        LevelSelectMenu.SetActive(true);
        AchievementsMenu.SetActive(false);
    }
	
	void GoToAchievementsMenu()
	{
		print("Achievements");
        MainMenu.SetActive(false);
        LevelSelectMenu.SetActive(false);
        AchievementsMenu.SetActive(true);
    }
	
	void ExitGame()
	{
        print("Bye everybody!");
		Application.Quit();
	}

    void GoToMainMenu()
    {
        MainMenu.SetActive(true);
        LevelSelectMenu.SetActive(false);
        AchievementsMenu.SetActive(false);
    }

    void GoToLevel(int _LevelNumber)
    {
        string sLevelName = "Level" + _LevelNumber;
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
}

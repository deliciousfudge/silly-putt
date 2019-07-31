using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_MainMenu : MonoBehaviour
{
    public GameObject MainMenu;
    public Button PlayButton;
    public Button AchievementsButton;
    public Button ExitButton;

    public GameObject LevelSelectMenu;
    public Button Level1Button;

    public GameObject AchievementsMenu;

    // Start is called before the first frame update
    void Start()
    {
        GoToMainMenu();

		PlayButton.onClick.AddListener(() => {GoToLevelSelectMenu();});
        AchievementsButton.onClick.AddListener(() => {GoToAchievementsMenu();});
        ExitButton.onClick.AddListener(() => {ExitGame();});

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
}

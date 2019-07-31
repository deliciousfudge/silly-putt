using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_MainMenu : MonoBehaviour
{
    public Button PlayButton;
    public Button AchievementsButton;
    public Button ExitButton;

    // Start is called before the first frame update
    void Start()
    {
		PlayButton.onClick.AddListener(() => {GoToLevelSelectMenu();});
        AchievementsButton.onClick.AddListener(() => {GoToAchievementsMenu();});
        ExitButton.onClick.AddListener(() => {ExitGame();});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void GoToLevelSelectMenu()
	{
		print("LevelSelect");
	}
	
	void GoToAchievementsMenu()
	{
		print("Achievements");
	}
	
	void ExitGame()
	{
        print("Bye everybody!");
		Application.Quit();
	}
}

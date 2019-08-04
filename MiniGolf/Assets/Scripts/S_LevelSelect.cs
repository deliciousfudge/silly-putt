using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_LevelSelect : MonoBehaviour
{
    public Button Level1Button;

    // Start is called before the first frame update
    void Start()
    {
        Level1Button.onClick.AddListener(() => { GoToLevel(1); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoToLevel(int _LevelNumber)
    {
        string sLevelName = "Level" + _LevelNumber;
        SceneManager.LoadScene(sLevelName);
    }
}

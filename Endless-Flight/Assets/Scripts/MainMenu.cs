using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    /// <summary>
    /// 
    /// </summary>
    void Start () {
		
	}

    /// <summary>
    /// Called when start game button is pressed and loads maingame scene
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("Scenes/MainGame");
    }

    /// <summary>
    /// Called when quit game button is pressed and quits application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    public GameObject pauseMenu;
    public GameObject gameOver;
    public Text scoreText;
    public Text mainHUDscoreText;
    public Slider fuelBar;

    private void OnEnable()
    {
        GameStateManager.loadGameStartComponents += EnableHUDRender;
        GameStateManager.gameOver += GameOverScreen;
    }

    private void OnDisable()
    {
        GameStateManager.loadGameStartComponents -= EnableHUDRender;
        GameStateManager.gameOver -= GameOverScreen;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Called on pause button press on HUD, calls the pauseGame method in the GameStateManager
    /// </summary>
    public void PauseGame()
    {
        GameStateManager.current.PauseGame();
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        GameStateManager.current.ResumeGame();
        pauseMenu.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    /// <summary>
    /// Enables HUD rendering
    /// </summary>
    private void EnableHUDRender()
    {
        GetComponent<Canvas>().enabled = true;
    }

    /// <summary>
    /// Disables HUD Rendering
    /// </summary>
    private void DisableHUDRender()
    {
        GetComponent<Canvas>().enabled = false;
    }

    public void GameOverScreen()
    {

        gameOver.SetActive(true);
        mainHUDscoreText.enabled = false;
        fuelBar.gameObject.SetActive(false);
        scoreText.text = "Your Score : " + GameObject.FindGameObjectWithTag("player").GetComponent<PlayerStats>().GetScore();

    }


}

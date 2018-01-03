using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour {

    public GameObject pauseMenu;

    private void OnEnable()
    {
        GameStateManager.loadGameStartComponents += EnableHUDRender;
    }

    private void OnDisable()
    {
        GameStateManager.loadGameStartComponents -= EnableHUDRender;

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
}

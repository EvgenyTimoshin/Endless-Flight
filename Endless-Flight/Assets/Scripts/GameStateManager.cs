using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    public static GameStateManager current;

    /// <summary>
    /// Event used to call methods on game start
    /// </summary>
    public delegate void GameStart();
    public static event GameStart startGame;

    /// <summary>
    /// Event used to pause the game
    /// </summary>
    public delegate void GamePause();
    public static event GamePause pauseGame;

    /// <summary>
    /// Event used to pause the game
    /// </summary>
    public delegate void GameResume();
    public static event GameResume resumeGame;

    public delegate void GameEnded();
    public static event ComponentsLoad gameOver;

    /// <summary>
    /// Event used to load all components neccessary when game starts
    /// </summary>
    public delegate void ComponentsLoad();
    public static event ComponentsLoad loadGameStartComponents;

    void Awake()
    {
        current = this;
    }

	// Use this for initialization
	void Start ()
	{
	    
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Called when the game is started (From Main Menu UI StartGame button)
    /// </summary>
    public void StartGame()
    {
        if (startGame != null)
        {
            startGame();
        }
    }

    /// <summary>
    /// Called when the game is paused (from HUD pause button)
    /// </summary>
    public void PauseGame()
    {
        if (pauseGame != null)
        {
            pauseGame();
        }
    }

    /// <summary>
    /// Called when the game is resumed (from HUD-pause menu resume button)
    /// </summary>
    public void ResumeGame()
    {
        if(resumeGame != null)
        {
            resumeGame();
        }
    }

    public void GameOver()
    {
        if (gameOver != null)
        {
            gameOver();
            pauseGame();
        }
    }

    /// <summary>
    /// Loads enabled relevant components and calls event across classes to load components required for game start
    /// </summary>
    public void LoadAllStartGameComponents()
    {
        if (loadGameStartComponents != null)
        {
            loadGameStartComponents();
        }
    }
}

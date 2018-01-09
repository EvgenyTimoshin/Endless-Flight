using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour {

    public static PickUpSpawner current;

    private void Awake()
    {
        current = this;
    }

    private void OnEnable()
    {
        GameStateManager.loadGameStartComponents += EnablePickUpSpawning;
        GameStateManager.pauseGame += DisablePickUpSpawning;
        GameStateManager.resumeGame += EnablePickUpSpawning;
    }

    private void OnDisable()
    {
        GameStateManager.loadGameStartComponents -= EnablePickUpSpawning;
        GameStateManager.pauseGame -= DisablePickUpSpawning;
        GameStateManager.resumeGame -= EnablePickUpSpawning;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Enables all pick ups to start spawning
    /// </summary>
    private void EnablePickUpSpawning()
    {
        InvokeRepeating("SpawnSpeedBoost", 25f, 20f);
        InvokeRepeating("SpawnFuel", 20f, 30f);
        InvokeRepeating("SpawnScoreBooster", 10f, 30f);
    }

    /// <summary>
    /// Disables all pick ups from spawning
    /// </summary>
    private void DisablePickUpSpawning()
    {
        CancelInvoke();
    }
}

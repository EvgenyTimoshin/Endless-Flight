using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ScenerySpawner : MonoBehaviour {

    public static ScenerySpawner current;
    public GameObject player;
    private Vector3 playerPosition;

    private void Awake()
    {
        current = this;
    }

    private void OnEnable()
    {
        IslandScript.spawnScenery += SpawnNextSceneryBlock;    
    }

    private void OnDisable()
    {
        IslandScript.spawnScenery -= SpawnNextSceneryBlock;
    }

    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        playerPosition = player.transform.position;
	}

    /// <summary>
    /// Calls methods to spawn next sceneryblock
    /// </summary>
    private void SpawnNextSceneryBlock()
    {
        SpawnNextIsland();
    }

    /// <summary>
    /// Spawns next island
    /// </summary>
    private void SpawnNextIsland()
    {
        Random rnd = new Random();
        int choice = rnd.Next(0, 5);
        string islandChoice = "Island2";

        if (choice == 0)
        {
            islandChoice = "Island1";
        }

        if (choice == 1)
        {
            islandChoice = "Island2";
        }
        if (choice == 2)
        {
            islandChoice = "Island3";
        }
        if (choice == 3)
        {
            islandChoice = "Island4";
        }
        if (choice == 4)
        {
            //islandChoice = "Island5";
        }


        GameObject island = GameObjectPool.current.GetPooledIsland(islandChoice + "(Clone)");
        island.transform.position = new Vector3(0, 0, playerPosition.z + 4000);
        island.SetActive(true);
    }

    private void spawnNextBackgroundScenery()
    {

    }
}

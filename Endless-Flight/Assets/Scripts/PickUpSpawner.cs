using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PickUpSpawner : MonoBehaviour
{

    public GameObject player;
    private static PickUpSpawner current;
    private bool allowPickUpSpawn = false;
    private Random rnd = new Random();
    private Vector3 spawnPos;
    private WaitForSeconds spawnInterval = new WaitForSeconds(3f);

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
        StartCoroutine(SpawnObjects());
    }

    /// <summary>
    /// Disables all pick ups from spawning
    /// </summary>
    private void DisablePickUpSpawning()
    {
        StopAllCoroutines();
    }


    /// <summary>
    /// Runs and decides which objects to spawn in
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnObjects()
    {
        while (allowPickUpSpawn)
        {
            spawnPos = new Vector3(rnd.Next(130,250),rnd.Next(50,150) , player.transform.position.z + 400f);

            int typeOfObjectSpawn = rnd.Next(0, 6);

            if (typeOfObjectSpawn == 0 || typeOfObjectSpawn == 1 || typeOfObjectSpawn == 2)
            {
                SpawnScoreBoosters(spawnPos);
            }

            if (typeOfObjectSpawn == 3 || typeOfObjectSpawn == 4)
            {
                SpawnScoreMultiplier(spawnPos);
            }

            if (typeOfObjectSpawn == 5)
            {
                SpawnSpeedBoosters(spawnPos);
            }
            
            yield return spawnInterval;
        }
    }

    public void SpawnSpeedBoosters(Vector3 spawnPos)
    {
        
            GameObject speedBooster = GameObjectPool.current.GetPooledPickUp("SpeedBooster");
            speedBooster.SetActive(true);

            speedBooster.transform.position = spawnPos;
    }

    public void SpawnScoreBoosters(Vector3 spawnPos)
    {
       
            GameObject scoreBooster = GameObjectPool.current.GetPooledPickUp("ScoreBooster");
            scoreBooster.SetActive(true);

            scoreBooster.transform.position = spawnPos;

    }


    public void SpawnScoreMultiplier(Vector3 spawnPos)
    { 
        GameObject scoreMultiplier = GameObjectPool.current.GetPooledPickUp("BlueScoreMultiplier");
        scoreMultiplier.SetActive(true);

        scoreMultiplier.transform.position = spawnPos;
    }
}




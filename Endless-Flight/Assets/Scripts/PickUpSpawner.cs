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
    private WaitForSeconds spawnInterval = new WaitForSeconds(4.5f);

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
        allowPickUpSpawn = true;
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
            Debug.Log("Spawning Objects");
            spawnPos = new Vector3(rnd.Next(130,250),rnd.Next(50,150) , player.transform.position.z + 600f);

            int typeOfObjectSpawn = rnd.Next(0, 8);

            if (typeOfObjectSpawn == 0 || typeOfObjectSpawn == 1 || typeOfObjectSpawn == 2)
            {
                SpawnScoreBoosters(spawnPos);
            }

            if (typeOfObjectSpawn == 3 || typeOfObjectSpawn == 4)
            {
                SpawnScoreMultiplier(spawnPos);
            }

			if (typeOfObjectSpawn == 5 || typeOfObjectSpawn == 6)
            {
                SpawnSpeedBoosters(spawnPos);
            }

			if (typeOfObjectSpawn == 7)
			{
				SpawnFuel(spawnPos);
			}
            
            yield return spawnInterval;
        }
    }

    public void SpawnSpeedBoosters(Vector3 spawnPos)
    {
        
            GameObject speedBooster = GameObjectPool.current.GetPooledPickUp("SpeedBooster" + "(Clone)");
            speedBooster.SetActive(true);

            speedBooster.transform.position = spawnPos;
    }

    public void SpawnScoreBoosters(Vector3 spawnPos)
    {
       
            GameObject scoreBooster = GameObjectPool.current.GetPooledPickUp("ScoreBooster" + "(Clone)");
            scoreBooster.SetActive(true);

            scoreBooster.transform.position = spawnPos;

    }


    public void SpawnScoreMultiplier(Vector3 spawnPos)
    { 
        GameObject scoreMultiplier = GameObjectPool.current.GetPooledPickUp("BlueScoreMultiplier"+"(Clone)");
        scoreMultiplier.SetActive(true);

        scoreMultiplier.transform.position = spawnPos;
    }

	private void SpawnFuel(Vector3 spawnPos)
	{
		GameObject fuel = GameObjectPool.current.GetPooledPickUp("Fuel"+"(Clone)");
		fuel.SetActive(true);

		fuel.transform.position = spawnPos;
	}
}




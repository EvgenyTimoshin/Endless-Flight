using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CloudSpawner : MonoBehaviour {

    public GameObject player;
    private bool cloudsSpawning = false;
    private static Random rnd = new Random();
    public int LeftSideSpawnLimitLX = -3200;
    public int LeftSideSpawnLimitRX = 0;

    public int RightSideSpawnLimitLX = 200;
    public int RightSideSpawnLimitRX = 4200;
    public float cloudLiftHeightPlus = 50;

    // Use this for initialization
    void Start () {
        spawnCloudBlock(-10000);
        spawnCloudBlock(-9000);
        spawnCloudBlock(-8000);
        spawnCloudBlock(-7000);
        spawnCloudBlock(-6000);
        spawnCloudBlock(-5000);
        spawnCloudBlock(-4000);
        spawnCloudBlock(-3000);
        spawnCloudBlock(-2000);
        spawnCloudBlock(-1000);
    }
	
	// Update is called once per frame
	void Update () {

		if((int)player.transform.position.z > 0 && (int)player.transform.position.z % 1000 == 0)
        {
            spawnCloudBlock(player.transform.position.z);
        }
	}

    private void spawnCloudBlock(float playerPositonZ)
    {
        float zSpawnPos = playerPositonZ + 8200;

        List<GameObject> clouds = new List<GameObject>();
        
        for(int i = 0; i < 4; i++)
        {
            GameObject newCloud = GameObjectPool.current.GetRandomPooledCloud();
            clouds.Add(newCloud);
            newCloud.SetActive(true);
        }

        ///spawn first left block
        for(int i = 0; i < clouds.Count/2; i++)
        {
            clouds[i].transform.position = new Vector3(rnd.Next(LeftSideSpawnLimitLX,LeftSideSpawnLimitRX), clouds[i].transform.position.y + cloudLiftHeightPlus, rnd.Next((int)zSpawnPos, (int)zSpawnPos + 1000));
            clouds[i].GetComponent<Rigidbody>().velocity = new Vector3(rnd.Next(-15,-10),0,0);
        }

        for(int i = clouds.Count/2; i < clouds.Count; i++)
        {
            clouds[i].transform.position = new Vector3(rnd.Next(RightSideSpawnLimitLX, RightSideSpawnLimitRX), clouds[i].transform.position.y + cloudLiftHeightPlus, rnd.Next((int)zSpawnPos,(int)zSpawnPos+1000));
            clouds[i].GetComponent<Rigidbody>().velocity = new Vector3(rnd.Next(-15, -10), 0, 0);
        }
    }
}

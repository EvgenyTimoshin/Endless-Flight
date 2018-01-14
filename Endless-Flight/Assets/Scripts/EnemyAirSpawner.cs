using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemyAirSpawner : MonoBehaviour {

    Random rnd = new System.Random();
    public GameObject player;
    float leftBorderLimitX;
    float rightBorderLimitX;
    float verticalLowerLimit;
    float verticalUpperLimit;

    public float planeYSpawnPos = 90;
    public bool continueSpawning = false;
    public static float waitTime = 10;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(waitTime);

	// Use this for initialization
	void Start ()
    { 
        PlayerController p = player.GetComponent<PlayerController>();
        leftBorderLimitX = p.leftBorderLimitX;
        rightBorderLimitX = p.rightBorderLimitX;
        verticalLowerLimit = p.verticalLowerLimit;
        verticalUpperLimit = p.verticalUpperLimit;
	}

    private void OnEnable()
    {
        GameStateManager.loadGameStartComponents += StartPlaneSpawning;
        GameStateManager.pauseGame += StopPlaneSpawning;
        GameStateManager.pauseGame += DisablePlanesMovement;
        GameStateManager.resumeGame += ResumeAllPlaneMovement;
        GameStateManager.resumeGame += StartPlaneSpawning;
    }

    private void OnDisable()
    {
        GameStateManager.loadGameStartComponents -= StartPlaneSpawning;
        GameStateManager.pauseGame -= StopPlaneSpawning;
        GameStateManager.resumeGame -= StartPlaneSpawning;
        GameStateManager.pauseGame -= DisablePlanesMovement;
        GameStateManager.resumeGame -= ResumeAllPlaneMovement;
    }

    // Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Invokes an event that repeats and runs the specified method over and over in specified time frame
    /// </summary>
    private void StartPlaneSpawning()
    {
        continueSpawning = true;
        StartCoroutine(SpawnEnemyPlane());
        //InvokeRepeating("SpawnEnemyPlane",10f,15f);
    }

    /// <summary>
    /// Stops all invokes on current script
    /// </summary>
    private void StopPlaneSpawning()
    {
        //CancelInvoke();
        continueSpawning = false;
        StopCoroutine(SpawnEnemyPlane());
        
    }

    private IEnumerator SpawnEnemyPlane()
    {
        while (continueSpawning)
        {
            int planeChoice = rnd.Next(0, 3);
            string typeOfPlane = "";

            if (planeChoice == 0)
            {
                typeOfPlane = "plane";
            }
            else if (planeChoice == 1)
            {
                typeOfPlane = "bomber";
            }
            else if (planeChoice == 2)
            {
                typeOfPlane = "warplane";
            }

            int typeOfSpawn = rnd.Next(0, 3);

            if (typeOfSpawn == 0)
            {
                spawnSingle(typeOfPlane);
            }
            else if (typeOfSpawn == 1)
            {
                spawnDouble(typeOfPlane);
            }
            else if (typeOfSpawn == 2)
            {
                spawnTriple("warplane");
            }

            yield return waitForSeconds;
        }
    }

    /// <summary>
    /// Spawns a plane at players position x in gameworld
    /// </summary>
    /// <param name="typeOfPlane"></param>
    private void spawnSingle(string typeOfPlane)
    {
        GameObject plane = GameObjectPool.current.GetPooledEnemyAir(typeOfPlane + "(Clone)");
        //Debug.Log("Extracted : " + plane.name);
        plane.transform.position = new Vector3(player.transform.position.x, planeYSpawnPos, player.transform.position.z + 4000);
        plane.GetComponent<Rigidbody>().velocity = new Vector3(0,0,-300);
        plane.SetActive(true);
    }


    /// <summary>
    /// Spawns 2 planes at two different modes
    /// </summary>
    /// <param name="typeOfPlane"></param>
    private void spawnDouble(string typeOfPlane)
    {
        GameObject[] planes = new GameObject[2];

        for (int i = 0; i < 2; i++)
        {
            GameObject plane = GameObjectPool.current.GetPooledEnemyAir(typeOfPlane + "(Clone)");
            plane.SetActive(true);
            Debug.Log("Extracted plane" + plane.name);
            planes[i] = plane;
        }

        int spawnChoice = rnd.Next(0, 2);

        string typeOfSpawn = "";

        if(spawnChoice == 0)
        {
            typeOfSpawn = "sidebyside";
        }
        else
        {
            typeOfSpawn = "seperated";
        }

        int planeOneX = rnd.Next((int)leftBorderLimitX, (int)rightBorderLimitX - 85);

        int sideBySideOffset = 40;

        if (typeOfSpawn  == "sidebyside")
        {
            if(typeOfPlane == "bomber")
            {
                sideBySideOffset = 85;
            }

            planes[0].transform.position = new Vector3(planeOneX, planeYSpawnPos, player.transform.position.z + 4000);
            planes[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
           
            planes[1].transform.position = new Vector3(planeOneX + sideBySideOffset, planeYSpawnPos, player.transform.position.z + 4000);
            planes[1].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
        }
        else
        {
            planeOneX = rnd.Next((int)leftBorderLimitX, (int)rightBorderLimitX - (int)leftBorderLimitX);

            sideBySideOffset = 100;

            planes[0].transform.position = new Vector3(planeOneX, planeYSpawnPos, player.transform.position.z + 4000);
            planes[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
            
            planes[1].transform.position = new Vector3(planeOneX + sideBySideOffset, planeYSpawnPos, player.transform.position.z + 4000);
            planes[1].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
        }
    }


    /// <summary>
    /// Spawns 3 planes
    /// </summary>
    /// <param name="typeOfPlane"></param>
    private void spawnTriple(string typeOfPlane)
    {
        GameObject[] planes = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            GameObject plane = GameObjectPool.current.GetPooledEnemyAir(typeOfPlane + "(Clone)");
            plane.SetActive(true);
            Debug.Log("Extracted plane" + plane.name);
            planes[i] = plane;
        }

        int planeOneX = rnd.Next((int)leftBorderLimitX + 45, (int)rightBorderLimitX - 90);

        int sideBySideOffset = 45;

        planes[0].transform.position = new Vector3(planeOneX, planeYSpawnPos, player.transform.position.z + 4000);
        planes[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);

        planes[1].transform.position = new Vector3(planeOneX + sideBySideOffset, planeYSpawnPos, player.transform.position.z + 4100);
        planes[1].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);

        planes[2].transform.position = new Vector3(planeOneX - sideBySideOffset, planeYSpawnPos, player.transform.position.z + 4100);
        planes[2].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);

    }

    private void DisablePlanesMovement()
    {
        List<GameObject> planes = GameObjectPool.current.GetPooledEnemyAirPool();

        foreach(var plane in planes)
        {
            if (plane.activeInHierarchy)
            {
                plane.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            }
        }
    }

    private void ResumeAllPlaneMovement()
    {
        List<GameObject> planes = GameObjectPool.current.GetPooledEnemyAirPool();

        foreach (var plane in planes)
        {
            if (plane.activeInHierarchy)
            {
                plane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -300);
            }
        }
    }

}

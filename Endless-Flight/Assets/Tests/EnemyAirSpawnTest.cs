using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class EnemyAirSpawnTest
{
    private GameObject player;
    private GameObject enemyAirSpawner;
    //Event manager of sorts
    private GameObject gameStateManager;

    /// <summary>
    /// Sets up relevant objects for testing in the scene
    /// </summary>
    public void SetupScene()
    { 
        /*
        player = new GameObject();
        player.AddComponent<Rigidbody>();
        player.AddComponent<PlayerController>();
        player.transform.position = new Vector3(0, 0, 0);
        player.tag = "player";
        */
        player =  GameObject.Instantiate(Resources.Load("Player/transport_plane_green",typeof(GameObject))) as GameObject;
        player.tag = "player";

        var gameObjectPool = new GameObject();
        gameObjectPool.AddComponent<GameObjectPool>();

        var audioListener = new GameObject().AddComponent<AudioListener>();

        gameStateManager = new GameObject();
        gameStateManager.AddComponent<GameStateManager>();

        enemyAirSpawner = new GameObject();
        enemyAirSpawner.AddComponent<EnemyAirSpawner>().player = player;
       // enemyAirSpawner.GetComponent<EnemyAirSpawner>().player = player;

    }

    /// <summary>
    /// Destroys all objects in the scene to allow other tests to have a clean start (clean up method)
    /// </summary>
    public void DestroyScene()
    {
        var objects = Object.FindObjectsOfType<GameObject>();

        foreach (var o in objects)
        {
            Object.Destroy(o.gameObject);
        }
    }


    /// <summary>
    /// Tests to see if planes spawning works
    /// </summary>
    /// <returns></returns>
    [UnityTest]
	public IEnumerator GeneralSpawningTest() {
		
        SetupScene();
        yield return null;

        var enemyPlanes = GameObject.FindGameObjectsWithTag("enemyAir");
        Debug.Log(enemyPlanes.Length);

        //Checks to make sure there is no plane objects active
        if (enemyPlanes.Length != 0)
        {
            Assert.Fail();
        }

        //Runs this function to trigger the other classes that subscibe to the events of this class (begin spawning)
        gameStateManager.GetComponent<GameStateManager>().LoadAllStartGameComponents();

        yield return null;

        //finds all of the active plane objects
        enemyPlanes = GameObject.FindGameObjectsWithTag("enemyAir");

        Debug.Log(enemyPlanes.Length);

        //checks to see if more than 0 objects exist to passs the test
        if (enemyPlanes.Length <= 0)
        {
            Assert.Fail();
        }

        //cleans up the scene for next tests
        DestroyScene();

		yield return null;
	}


    /// <summary>
    /// Tests if enemy planes get disabled after the player passes them in the game world
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator PlaneDisableTest()
    {
        SetupScene();
        yield return null;

        //places the player at a default position for testing
        player.transform.position = new Vector3(0,0,0);

        var enemyPlanes = GameObject.FindGameObjectsWithTag("enemyAir");
        Debug.Log(enemyPlanes.Length);

        //Checks to make sure there is no plane objects active
        if (enemyPlanes.Length != 0)
        {
            Assert.Fail();
        }

        //triggers an event which will allow spawning (event triggered in EnemyAirSpawner script by subscription)
        gameStateManager.GetComponent<GameStateManager>().LoadAllStartGameComponents();

        //skips a frame to allow objects to initialise and react to game
        yield return null;

        enemyPlanes = GameObject.FindGameObjectsWithTag("enemyAir");
        Debug.Log(enemyPlanes.Length);

        //checks to make sure some enemies spawned
        if (enemyPlanes.Length <= 0)
        {
            Assert.Fail();
        }

        //skips a frame
        yield return null;

        //places the player ahead of the enemy planes ( plane spawns at 4000 + player.z postion)
        //Should trigger the enemy plane to diactivate
        player.transform.position = new Vector3(0, 0, 400000);

        //skips a frame to allow update to game
        yield return new WaitForSeconds(10f);

        enemyPlanes = GameObject.FindGameObjectsWithTag("enemyAir");


        Debug.Log(enemyPlanes.Length);
        //Checks to make sure the planes deactivated after the player passes them
        if (enemyPlanes.Length != 0)
        {
            Assert.Fail();
        }


        yield return null;
        DestroyScene();
    }
}

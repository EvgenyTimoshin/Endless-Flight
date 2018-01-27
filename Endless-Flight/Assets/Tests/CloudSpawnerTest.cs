using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CloudTest
{
    private GameObject player;

   /// <summary>
   /// Tests to make sure that when the game starts the clouds spawn accordingly
   /// (Player position on the Z axis is -2200 as that is where the player starts the game)
   /// </summary>
   /// <returns></returns>
   [UnityTest]
    public IEnumerator CloudSpawnerInitialTest()
    {
        //sets up the scene for testing
        SetupScene();
        
        //moves player to desired position
        player.transform.position = new Vector3(0,0,-2200);

        //Gives unity control for 1 second to allow scene to initialise
        yield return new WaitForSeconds(1f);

        int LeftSideSpawnLimitLX = -3200;
        int LeftSideSpawnLimitRX = 0;

        int RightSideSpawnLimitLX = 200;
        int RightSideSpawnLimitRX = 4200;

        //gets all of the active cloud game objects in the scene
        var clouds = GameObject.FindGameObjectsWithTag("cloud");

        Debug.Log(clouds.Length);

        //Allows test to pass if the active objects are indeed active and also in the contraints of the desired position
        foreach (var cloud in clouds)
        {
            if (cloud.activeInHierarchy)
            {
                Assert.GreaterOrEqual(cloud.transform.position.x, LeftSideSpawnLimitLX);
                Assert.LessOrEqual(cloud.transform.position.x, RightSideSpawnLimitRX);
            }
        }

        //Cleans up scene
         DestroyScene();
         yield return null;
    }

    /// <summary>
    /// Tests to make sure clouds are disabled once they are behind the player objects (out of view of render essentially)
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator CloudDisableTest()
    {
        //Sets up scene for testing
        SetupScene();

        //sets player postion to desired location for testing
        player.transform.position = new Vector3(250, 100, 10101);

        //gives unity control for 1 second
        yield return new WaitForSeconds(1f);

        //gets all of the active cloud objects in the scene
        var clouds = GameObject.FindGameObjectsWithTag("cloud");

        Debug.Log(clouds.Length);

        //checks to make sure that the cloud array is no bigger than 0 , (no active clouds in the scene)
        if (clouds.Length != 0)
        {
            Assert.Fail();
        }
        
        //cleans up the scene
        DestroyScene();
        yield return null;

    }

    /// <summary>
    /// Tests to make sure that clouds spawn as the player object moves
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator CloudProceduralSpawningTest()
    {
        //Sets up scene for testing
        SetupScene();

        //sets player postion to desired location for testing
        player.transform.position = new Vector3(250, 100, 10101);

        //gives unity control for 1 second
        yield return new WaitForSeconds(1f);

        //gets all of the active cloud objects in the scene
        var clouds = GameObject.FindGameObjectsWithTag("cloud");

        Debug.Log(clouds.Length);

        //checks to make sure that the cloud array is no bigger than 0 , (no active clouds in the scene)
        if (clouds.Length != 0)
        {
            Assert.Fail();
        }

        //sets player z position to be % 1000 == 0 to trigger clouds spawning
        player.transform.position = new Vector3(250,100,1000);

        //lets unity take control to allow game objects in scene to react to changes
        yield return null;

        //gets all of the active clouds in the scene
        var newClouds = GameObject.FindGameObjectsWithTag("cloud");

        Debug.Log(newClouds.Length);
        //checks to make sure clouds spawned in
        if (newClouds.Length <= 0)
        {
            Assert.Fail();
        }

        //cleans up the scene
        DestroyScene();
    }

    /// <summary>
    /// Sets up relevant objects for testing in the scene
    /// </summary>
    public void SetupScene()
    {
        player = new GameObject();
        player.AddComponent<Rigidbody>();
        player.transform.position = new Vector3(0, 0, 0);
        player.tag = "player";


        var gameObjectPool = new GameObject();
        gameObjectPool.AddComponent<GameObjectPool>();

        var cloudSpawner = new GameObject();
        cloudSpawner.AddComponent<CloudSpawner>();
        cloudSpawner.GetComponent<CloudSpawner>().player = player;
    }

    /// <summary>
    /// Destroys all objects in the scene to allow other tests to have a clean start (clean up method)
    /// </summary>
    public void DestroyScene()
    {
        var objects = GameObject.FindObjectsOfType<GameObject>();

        foreach (var o in objects)
        {
            Object.Destroy(o.gameObject);
        }
    }


}

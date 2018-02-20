using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using NUnit.Framework.Constraints;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

public class GameObjectPool : MonoBehaviour
{
    public static GameObjectPool current;
    private List<GameObject> islandPool;
    private List<GameObject> ringPool;
    private List<GameObject> shipPool;
    private List<GameObject> enemyPlanePool;
    private List<GameObject> cloudsPool;
    private List<GameObject> pickUpPool;
	private List<GameObject> balloonPool;
    private static Random rnd = new Random();

    /// <summary>
    /// Called on awake of the scene, instantiates itself as a static class
    /// </summary>
    public void Awake()
    {
        current = this;
    }

    /// <summary>
    /// Initialises the current class
    /// </summary>
	void Start ()
	{
        islandPool = new List<GameObject>();
	    ringPool = new List<GameObject>();
	    shipPool = new List<GameObject>();
        enemyPlanePool = new List<GameObject>();
        cloudsPool = new List<GameObject>();
	    pickUpPool = new List<GameObject>();
		balloonPool = new List<GameObject> ();
        

        ///Pool islands
	    Object[] list = Resources.LoadAll("Islands");

	    for (int i = 0; i < list.Length; i++)
	    {
	        for (int j = 0; j < 3; j++)
	        {
	            GameObject obj = (GameObject) list[i];
	            obj.SetActive(false);
	            islandPool.Add(Instantiate(obj));
	        }
	    }


	    //Pool rings
	    list = Resources.LoadAll("Rings");

	    for (int i = 0; i < list.Length; i++)
	    {
	        for (int j = 0; j < 10; j++)
	        {
	            GameObject obj = (GameObject)list[i];
	            obj.SetActive(false);
	            ringPool.Add(Instantiate(obj));
	            Debug.Log(list[i].name);
            }
	    }

        //Pool ships
	    list = Resources.LoadAll("Ships");

	    for (int i = 0; i < list.Length; i++)
	    {
	        for (int j = 0; j < 3; j++)
	        {
	            GameObject obj = (GameObject)list[i];
	            obj.SetActive(false);
	            shipPool.Add(Instantiate(obj));
	            //Debug.Log(list[i].name);
	        }
	    }

        //pool enemy aircrafts
        list = Resources.LoadAll("EnemyAir");

        for(int i = 0; i < list.Length; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                GameObject obj = (GameObject)list[i];
                obj.SetActive(false);
                enemyPlanePool.Add(Instantiate(obj));
            }
        }

        //pool clouds
        list = Resources.LoadAll("Clouds");

        for (int i = 0; i < list.Length; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                GameObject obj = (GameObject)list[i];
                obj.SetActive(false);
                cloudsPool.Add(Instantiate(obj));
            }
        }

	    list = Resources.LoadAll("PickUps");

	    for (int i = 0; i < list.Length; i++)
	    {
	        for (int j = 0; j < 10; j++)
	        {
	            GameObject obj = (GameObject)list[i];
	            obj.SetActive(false);
	            pickUpPool.Add(Instantiate(obj));
	        }
	    }

		list = Resources.LoadAll("Balloons");

		for (int i = 0; i < list.Length; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				GameObject obj = (GameObject)list[i];
				obj.SetActive(false);
				balloonPool.Add(Instantiate(obj));
				Debug.Log(list[i].name);
			}
		}
    }
	
    /// <summary>
    /// Called when an island is needed
    /// </summary>
    /// <param name="desiredObj"></param>
    /// <returns>An island from the island pool</returns>
    public GameObject GetPooledIsland(string desiredObj)
    {
        var island = islandPool.Find(i => i.name == desiredObj && !i.activeInHierarchy);

        return island;
    }

    /// <summary>
    /// Called when a ring is needed
    /// </summary>
    /// <param name="desiredObj"></param>
    /// <returns>A ring from the ring pool</returns>
    public GameObject GetPooledRing(string desiredObj)
    {
        foreach (var ob in ringPool)
        {   
            if (desiredObj == ob.name && !ob.activeInHierarchy)
            {
                return ob;
            }
            
        }

        return null;
    }

    /// <summary>
    /// Called when a ship is needed
    /// </summary>
    /// <param name="index"></param>
    /// <returns><see langword="abstract"/>ship from the ship pool</returns>
    public GameObject GetPooledShip(int index)
    {
        Debug.Log("Trying to spawn : " + shipPool[index] + "Out of "  + shipPool.Count );
        Debug.Log(index);

        return shipPool[index];
    }

    public GameObject GetPooledEnemyAir(string desiredObj)
    {
        var plane = enemyPlanePool.Find(p => p.name == desiredObj && !p.activeInHierarchy);

        return plane;
    }

    public GameObject GetRandomPooledCloud()
    {
        int cloudChoice = rnd.Next(cloudsPool.Count);

        while(cloudsPool[cloudChoice].activeInHierarchy)
        {
            cloudChoice = rnd.Next(cloudsPool.Count);
        }

        return cloudsPool[cloudChoice];
    }

    public GameObject GetPooledPickUp(string desiredPickup)
    {
        var pickUp = pickUpPool.Find(p => p.name == desiredPickup && !p.activeInHierarchy);

        return pickUp;
    }

	public GameObject GetPooledBalloon()
	{
		var balloon = balloonPool.Find(p => !p.activeInHierarchy);

		return balloon;
	}

    public List<GameObject> GetPooledEnemyAirPool()
    {
        return enemyPlanePool;
    }

    public List<GameObject> GetPooledClouds()
    {
        return cloudsPool;
    }

    public List<GameObject> GetPooledPickUps()
    {
        return pickUpPool;
    }


}

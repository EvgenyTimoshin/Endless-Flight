using System;
//using System.Collections;
//using System.Collections.Generic;
//using NUnit.Framework.Constraints;
using UnityEngine;
using Random = System.Random;

public class IslandScript : MonoBehaviour
{
    private float NextIslandDistance = 3500;

    /// <summary>
    /// Sets up current class
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// Called when rigid body (player) exits the collider of current object
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        gameObject.SetActive(false);

        Spawn_Next_Island();
        
    }

    /// <summary>
    /// Called when rigid body enters collider of current object
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        

    }

    /// <summary>
    /// Spawns a random island further in the game world *****************needs changing
    /// </summary>
    void Spawn_Next_Island()
    {
        Random rnd = new Random();
        int choice = rnd.Next(1, 5);
        String islandChoice = "Island2";

        if (choice == 0)
        {
            islandChoice = "island1";
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
            islandChoice = "Island5";
        }
        

        GameObject island = GameObjectPool.current.GetPooledIsland(islandChoice + "(Clone)");
        //if(island == null) return;
        
        Debug.Log("Extracted : " + island.name);
        island.transform.position = new Vector3(0, 0, transform.position.z + NextIslandDistance);
        island.SetActive(true);
        NextIslandDistance += 200;

    }
}

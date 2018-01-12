using System;
//using System.Collections;
//using System.Collections.Generic;
//using NUnit.Framework.Constraints;
using UnityEngine;
using Random = System.Random;

public class IslandScript : MonoBehaviour
{
    public delegate void ScenerySpawn();
    public static event ScenerySpawn spawnScenery;
    private String playerTag = "player";

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
        if (other.tag == playerTag)
        {
            gameObject.SetActive(false);
            spawnScenery();
        }


    }
}

   

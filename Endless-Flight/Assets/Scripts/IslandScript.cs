using System;
//using System.Collections;
//using System.Collections.Generic;
//using NUnit.Framework.Constraints;
using UnityEngine;
using Random = System.Random;

public class IslandScript : MonoBehaviour
{

    void Start()
    {
        BoxCollider b = GetComponentInParent<BoxCollider>();
        b.isTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);

        Spawn_Next_Island();
        
    }

    void OnTriggerEnter(Collider other)
    {
        

    }

    void Spawn_Next_Island()
    {
        /*
        Random rnd = new Random();
        int choice = rnd.Next(1, 4);
        String islandChoice = "Island/Island2";

        if (choice == 1)
        {
            islandChoice = "Islands/Island2";
        }
        if (choice == 2)
        {
            islandChoice = "Islands/Island3";
        }
        if (choice == 3)
        {
            islandChoice = "Islands/Island4";
        }
        if (choice == 4)
        {
            islandChoice = "Islands/Island5";
        }
        Instantiate((Resources.Load(islandChoice)), new Vector3(0, 0, transform.position.z + 3200), new Quaternion(0, 0, 0, 0));
        */
        
        Random rnd = new Random();
        int choice = rnd.Next(1, 5);
        String islandChoice = "Island2";

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
        island.transform.position = new Vector3(0, 0, transform.position.z + 3200);
        island.SetActive(true);

    }
}

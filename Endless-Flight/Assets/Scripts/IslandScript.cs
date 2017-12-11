using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = System.Random;

public class IslandScript : MonoBehaviour {

    private float townPosZ = 490;
    //private bool Loaded = false;
    public int i = 0;

    void Start()
    {
        BoxCollider b = GetComponentInParent<BoxCollider>();
        b.isTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);

        i++;

        Spawn_Next_Island();
        
    }

    void OnTriggerEnter(Collider other)
    {
        

    }

    void Spawn_Next_Island()
    {
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
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

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
        Instantiate((Resources.Load("Islands/Island2")), new Vector3(0, 0, transform.position.z + 1600), new Quaternion(0, 0, 0, 0));
    }
}

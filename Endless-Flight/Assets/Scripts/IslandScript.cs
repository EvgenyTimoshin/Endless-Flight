using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandScript : MonoBehaviour {


    void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        
    }
}

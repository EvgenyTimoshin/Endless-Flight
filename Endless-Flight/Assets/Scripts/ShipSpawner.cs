using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{

    private float shipPosOffset = 600;
	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnRandomShip", 2f, 15.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnRandomShip()
    {
        System.Random rnd = new System.Random();
        int rand = rnd.Next(0, 15);
        GameObject ship = GameObjectPool.current.GetPooledShip(rand);
        ship.SetActive(true);
        ship.transform.position = new Vector3(rand,ship.transform.position.y,transform.position.z + shipPosOffset);
        //ship.transform.rotation = new Quaternion(0, rnd.Next(0,90), 0,0);
        Debug.Log("Ship Spawned");
    }
}

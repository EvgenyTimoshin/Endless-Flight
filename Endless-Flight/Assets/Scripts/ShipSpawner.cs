using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{

    private float shipPosOffset = 2000;
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
		ship.transform.position = new Vector3(rnd.Next(-300,700),ship.transform.position.y,transform.position.z + shipPosOffset);
		rand = rnd.Next (0, 2);
		float degree;
		if (rand == 0)
			degree = 90;
		else
			degree = 0;
			
        //ship.transform.rotation = new Quaternion(0, degree, 0,0);
        Debug.Log("Ship Spawned");
    }
}

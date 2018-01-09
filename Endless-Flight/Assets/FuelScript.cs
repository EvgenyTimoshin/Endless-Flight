using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0,0,0.7f);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player")
        {
            gameObject.SetActive(false);
            other.GetComponent<PlayerStats>().modifyFuelBy(30);
        }
    }
}

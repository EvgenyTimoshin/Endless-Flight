using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoosterScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player")
        {
            gameObject.SetActive(false);
            other.GetComponent<PlayerController>().BeginSpeedBoost(100, 5);
            other.GetComponent<PlayerStats>().SetScoreMultiplier(5, 5);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoosterScript : MonoBehaviour, IPickUp {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            PickUpInteraction(other);
        }
    }

    public void PickUpInteraction(Collider other)
    {
        PickUpDestroy();
        other.GetComponent<PlayerController>().BeginSpeedBoost(100, 5);
        other.GetComponent<PlayerStats>().SetScoreMultiplier(5, 5);
        
    }

    public void PickUpDestroy()
    {
        gameObject.SetActive(false);
    }
}

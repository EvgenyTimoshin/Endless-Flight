using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RingScript : MonoBehaviour
{

    public GameObject player;
	
    /// <summary>
    /// Initialises current class
    /// </summary>
	void Start () {
		
		BoxCollider b = GetComponentInParent<BoxCollider>();
		b.isTrigger = true;
        player = GameObject.Find("transport_plane_green");
		
	}
	
	/// <summary>
    /// Called once per frame
    /// </summary>
	void Update ()
	{
	    if (transform.position.z < player.transform.position.z - 200)
	    {
            gameObject.SetActive(false);
	    }
	}

	void OnTriggerExit(Collider other)
	{

	}

    /// <summary>
    /// Called when rigid body enters the current object, deletes the current ring and calls player to increase score
    /// </summary>
    /// <param name="other"></param>
	void OnTriggerEnter(Collider other)
	{
        gameObject.SetActive(false);
	    //PlayerStats p = other.gameObject.GetComponent<PlayerStats>();

	}

	
		
}

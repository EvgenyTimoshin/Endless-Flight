using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NUnit.Framework.Constraints;
using Random = System.Random;

public class RingScript : MonoBehaviour
{
    private float RingLifeSpan = 600;

    public GameObject player;
	// Use this for initialization
	void Start () {
		
		BoxCollider b = GetComponentInParent<BoxCollider>();
		b.isTrigger = true;
        player = GameObject.Find("transport_plane_green");
		
	}
	
	// Update is called once per frame
	void Update ()
	{
        /*
	    RingLifeSpan --;
	    if (RingLifeSpan < 0)
	    {
	        Destroy(gameObject);
	    }
        */

	    if (transform.position.z < player.transform.position.z - 200)
	    {
            gameObject.SetActive(false);
	    }
	}

	void OnTriggerExit(Collider other)
	{

	}

	void OnTriggerEnter(Collider other)
	{
	    //Destroy(gameObject);
        gameObject.SetActive(false);
	    Player p = other.gameObject.GetComponent<Player>();
        p.Increase_Score();
	}

	
		
}

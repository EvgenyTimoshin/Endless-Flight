using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework.Constraints;
using Random = System.Random;

public class RingScript : MonoBehaviour
{

    private float RingLifeSpan = 180;

	// Use this for initialization
	void Start () {
		
		BoxCollider b = GetComponentInParent<BoxCollider>();
		b.isTrigger = true;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    RingLifeSpan --;
	    if (RingLifeSpan < 0)
	    {
	        Destroy(gameObject);
	    }
	}

	void OnTriggerExit(Collider other)
	{

	}

	void OnTriggerEnter(Collider other)
	{
	    Destroy(gameObject);
	    Player p = other.gameObject.GetComponent<Player>();
        p.Increase_Score();
	}

	
		
}

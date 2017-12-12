using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework.Constraints;
using Random = System.Random;

public class RingScript : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
		
		BoxCollider b = GetComponentInParent<BoxCollider>();
		b.isTrigger = true;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerExit(Collider other)
	{

	}

	void OnTriggerEnter(Collider other)
	{
	    Destroy(gameObject);
	    //other.increaseScore();
	}

	
		
}

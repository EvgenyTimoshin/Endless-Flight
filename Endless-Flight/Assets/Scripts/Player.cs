using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{

    
	// Use this for initialization
	void Start ()
	{

	    GetComponent<Animation>().Play("TakeOff");


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

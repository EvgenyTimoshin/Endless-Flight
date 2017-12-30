using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	/// <summary>
    /// Called every frame, rotates the propellor
    /// </summary>
	void Update () {
	    transform.Rotate(Vector3.right * 40);
    }
}

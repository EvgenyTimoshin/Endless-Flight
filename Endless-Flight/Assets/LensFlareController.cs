using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensFlareController : MonoBehaviour {

	public float SunSpeed = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x+SunSpeed,transform.position.y,transform.position.z);
	}
}

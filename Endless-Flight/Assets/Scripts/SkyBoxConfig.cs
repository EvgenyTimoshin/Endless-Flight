using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxConfig : MonoBehaviour {

	public float skyBoxRotSpeed = 1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyBoxRotSpeed);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    const float minimumSpeed = -1500;
    const float maxHorizontalSpeed = 5000;
    float currentHorizontalSpeed = 0;
    //float maxHorizontalSpeed = 5000;
    float bonusHorizontalSpeed = 0;
    float boostHorizontalSpeed = 0;
 

    // Use this for initialization
    void Start ()
	{
	    rb = GetComponent<Rigidbody>();
	    //Animator anim = GetComponent<Animator>();
	    //anim.Play("TakeOff");

	}
	
	// Update is called once per frame
	void Update () {

	    rb.AddRelativeForce(Vector3.forward * 25 * Time.deltaTime * 50);

	    if (Input.GetKey(KeyCode.A))
	    {
	        transform.Rotate(0, 0, Time.deltaTime * 10);
	        currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, maxHorizontalSpeed + bonusHorizontalSpeed + boostHorizontalSpeed, Time.deltaTime / 0.2f);
	    }

        if (Input.GetKey(KeyCode.D))
	    {
            transform.Rotate(0,0,-Time.deltaTime * 10);
	        //currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, -maxHorizontalSpeed + -bonusHorizontalSpeed + -boostHorizontalSpeed, Time.deltaTime / 0.2f);
	    }

	    if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
	    {
	        currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, 0, Time.deltaTime / 0.1f);
	    }
        /*
	    if (transform.position.x < -3800)
	    {
	        transform.position = new Vector3(-3800, transform.position.y, transform.position.z);
	        currentHorizontalSpeed = 0;
	    }

	    if (transform.position.x > -630)
	    {
	        transform.position = new Vector3(-630, transform.position.y, transform.position.z);
	        currentHorizontalSpeed = 0;
	    }
        */

	    rb.velocity = new Vector3(currentHorizontalSpeed, rb.velocity.y, rb.velocity.z);


    }

}

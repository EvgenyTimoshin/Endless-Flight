using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    const float minimumSpeed = -1500;
    const float maxHorizontalSpeed = 250;
    float currentHorizontalSpeed = 0;
    private float currentRotation = 0;
    private Vector3 currentAngle;

    //float maxHorizontalSpeed = 5000;
    float bonusHorizontalSpeed = 0;
    float boostHorizontalSpeed = 0;

    private bool controlZone = true;
 

    // Use this for initialization
    void Start ()
	{
	    rb = GetComponent<Rigidbody>();
	    currentAngle = rb.transform.eulerAngles;
        //Animator anim = GetComponent<Animator>();
        //anim.Play("TakeOff");

    }
	
	// Update is called once per frame
	void Update () {

	    rb.AddRelativeForce(Vector3.forward  * Time.deltaTime * 50);

	    if (Input.GetKey(KeyCode.A))
	    {
            //transform.Rotate(0, 0, Time.deltaTime * 50);
            //currentRotation = Mathf.Lerp(45,currentRotation, Time.deltaTime * 50 );

	        currentAngle = new Vector3(
	            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.z, 45, Time.deltaTime));

            currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, -maxHorizontalSpeed + -bonusHorizontalSpeed + -boostHorizontalSpeed, Time.deltaTime );
            
	    }

        if (Input.GetKey(KeyCode.D))
	    {
            // transform.Rotate(0,0,-Time.deltaTime * 50);
	        currentAngle = new Vector3(
	            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.z, -45, Time.deltaTime));

            currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, maxHorizontalSpeed + bonusHorizontalSpeed + boostHorizontalSpeed, Time.deltaTime );
        }

	    if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
	    {
	        currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, 0, Time.deltaTime / 0.1f);
            // currentRotation = Mathf.Lerp(0, rb.rotation.z, Time.deltaTime * 50);
	        currentAngle = new Vector3(
	            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime * 2));
        }
        
	    if (transform.position.x < 150)
	    {
	        transform.position = new Vector3(150, transform.position.y, transform.position.z);
	        currentHorizontalSpeed = 0;
	        currentAngle = new Vector3(
	            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime * 2));
            Input.ResetInputAxes();
             
        }

	    if (transform.position.x > 350)
	    {
	        transform.position = new Vector3(350, transform.position.y, transform.position.z);
	        currentHorizontalSpeed = 0;
	        currentAngle = new Vector3(
	            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
	            Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime * 2));
            Input.ResetInputAxes();
        }
        

	    rb.velocity = new Vector3(currentHorizontalSpeed, rb.velocity.y, rb.velocity.z);
        //rb.rotation = new Quaternion(rb.rotation.x, rb.rotation.y, currentRotation, rb.rotation.w);
        //rb.rotation.Set(rb.rotation.x, rb.rotation.y, 45, rb.rotation.w);
	    rb.transform.eulerAngles = currentAngle;


    }

}

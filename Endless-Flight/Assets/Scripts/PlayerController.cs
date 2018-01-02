using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;


/// <summary>
/// Main Player class that controls the behaviour of the player object
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    Rigidbody rb;
    public float forwardVelocity = 100;
    public float minimumSpeed = -1500;
    public float maxHorizontalSpeed = 250;
    float currentHorizontalSpeed = 0;
    private float currentRotation = 0;
    private Vector3 currentAngle;
    public float leftBorderLimitX = 150;
    public float rightBorderLimitX = 350;
    //float maxHorizontalSpeed = 5000;
    public float bonusHorizontalSpeed = 0;
    public float boostHorizontalSpeed = 0;

    private float screenCenterX;

	public bool moving = false;

    /// <summary>
    /// Initialises the class
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        currentAngle = rb.transform.eulerAngles;
        screenCenterX = Screen.width * 0.5f;
        mainCamera = Camera.main;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }


    void DebugCode()
    {
        //Debug.Log(rb.velocity);
    }

    /// <summary>
    /// Called every frame update of the game
    /// </summary>
    void Update()
    {
        DebugCode();
        
        if (Input.GetKey(KeyCode.A))
        {
            moveLeft();
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveRight();
        }

        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, 0, Time.deltaTime / 0.1f);
            currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime * 2));
        }

        if (transform.position.x < leftBorderLimitX)
        {
            transform.position = new Vector3(leftBorderLimitX + 1, transform.position.y, transform.position.z);
            currentHorizontalSpeed = 0;
            currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime * 2));
            Input.ResetInputAxes();

        }

        if (transform.position.x > rightBorderLimitX)
        {
            transform.position = new Vector3(rightBorderLimitX - 1, transform.position.y, transform.position.z);
            currentHorizontalSpeed = 0;
            currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime * 2));
            Input.ResetInputAxes();
        }
 
        //Touchscreen Controls
	    foreach (Touch touch in Input.touches)
	    {
	        if (touch.position.x < Screen.width / 2)
	        {
	            moveLeft();
            }
	        else if (touch.position.x > Screen.width / 2)
	        {
	            moveRight();

	        }
            
	    }

        
        if (Input.acceleration.x < 0.19)
        {
            moveLeft();
        }

        if (Input.acceleration.x > -0.19)
        {
            moveRight();
        }
    }

    /// <summary>
    /// Called Every Physics step
    /// </summary>
    void FixedUpdate()
    {
        if (moving)
        {
            rb.AddRelativeForce(Vector3.forward * Time.deltaTime * forwardVelocity);

            rb.velocity = new Vector3(currentHorizontalSpeed, rb.velocity.y, rb.velocity.z);
            rb.transform.eulerAngles = currentAngle;
        }
    }

    /// <summary>
    /// Controls behaviour of object moving to the right
    /// </summary>
    void moveRight()
    {
        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
            Mathf.LerpAngle(currentAngle.z, -70, Time.deltaTime));
        currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, maxHorizontalSpeed + bonusHorizontalSpeed + boostHorizontalSpeed, Time.deltaTime);
    }

    /// <summary>
    /// Controls behaviour of object moving to the left
    /// </summary>
    void moveLeft()
    {
        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
            Mathf.LerpAngle(currentAngle.z, 70, Time.deltaTime));
       currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, -maxHorizontalSpeed + -bonusHorizontalSpeed + -boostHorizontalSpeed, Time.deltaTime);
    }

    /// <summary>
    /// Called to enable movement of plane
    /// </summary>
	public void EnableMovement()
	{
        rb.isKinematic = false;
        rb.velocity = new Vector3(0, 0, 20);
        moving = true;
	}

    public void TakeOffAnim()
    {
        Animation anim = GetComponent<Animation>();
        anim.Play("TakeOff");
    }

}

    



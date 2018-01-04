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

    public float maxVerticalSpeed = 250;
    private float currentVerticalSpeed = 0;
    
    private float currentRotation = 0;
    private Vector3 currentAngle;
    public float leftBorderLimitX = 150;
    public float rightBorderLimitX = 350;
    public float verticalUpperLimit = 100;
    public float verticalLowerLimit = 20;
    //float maxHorizontalSpeed = 5000;
    public float bonusHorizontalSpeed = 0;
    public float boostHorizontalSpeed = 0;

    private float screenCenterX;

	public bool moving = false;

    private Vector3 storedVelocity = new Vector3(0, 0, 20);

    /// <summary>
    /// Called when this script is enabled
    /// </summary>
    private void OnEnable()
    {
        GameStateManager.loadGameStartComponents += TakeOffAnim;
        GameStateManager.pauseGame += DisableMovement;
        GameStateManager.resumeGame += EnableMovement;
    }

    /// <summary>
    /// Called when this script is disabled
    /// </summary>
    private void OnDisable()
    {
        GameStateManager.loadGameStartComponents -= TakeOffAnim;
        GameStateManager.pauseGame -= DisableMovement;
        GameStateManager.resumeGame -= EnableMovement;
    }

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

    /// <summary>
    /// Used to log information in console
    /// </summary>
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

        if (moving)
        {
            CheckPlayerBorders();
            CheckControlInput();
            KeyBoardInput();

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

            //Accelerometer controls
            if (Input.acceleration.x < 0.19)
            {
                moveLeft();
            }

            if (Input.acceleration.x > -0.19)
            {
                moveRight();
            }
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
            rb.velocity = new Vector3(currentHorizontalSpeed, currentVerticalSpeed, rb.velocity.z);
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

    void moveUp()
    {
        currentAngle = new Vector3(
        Mathf.LerpAngle(currentAngle.x, -50, Time.deltaTime),
            currentAngle.y,
            currentAngle.z);
        currentVerticalSpeed = Mathf.Lerp(currentVerticalSpeed, +maxVerticalSpeed, Time.deltaTime);
    }

    void moveDown()
    {
        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, 50, Time.deltaTime),
            currentAngle.y,
            currentAngle.z);
        currentVerticalSpeed = Mathf.Lerp(currentVerticalSpeed, -maxVerticalSpeed, Time.deltaTime);
    }


    private void CheckControlInput()
    {
        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, 0, Time.deltaTime / 0.1f);
            currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, currentAngle.x, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime * 2));
        }

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime * 2),
                Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
                Mathf.LerpAngle(currentAngle.z, currentAngle.z, Time.deltaTime));
            currentVerticalSpeed = Mathf.Lerp(currentVerticalSpeed, 0, Time.deltaTime / 0.1f);
        }
    }
    /// <summary>
    ///Contains code for checking if player is in the game movement bounds 
    /// </summary>
    private void CheckPlayerBorders()
    {
        
        if (transform.position.y > verticalUpperLimit)
        {
            transform.position = new Vector3(transform.position.x, verticalUpperLimit - 1, transform.position.z);
            currentVerticalSpeed = 0;
        }

        if (transform.position.y < verticalLowerLimit)
        {
            transform.position = new Vector3(transform.position.x, verticalLowerLimit + 1, transform.position.z);
            currentVerticalSpeed = 0;
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
    }

    /// <summary>
    /// Contains code for all keyboard input relating to player control
    /// </summary>
    private void KeyBoardInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            moveLeft();
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveRight();
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveUp();
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDown();
        }
    }

    /// <summary>
    /// Called to enable movement of plane (called from animation (as an event))
    /// </summary>
	public void EnableMovement()
	{
        rb.isKinematic = false;
        rb.velocity = storedVelocity;
        moving = true;
	}

    /// <summary>
    /// Disables player movement
    /// </summary>
    public void DisableMovement()
    {
        storedVelocity = rb.velocity;
        rb.isKinematic = true;
        moving = false;
    }

    public void TakeOffAnim()
    {
        Animation anim = GetComponent<Animation>();
        anim.Play("TakeOff");
    }

}

    



using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;


/// <summary>
/// Main Player class that controls the behaviour of the player object
/// </summary>
public class PlayerController : MonoBehaviour, IPausable
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
    public float bonusHorizontalSpeed = 0;
    public float boostHorizontalSpeed = 0;

    private float screenCenterX;
    public float startSpeed = 40f;
	public bool moving = false;

    private Vector3 storedVelocity;

    private bool speedBoosted = false;
    private int speedBoostValue = 0;
    private float speedBoostTimeout = 0;
    private float speedBoostTimer = 0;


    /// <summary>
    /// Called when this script is enabled
    /// </summary>
    private void OnEnable()
    {
        GameStateManager.loadGameStartComponents += TakeOffAnim;
        GameStateManager.pauseGame += Pause;
        GameStateManager.resumeGame += Resume;
    }

    /// <summary>
    /// Called when this script is disabled
    /// </summary>
    private void OnDisable()
    {
        GameStateManager.loadGameStartComponents -= TakeOffAnim;
        GameStateManager.pauseGame -= Pause;
        GameStateManager.resumeGame -= Resume;
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
        storedVelocity = new Vector3(0, 0, startSpeed);
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
    /// Begins boosted speed
    /// </summary>
    /// <param name="boostTimes"></param>
    /// <param name="boostTime"></param>
    public void BeginSpeedBoost(int boostValue, int boostTime)
    {
        speedBoosted = true;
        speedBoostTimeout = boostTime;
        speedBoostValue = boostValue;
    }

    /// <summary>
    /// Called Every Physics step
    /// </summary>
    void FixedUpdate()
    {
        if (moving)
        {
            if(speedBoosted)
            {
                
                BoostPlayerSpeed();
            }

            rb.velocity = new Vector3(currentHorizontalSpeed, currentVerticalSpeed, rb.velocity.z);
            rb.transform.eulerAngles = currentAngle;
        }
    }

    /// <summary>
    /// Runs the calculation for player's speed boost
    /// </summary>
    private void BoostPlayerSpeed()
    {
        speedBoostTimer += Time.deltaTime;
        if (speedBoostTimer < speedBoostTimeout)
        {
            rb.AddRelativeForce(Vector3.forward  * speedBoostValue);
        }
        else
        {
            speedBoosted = false;
            speedBoostTimer = 0;
            speedBoostTimeout = 0;
            speedBoostValue = 0;
            Resume();
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
        Mathf.LerpAngle(currentAngle.x, -35, Time.deltaTime),
            currentAngle.y,
            currentAngle.z);
        currentVerticalSpeed = Mathf.Lerp(currentVerticalSpeed, +maxVerticalSpeed, Time.deltaTime/2);
    }

    void moveDown()
    {
        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, 35, Time.deltaTime),
            currentAngle.y,
            currentAngle.z);
        currentVerticalSpeed = Mathf.Lerp(currentVerticalSpeed, -maxVerticalSpeed, Time.deltaTime/2);
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
            Input.ResetInputAxes();
        }

        if (transform.position.y < verticalLowerLimit)
        {
            transform.position = new Vector3(transform.position.x, verticalLowerLimit + 1, transform.position.z);
            currentVerticalSpeed = 0;
            Input.ResetInputAxes();
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

    public void TakeOffAnim()
    {
        Animation anim = GetComponent<Animation>();
        anim.Play("TakeOff");
    }

    public void Pause()
    {
        storedVelocity = rb.velocity;
        rb.isKinematic = true;
        moving = false; ;
    }

    public void Resume()
    {
        rb.velocity = storedVelocity;
        rb.isKinematic = false;

        moving = true;
        rb.velocity = storedVelocity;
		BoxCollider[] boxCols = GetComponents<BoxCollider>();

		foreach (var b in boxCols) {
			b.enabled = true;
		}
    }

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "pickUp" && other.tag != "island")
		{
			//gameObject.SetActive(false);
			GetComponent<Rigidbody>().useGravity = true;
			GetComponent<Rigidbody>().mass = 2;
			GetComponent<PlayerController>().enabled = false;

			GetComponent<Rigidbody>().transform.Rotate(0.2f,0.2f,0);

			Debug.Log ("Crashed into :  " + other);
		}
		//PlayerStats p = other.gameObject.GetComponent<PlayerStats>();
	}
}

    



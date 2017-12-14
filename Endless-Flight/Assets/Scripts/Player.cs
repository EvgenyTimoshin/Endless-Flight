using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Camera mainCamera;
    Rigidbody rb;
    const float minimumSpeed = -1500;
    const float maxHorizontalSpeed = 350;
    float currentHorizontalSpeed = 0;
    private float currentRotation = 0;
    private Vector3 currentAngle;

    //float maxHorizontalSpeed = 5000;
    float bonusHorizontalSpeed = 0;

    float boostHorizontalSpeed = 0;
    private bool FirstPersonView = false;
    private float screenCenterX;
    private float score = 0;
    public Text scoreText;


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentAngle = rb.transform.eulerAngles;
        screenCenterX = Screen.width * 0.5f;
        mainCamera = Camera.main;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        //Animator anim = GetComponent<Animator>();

        //anim.enabled = true;
        //anim["NewTakeOff"].wrapMode = WrapMode.Once;
        //anim.Play("NewTakeOff");
        //anim.enabled = false;
        // InvokeRepeating("spawnTerrain", 0, 0.5f);


    }

    

    // Update is called once per frame
    void Update()
    {

        rb.AddRelativeForce(Vector3.forward * 2 * Time.deltaTime * 70);

        if (Input.GetKey(KeyCode.A))
        {
            //transform.Rotate(0, 0, Time.deltaTime * 50);
            //currentRotation = Mathf.Lerp(45,currentRotation, Time.deltaTime * 50 );

            moveLeft();

        }

        if (Input.GetKey(KeyCode.D))
        {
            moveRight();
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

    void moveRight()
    {
        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
            Mathf.LerpAngle(currentAngle.z, -70, Time.deltaTime));

        currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, maxHorizontalSpeed + bonusHorizontalSpeed + boostHorizontalSpeed, Time.deltaTime);
    }

    void moveLeft()
    {
        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, 0, Time.deltaTime),
            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime),
            Mathf.LerpAngle(currentAngle.z, 70, Time.deltaTime));

        currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, -maxHorizontalSpeed + -bonusHorizontalSpeed + -boostHorizontalSpeed, Time.deltaTime);
    }

    public void Increase_Score()
    {
        score++;
        scoreText.text = "Score :  " + score;
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
    }
}

    



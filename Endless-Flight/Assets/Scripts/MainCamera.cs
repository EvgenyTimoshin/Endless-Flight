using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public GameObject Player;
    public bool gameStarted = false;
    public bool cameraLockToPlayer = false;
    private Vector3 currentAngle;

    /// <summary>
    /// Used to initialise curretn class
    /// </summary>
    void Start ()
    {
        currentAngle = transform.eulerAngles;
    }
	
	/// <summary>
    /// Called once per frame
    /// </summary>
	void Update () { 

        

	    if (cameraLockToPlayer)
	    {
	        transform.position =
	            new Vector3(Mathf.Lerp(transform.position.x, Player.transform.position.x, Time.deltaTime * 5),
	                Player.transform.position.y + 20, Player.transform.position.z - 80);
            transform.eulerAngles = new Vector3(5,0,0);
	    }

	}

    void LateUpdate()
    {
        if (gameStarted)
        {
            LerpCameraToGameStart();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
    }

    private void LerpCameraToGameStart()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, Player.transform.position.x, Time.deltaTime/1.5f),
                Mathf.Lerp(transform.position.y, Player.transform.position.y + 20, Time.deltaTime/1.5f),
                Mathf.Lerp(transform.position.z, Player.transform.position.z - 80, Time.deltaTime/1.5f));

        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, 5, Time.deltaTime/1.5f),
            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime/1.5f),
            Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime));

        transform.eulerAngles = currentAngle;

        if (transform.position.x > Player.transform.position.x - 0.5f)
        {
            gameStarted = false;
            cameraLockToPlayer = true;
            GameStateManager.current.LoadAllGameComponents();
        }
    }
}

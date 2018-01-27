using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public GameObject Player;
    public bool gameStarted = false;
    public bool cameraLockToPlayer = false;
    private Vector3 currentAngle;
    public int cameraType = 0;
    public Vector3 cameraMenuPosition;
    public float cameraTilt = 10;
    private bool cameraEndScreen = false;

    /// <summary>
    /// Used to initialise curretn class
    /// </summary>
    void Start ()
    {
        currentAngle = transform.eulerAngles;
        transform.position = cameraMenuPosition;
    }

    /// <summary>
    /// Called when this script is enabled
    /// </summary>
    private void OnEnable()
    {
        GameStateManager.startGame += StartGame;
        GameStateManager.gameOver += CameraMoveGameOver;
    }

    /// <summary>
    /// Called when this script is enabled
    /// </summary>
    private void OnDisable()
    {
        GameStateManager.startGame -= StartGame;
        GameStateManager.gameOver -= CameraMoveGameOver;
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update () {

	    if (Input.GetKeyDown(KeyCode.E))
	    {
	        switchCamera();
	    }

	}

    /// <summary>
    /// Called every physics frame updated
    /// </summary>
    void LateUpdate()
    {
        if (gameStarted)
        {
            LerpCameraToGameStart();
        }

        if (cameraLockToPlayer && !cameraEndScreen)
        {
            if (cameraType == 0)
            {
                transform.position =
                    new Vector3(Mathf.Lerp(transform.position.x, Player.transform.position.x, Time.deltaTime * 5),
                        Mathf.Lerp(transform.position.y, Player.transform.position.y + 20, Time.deltaTime + 5), Player.transform.position.z - 80);
                transform.eulerAngles = new Vector3(cameraTilt, 0, 0);
            }
            else
            {
                transform.position =
                    new Vector3(250,
                        90, Player.transform.position.z - 120);
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        if (cameraEndScreen)
        {
            LerpCameraToGameEnd();
        }
    }

    /// <summary>
    /// Called when camera is switched
    /// </summary>
    void switchCamera()
    {
        if (cameraType == 0)
        {
            cameraType = 1;
        }
        else
        {
            cameraType = 0;
        }
    }

    /// <summary>
    /// Called by event to start the camera movement
    /// </summary>
    public void StartGame()
    {
        gameStarted = true;
    }

    /// <summary>
    /// Lerps the camera from the MainMenuPosition to player follow position
    /// </summary>
    private void LerpCameraToGameStart()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, Player.transform.position.x, Time.deltaTime/1.5f),
                Mathf.Lerp(transform.position.y, Player.transform.position.y + 20, Time.deltaTime/1.5f),
                Mathf.Lerp(transform.position.z, Player.transform.position.z - 80, Time.deltaTime/1.5f));

        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, cameraTilt, Time.deltaTime/1.5f),
            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime/1.5f),
            Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime));

        transform.eulerAngles = currentAngle;

        if (transform.position.x > Player.transform.position.x - 0.5f)
        {
            gameStarted = false;
            cameraLockToPlayer = true;
            GameStateManager.current.LoadAllStartGameComponents();
        }
    }

    private void LerpCameraToGameEnd()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, Player.transform.position.x, Time.deltaTime/1.5f),
                Mathf.Lerp(transform.position.y, Player.transform.position.y + 60, Time.deltaTime/1.5f),
                Mathf.Lerp(transform.position.z, Player.transform.position.z - 80, Time.deltaTime/1.5f));

        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, cameraTilt, Time.deltaTime/1.5f),
            Mathf.LerpAngle(currentAngle.y, 0, Time.deltaTime/1.5f),
            Mathf.LerpAngle(currentAngle.z, 0, Time.deltaTime));

        transform.eulerAngles = currentAngle;
    }

    public void CameraMoveGameOver()
    {
        cameraEndScreen = true;
    }
}

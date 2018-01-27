using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CloudScript : MonoBehaviour {

    private GameObject player;
    Random rnd = new System.Random();
	// Use this for initialization

	void Start () {
        player = GameObject.FindGameObjectWithTag("player");
	}

    private void OnEnable()
    {
        ResumeCloudMovement();
        GameStateManager.pauseGame += StopCloudMovement;
        GameStateManager.resumeGame += ResumeCloudMovement;
    }

    private void OnDisable()
    {
        GameStateManager.pauseGame -= StopCloudMovement;
        GameStateManager.resumeGame -= ResumeCloudMovement;
    }

    // Update is called once per frame
	void Update () {
		if(transform.position.z < player.transform.position.z)
        {
            gameObject.SetActive(false);
        }
	}

    /// <summary>
    /// Stops clouds moving
    /// </summary>
    private void StopCloudMovement()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(rnd.Next(0, 0), 0, 0);
    }

    /// <summary>
    /// ResumesCloudMovement
    /// </summary>
    private void ResumeCloudMovement()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(rnd.Next(-15, -10), 0, 0);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CloudScript : MonoBehaviour, IPausable {

    private GameObject player;
    Random rnd = new System.Random();
	// Use this for initialization

	void Start () {
        player = GameObject.FindGameObjectWithTag("player");
	}

    private void OnEnable()
    {
        Resume();
        GameStateManager.pauseGame += Pause;
        GameStateManager.resumeGame += Resume;
    }

    private void OnDisable()
    {
        GameStateManager.pauseGame -= Pause;
        GameStateManager.resumeGame -= Resume;
    }

    // Update is called once per frame
	void Update () {
		if(transform.position.z < player.transform.position.z)
        {
            gameObject.SetActive(false);
        }
	}

    public void Pause()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(rnd.Next(0, 0), 0, 0);
    }

    public void Resume()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(rnd.Next(-15, -10), 0, 0);
    }
}

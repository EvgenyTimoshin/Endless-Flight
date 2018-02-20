using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonScript : MonoBehaviour {
	
	private GameObject player;
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
		if(transform.position.z < player.transform.position.z - 100)
		{
			var currentAngle = new Vector3 (-90, 0, 0);
			transform.eulerAngles = currentAngle;
			gameObject.SetActive(false);
		}
	}

	public void Pause()
	{
		GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
	}

	public void Resume()
	{
		if (player.transform.position.y > 140) {
			GetComponent<Rigidbody> ().velocity = new Vector3 (0, 30, 0);
		} else {
			GetComponent<Rigidbody> ().velocity = new Vector3 (0, 20, 0);
		}
	}
}

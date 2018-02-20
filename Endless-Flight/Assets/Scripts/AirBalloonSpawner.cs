using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBalloonSpawner : MonoBehaviour,IPausable {
	
	public GameObject player;
	private bool continueSpawning = false;
	private WaitForSeconds waitForSeconds = new WaitForSeconds(10f);

	private void OnEnable()
	{
		GameStateManager.loadGameStartComponents += EnableBalloonSpawning;
		GameStateManager.resumeGame += Resume;
		GameStateManager.pauseGame += Pause;
	}

	private void OnDisable()
	{
		GameStateManager.loadGameStartComponents -= EnableBalloonSpawning;
		GameStateManager.resumeGame -= Resume;
		GameStateManager.pauseGame -= Pause;
	}

	private void EnableBalloonSpawning()
	{
		continueSpawning = true;
		StartCoroutine(SpawnBalloons());
	}

	/// <summary>
	/// Disables all pick ups from spawning
	/// </summary>
	private void DisablePickUpSpawning()
	{
		StopAllCoroutines();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator SpawnBalloons()
	{
		while (continueSpawning)
		{
			SpawnBalloon();

			yield return waitForSeconds;
		}
	}

	private void SpawnBalloon()
	{
		GameObject balloon = GameObjectPool.current.GetPooledBalloon();
		balloon.SetActive (true);

		if (player.transform.position.y > 120) {
			balloon.transform.position = new Vector3 (player.transform.position.x,
				-18, player.transform.position.z + 200);
		} else {
			balloon.transform.position = new Vector3 (player.transform.position.x,
				-18, player.transform.position.z + 150);
		}


	}

	public void Pause()
	{
		continueSpawning = false;
		StopCoroutine(SpawnBalloons());
	}

	public void Resume()
	{
		continueSpawning = true;
		StartCoroutine(SpawnBalloons());
	}


}

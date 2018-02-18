using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellor : MonoBehaviour, IPausable {

    private bool propellorRotating = true;

	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
        GameStateManager.pauseGame += Pause;
        GameStateManager.resumeGame += Resume;
    }

    private void OnDisable()
    {
        GameStateManager.pauseGame -= Pause;
        GameStateManager.resumeGame -= Resume;
    }

    /// <summary>
    /// Called every frame, rotates the propellor
    /// </summary>
    void Update () {
        if (propellorRotating)
        {
            transform.Rotate(Vector3.right * 40);
        }
    }

    public void Pause()
    {
        propellorRotating = false;
    }

    public void Resume()
    {
        propellorRotating = true;
    }
}

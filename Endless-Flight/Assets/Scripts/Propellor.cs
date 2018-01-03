using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellor : MonoBehaviour {

    private bool propellorRotating = true;

	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
        GameStateManager.pauseGame += DisableRotation;
        GameStateManager.resumeGame += EnableRotation;
    }

    private void OnDisable()
    {
        GameStateManager.pauseGame -= DisableRotation;
        GameStateManager.resumeGame -= EnableRotation;
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

    private void DisableRotation()
    {
        propellorRotating = false;
    }

    private void EnableRotation()
    {
        propellorRotating = true;
    }
}

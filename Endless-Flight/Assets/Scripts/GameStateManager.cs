using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    public static GameStateManager current;
    //public Camera mainCamera;


    void Awake()
    {
        current = this;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        MainCamera m = Camera.main.GetComponent<MainCamera>();
        m.StartGame();
    }

    public void LoadAllGameComponents()
    {
        GameObject go = GameObject.Find("transport_plane_green");
        go.GetComponent<PlayerController>().TakeOffAnim();
        go = GameObject.Find("HUD");
        go.GetComponent<Canvas>().enabled = true;
    }
}

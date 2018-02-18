using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ShipScript : MonoBehaviour, IPausable
{
    private Rigidbody rb;
    private Random rnd = new System.Random();

    public GameObject Player;
	// Use this for initialization
	void Start ()
	{
	    rb = GetComponent<Rigidbody>();
	    Player = GameObject.Find("transport_plane_green");
    }
	
	// Update is called once per frame
	void Update () {
        
	    if (transform.position.z < Player.transform.position.z - 200)
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
        GetComponent<Rigidbody>().velocity = new Vector3(rnd.Next(0, 0), 0, 0);
    }
}

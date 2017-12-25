using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    private Rigidbody rb;

    public GameObject Player;
	// Use this for initialization
	void Start ()
	{
	    rb = GetComponent<Rigidbody>();
	    Player = GameObject.Find("transport_plane_green");
    }
	
	// Update is called once per frame
	void Update () {
	    rb.AddRelativeForce(Vector3.forward * 2 * Time.deltaTime * -70);
        
	    if (transform.position.z < Player.transform.position.z - 200)
	    {
            gameObject.SetActive(false);
	    }
        
	}
}

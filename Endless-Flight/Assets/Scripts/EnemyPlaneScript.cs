using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneScript : MonoBehaviour {

    public GameObject player;

    /// <summary>
    /// Initialises current class
    /// </summary>
    void Start()
    {
        //BoxCollider b = GetComponentInParent<BoxCollider>();
        //b.isTrigger = true;
        player = GameObject.Find("transport_plane_green");

    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        if (transform.position.z < player.transform.position.z - 300)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {

    }

    /// <summary>
    /// Called when rigid body enters the current object, deactivates current plane from scene
    /// </summary>
    /// <param name="other"></param>
	void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            gameObject.SetActive(false);
        }
        //PlayerStats p = other.gameObject.GetComponent<PlayerStats>();
    }
}

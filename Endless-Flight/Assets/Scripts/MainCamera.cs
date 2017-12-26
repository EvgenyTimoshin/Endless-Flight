using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public GameObject Player;
    public int View;

    // Use this for initialization
    void Start ()
    {
        View = 0;
    }
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKey(KeyCode.E))
	    {
	        View = ChangeCameraView();
	    }

	    if (View == 0)
	    {
	        transform.position =
	            new Vector3(Mathf.Lerp(transform.position.x, Player.transform.position.x, Time.deltaTime * 5),
	                Player.transform.position.y + 20, Player.transform.position.z - 80);
            transform.eulerAngles = new Vector3(5,0,0);
	    }

	    if (View == 1)
	    {
	        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 10, Player.transform.position.z-30);
	    }

	    if (View == 2)
	    {
	        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 1.9f, Player.transform.position.z + 3.2f);
	        transform.rotation = Player.transform.rotation;

	    }
	}

    private int ChangeCameraView()
    {
        if (View == 0)
        {
            return 1;
        }
        if (View == 1)
        {
            return 2;
        }

        return 0;
    }
}

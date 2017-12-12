using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework.Constraints;
using Random = System.Random;

public class RingSpawn : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
		
		BoxCollider b = GetComponentInParent<BoxCollider>();
		b.isTrigger = true;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	int i = 0;

	void OnTriggerExit(Collider other)
	{
		


	}

	void OnTriggerEnter(Collider other)
	{
		Destroy (gameObject);

		i++;

		Spawn_Rings ();
		Diagnol_Left();
		Diagnol_Right ();


	}

	void Spawn_Rings() {
		
		Random rnd = new Random();
		int choice = rnd.Next(1, 2);
		string ringColour = "Rings/ParticleSystemRed";

		if (choice == 1)
		{
			ringColour = "Rings/ParticleSystemRed";
		}
		if (choice == 2)
		{
			ringColour = "Rings/ParticleSystemBlue";
		}


		Instantiate((Resources.Load(ringColour)), new Vector3(230, 150, transform.position.z + 380), new Quaternion(0, 0, 0, 0));
	}

	void Diagnol_Left() {

		string ringColour = "Rings/ParticleSystemRed";

		Instantiate((Resources.Load(ringColour)), new Vector3(230, 150, transform.position.z + 680), new Quaternion(0, 0, 0, 0));
		Instantiate((Resources.Load (ringColour)), new Vector3 (280, 150, transform.position.z + 730), new Quaternion (0, 0, 0, 0));
		Instantiate((Resources.Load(ringColour)), new Vector3(330, 150, transform.position.z + 780), new Quaternion(0, 0, 0, 0));
		Instantiate((Resources.Load(ringColour)), new Vector3(380, 150, transform.position.z + 830), new Quaternion(0, 0, 0, 0));
	
	}

	void Diagnol_Right() {
		string ringColour = "Rings/ParticleSystemRed";

		Instantiate((Resources.Load(ringColour)), new Vector3(300, 150, transform.position.z + 900), new Quaternion(0, 0, 0, 0));
		Instantiate((Resources.Load (ringColour)), new Vector3 (250, 150, transform.position.z + 950), new Quaternion (0, 0, 0, 0));
		Instantiate((Resources.Load(ringColour)), new Vector3(200, 150, transform.position.z + 1000), new Quaternion(0, 0, 0, 0));
		Instantiate((Resources.Load(ringColour)), new Vector3(150, 150, transform.position.z + 1050), new Quaternion(0, 0, 0, 0));
	}
		
}

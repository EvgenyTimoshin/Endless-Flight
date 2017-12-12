using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RingSpawner : MonoBehaviour
{

    private string ringColour;
    private Random rnd = new Random();
    public float frontOffset = 250;

    // Use this for initialization
    void Start () {
		InvokeRepeating("Spawn_Rings", 1f, 5.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    frontOffset += 1f;
	}

    void Spawn_Rings()
    { 
        int choice = rnd.Next(0,1);

        if (choice == 0)
        {
            ringColour = "Rings/ParticleSystemRed";
        }
        if (choice == 1)
        {
            ringColour = "Rings/ParticleSystemBlue";
        }

        choice = rnd.Next(0, 3);

        if (choice == 0)
        {
            Single();
        }
        if (choice == 1)
        {
            Row();
        }
        if (choice == 2)
        {
            Column();
        }
        if (choice == 3)
        {
            if (transform.parent.parent.position.x > 200 && transform.parent.parent.position.x < 300)
            { 
                choice = rnd.Next(0,1);
                if (choice == 0)
                {
                    Diagnol_Right();
                }
                else
                {
                    Diagnol_Left();
                }

                return;
            }
            if (transform.parent.parent.position.x > 330)
            {
                Diagnol_Left();
                
            }

            if (transform.parent.parent.position.x < 170)
            {
                Diagnol_Right();
            }
        }
    }

    void Single()
    {
        Instantiate(Instantiate(Resources.Load(ringColour), new Vector3(rnd.Next(170,330), 150, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
    }

    void Row()
    {
        int x = 170;
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x + 40, 150, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x + 80, 150, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x + 120, 150, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, 160, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
    }

    void Column()
    {
        int x = rnd.Next(170, 330);
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, 150, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, 150, transform.position.z + frontOffset + 40), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, 150, transform.position.z + frontOffset + 80), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, 150, transform.position.z + frontOffset + 120), new Quaternion(0, 0, 0, 0)));
    }

    void Diagnol_Left()
    {

        
        int x = rnd.Next(240,335);
        Instantiate((Resources.Load(ringColour)), new Vector3(x - 20, 150, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x - 40, 150, transform.position.z + frontOffset + 40), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x - 60, 150, transform.position.z + frontOffset + 80), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x - 80, 150, transform.position.z + frontOffset + 120), new Quaternion(0, 0, 0, 0));

    }

    void Diagnol_Right()
    {
        int x = rnd.Next(170, 260);
        Instantiate((Resources.Load(ringColour)), new Vector3(x + 20, 150, transform.position.z + frontOffset ), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x + 40, 150, transform.position.z + frontOffset + 40), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x + 60, 150, transform.position.z + frontOffset + 80), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x + 80, 150, transform.position.z + frontOffset + 120), new Quaternion(0, 0, 0, 0));
    }
}

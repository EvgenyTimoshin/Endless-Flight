using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RingSpawner : MonoBehaviour
{
    
    private string ringColour;
    private Random rnd = new Random();
    public float frontOffset = 250;
    private float RingPosX = 95;

    // Use this for initialization
    void Start () {
		InvokeRepeating("Spawn_Rings", 2f,4.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    frontOffset += 1f;
	}

    /// <summary>
    /// 
    /// </summary>
    void Spawn_Rings()
    { 
        int choice = rnd.Next(0,2);

        if (choice == 0)
        {
            ringColour = "ParticleSystemRed";
        }
        if (choice == 1)
        {
            ringColour = "ParticleSystemBlue";
        }

        choice = rnd.Next(0, 4);

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
            if (transform.parent.position.x > 200 && transform.parent.position.x < 300)
            { 
                choice = rnd.Next(0,2);
                if (choice == 0)
                {
                    Diagnol_Right();
                    return;
                }
                else
                {
                    Diagnol_Left();
                    return;
                }

                
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

    /// <summary>
    /// 
    /// </summary>
    void Single()
    {
        GameObject ring = GameObjectPool.current.GetPooledRing(ringColour + "(Clone)");
        Debug.Log("Extracted : " + ring.name);
        ring.transform.position = new Vector3(rnd.Next(170, 330), RingPosX, transform.position.z + frontOffset);
        ring.SetActive(true);
        //Instantiate(Instantiate(Resources.Load(ringColour), new Vector3(rnd.Next(170,330), RingPosX, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
    }

    /// <summary>
    /// 
    /// </summary>
    void Row()
    {
        Debug.Log("ROW SPAWNED");

        GameObject[] rings = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject ring = GameObjectPool.current.GetPooledRing(ringColour + "(Clone)");
            
            rings[i] = ring;
        }

        

        int x = rnd.Next(170,230);

        rings[0].transform.position = new Vector3(x + 40, RingPosX, transform.position.z + frontOffset);
        rings[1].transform.position = new Vector3(x + 80, RingPosX, transform.position.z + frontOffset);
        rings[2].transform.position = new Vector3(x + 120, RingPosX, transform.position.z + frontOffset);
        rings[3].transform.position = new Vector3(x, 160, transform.position.z + frontOffset);

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(rings[i]);
            
        }

        /*
        int x = 170;
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x + 40, RingPosX, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x + 80, RingPosX, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x + 120, RingPosX, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, 160, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        */
    }

    /// <summary>
    /// 
    /// </summary>
    void Column()
    {
        GameObject[] rings = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject ring = GameObjectPool.current.GetPooledRing(ringColour + "(Clone)");
            ring.SetActive(true);
            rings[i] = ring;
        }

        int x = rnd.Next(170, 330);

        rings[0].transform.position = new Vector3(x, RingPosX, transform.position.z + frontOffset);
        rings[1].transform.position = new Vector3(x, RingPosX, transform.position.z + frontOffset + 40);
        rings[2].transform.position = new Vector3(x, RingPosX, transform.position.z + frontOffset + 80);
        rings[3].transform.position = new Vector3(x, RingPosX, transform.position.z + frontOffset + 120);       

        for (int i = 0; i < rings.Length; i++)
        {
            rings[i].SetActive(true);
        }

        /*
        int x = rnd.Next(170, 330);
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, RingPosX, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, RingPosX, transform.position.z + frontOffset + 40), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, RingPosX, transform.position.z + frontOffset + 80), new Quaternion(0, 0, 0, 0)));
        Instantiate(Instantiate((Resources.Load(ringColour)), new Vector3(x, RingPosX, transform.position.z + frontOffset + 120), new Quaternion(0, 0, 0, 0)));
        */
    }

    /// <summary>
    /// 
    /// </summary>
    void Diagnol_Left()
    {
        GameObject[] rings = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject ring = GameObjectPool.current.GetPooledRing(ringColour + "(Clone)");
            ring.SetActive(true);
            rings[i] = ring;
        }

        int x = rnd.Next(240, 335);

        rings[0].transform.position = new Vector3(x - 20, RingPosX, transform.position.z + frontOffset);
        rings[1].transform.position = new Vector3(x - 40, RingPosX, transform.position.z + frontOffset + 40);
        rings[2].transform.position = new Vector3(x - 60, RingPosX, transform.position.z + frontOffset + 80);
        rings[3].transform.position = new Vector3(x - 80, RingPosX, transform.position.z + frontOffset + 120);

        for (int i = 0; i < rings.Length; i++)
        {
            rings[i].SetActive(true);
        }

        /*
        
        int x = rnd.Next(240,335);
        Instantiate((Resources.Load(ringColour)), new Vector3(x - 20, RingPosX, transform.position.z + frontOffset), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x - 40, RingPosX, transform.position.z + frontOffset + 40), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x - 60, RingPosX, transform.position.z + frontOffset + 80), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x - 80, RingPosX, transform.position.z + frontOffset + 120), new Quaternion(0, 0, 0, 0));
        */

    }

    void Diagnol_Right()
    {
        GameObject[] rings = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject ring = GameObjectPool.current.GetPooledRing(ringColour + "(Clone)");
            ring.SetActive(true);
            rings[i] = ring;
        }

        int x = rnd.Next(170, 260);

        rings[0].transform.position = new Vector3(x + 20, RingPosX, transform.position.z + frontOffset);
        rings[1].transform.position = new Vector3(x + 40, RingPosX, transform.position.z + frontOffset + 40);
        rings[2].transform.position = new Vector3(x + 60, RingPosX, transform.position.z + frontOffset + 80);
        rings[3].transform.position = new Vector3(x + 80, RingPosX, transform.position.z + frontOffset + 120);

        for (int i = 0; i < rings.Length; i++)
        {
            rings[i].SetActive(true);
        }

        /*
        
        Instantiate((Resources.Load(ringColour)), new Vector3(x + 20, RingPosX, transform.position.z + frontOffset ), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x + 40, RingPosX, transform.position.z + frontOffset + 40), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x + 60, RingPosX, transform.position.z + frontOffset + 80), new Quaternion(0, 0, 0, 0));
        Instantiate((Resources.Load(ringColour)), new Vector3(x + 80, RingPosX, transform.position.z + frontOffset + 120), new Quaternion(0, 0, 0, 0));
        */
    }
}

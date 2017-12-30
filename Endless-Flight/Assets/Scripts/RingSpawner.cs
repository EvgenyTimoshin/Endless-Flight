using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RingSpawner : MonoBehaviour
{
    
    private string ringColour;
    private Random rnd = new Random();
    public float frontOffset = 250;
    private float RingPosY = 95;

    /// <summary>
    /// Initialises current class
    /// </summary>
    void Start () {
		InvokeRepeating("Spawn_Rings", 10f,2.0f);
	}
	
	/// <summary>
    /// Called once per frame
    /// </summary>
	void Update ()
	{
	    frontOffset += 1f;
	}

    /// <summary>
    /// Selects a random ring to be spawned
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
        ring.transform.position = new Vector3(rnd.Next(170, 330), RingPosY, transform.position.z + frontOffset);
        ring.SetActive(true);
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

        rings[0].transform.position = new Vector3(x + 40, RingPosY, transform.position.z + frontOffset);
        rings[1].transform.position = new Vector3(x + 80, RingPosY, transform.position.z + frontOffset);
        rings[2].transform.position = new Vector3(x + 120, RingPosY, transform.position.z + frontOffset);
        rings[3].transform.position = new Vector3(x,RingPosY,transform.position.z + frontOffset);

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(rings[i]);
            
        }

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

        rings[0].transform.position = new Vector3(x, RingPosY, transform.position.z + frontOffset);
        rings[1].transform.position = new Vector3(x, RingPosY, transform.position.z + frontOffset + 60);
        rings[2].transform.position = new Vector3(x, RingPosY, transform.position.z + frontOffset + 120);
        rings[3].transform.position = new Vector3(x, RingPosY, transform.position.z + frontOffset + 180);       

        for (int i = 0; i < rings.Length; i++)
        {
            rings[i].SetActive(true);
        }

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

        rings[0].transform.position = new Vector3(x - 20, RingPosY, transform.position.z + frontOffset);
        rings[1].transform.position = new Vector3(x - 40, RingPosY, transform.position.z + frontOffset + 60);
        rings[2].transform.position = new Vector3(x - 60, RingPosY, transform.position.z + frontOffset + 120);
        rings[3].transform.position = new Vector3(x - 80, RingPosY, transform.position.z + frontOffset + 180);

        for (int i = 0; i < rings.Length; i++)
        {
            rings[i].SetActive(true);
        }

    }

    /// <summary>
    /// 
    /// </summary>
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

        rings[0].transform.position = new Vector3(x + 20, RingPosY, transform.position.z + frontOffset);
        rings[1].transform.position = new Vector3(x + 40, RingPosY, transform.position.z + frontOffset + 60);
        rings[2].transform.position = new Vector3(x + 60, RingPosY, transform.position.z + frontOffset + 120);
        rings[3].transform.position = new Vector3(x + 80, RingPosY, transform.position.z + frontOffset + 180);

        for (int i = 0; i < rings.Length; i++)
        {
            rings[i].SetActive(true);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameObjectPool : MonoBehaviour
{
    public static GameObjectPool current;
    List<GameObject> pool;

    void Awake()
    {
        current = this;
    }

    // Use this for initialization
	void Start ()
	{
        pool = new List<GameObject>();
        
	    Object[] list = Resources.LoadAll("Islands");

	    for (int i = 0; i < list.Length; i++)
	    {
	        GameObject obj = (GameObject)list[i];
            obj.SetActive(false);
	        pool.Add(Instantiate(obj));
	        Debug.Log(list[i].name);
	    }
	    for (int i = 0; i < list.Length; i++)
	    {
	        GameObject obj = (GameObject)list[i];
	        obj.SetActive(false);
	        pool.Add(Instantiate(obj));
	        Debug.Log(list[i].name);
	    }


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject GetPooledGameObject(string desiredObj)
    {
        foreach (var ob in pool)
        {
            //Debug.Log("Object in pool loop" + ob.name);
            if (desiredObj == ob.name && !ob.activeInHierarchy)
            {
                return ob;
            }
        }

        return null;
    }
}

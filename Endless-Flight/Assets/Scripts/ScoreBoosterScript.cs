using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoosterScript : MonoBehaviour, IPickUp {

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 0.7f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
           PickUpInteraction(other);
        }
    }

    public void PickUpInteraction(Collider other)
    {
        other.GetComponent<PlayerStats>().increaseScoreBy(50);
        PickUpDestroy();
    }

    public void PickUpDestroy()
    {
        gameObject.SetActive(false);
    }
}

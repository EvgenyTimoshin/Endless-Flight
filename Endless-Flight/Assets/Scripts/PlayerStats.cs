using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public Text scoreText;
    public Slider healthBar;
    private int score = 0;
    private int health = 100;
    private bool scoreEnabled = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (scoreEnabled)
        {
            increaseScoreBy(1);
        }

        scoreText.text = "Score " + score / 20;
        healthBar.value = health;
    }

    public void increaseScoreBy(int plusScore)
    {
        score += plusScore;
    }

    public void modifyHealthBy(int healthModifyBy)
    {
        health += healthModifyBy;
    }

    public void flipScoreState()
    {
        if(scoreEnabled)
        {
            scoreEnabled = false;
            return;
        }
        else
        {
            scoreEnabled = true;
            return;
        }
    }
}

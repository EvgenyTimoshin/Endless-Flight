using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public Text scoreText;
    public Slider fuelBar;
    private int score = 0;
    public float Fuel = 100;
    private bool scoreEnabled = false;
    private float timer = 0;
    private float timeout = 0.2f;

    private bool scoreIsMultiplied = false;
    private int scoreMultiplier = 1;
    private float scoreMultiplierTimer = 0;
    private float scoreMultiplierTimeout = 0;

    private bool playerAlive = true;

    // Use this for initialization
    void Start() {

    }

    private void OnEnable()
    {
        GameStateManager.pauseGame += DisableScoreCount;
        GameStateManager.resumeGame += EnableScoreCount;
    }

    private void OnDisable()
    {
        GameStateManager.pauseGame -= DisableScoreCount;
        GameStateManager.resumeGame -= EnableScoreCount;
    }

    // Update is called once per frame
    void Update() {

        if (scoreEnabled)
        {
            if(scoreIsMultiplied)
            {
                scoreMultiplierTimer += Time.deltaTime;
            }

            timer += Time.deltaTime;
            if (timer > timeout)
            {
                timer -= timeout;
                increaseScoreBy(1);
            }
                modifyFuelBy(-5f * Time.deltaTime);
        }

        if (fuelBar != null)
        {
            fuelBar.value = Fuel;
        }

        CheckFuel();
        CheckPlayerAlive();
        CheckPlayerCrashed();
    }

    /// <summary>
    /// Increases score by specified amount
    /// </summary>
    /// <param name="plusScore"></param>
    public void increaseScoreBy(int plusScore)
    {
        if (scoreIsMultiplied)
        {
            if (scoreMultiplierTimer > scoreMultiplierTimeout)
            {
                Debug.Log("ScoreMultiplierStoped");
                scoreMultiplier = 1;
                scoreMultiplierTimeout = 0;
                scoreMultiplierTimer = 0;
                scoreIsMultiplied = false;
            }
        }
        score += plusScore * scoreMultiplier;
        scoreText.text = "S c o r e " + score;
    }

    /// <summary>
    /// Modifies players fuel by specified amount
    /// </summary>
    /// <param name="fuelModifyBy"></param>
    public void modifyFuelBy(float fuelModifyBy)
    {
        if(Fuel + fuelModifyBy > 100)
        {
            Fuel = 100;
        }
        else
        {
            Fuel += fuelModifyBy;
        }
    }

    /// <summary>
    /// Enables the score count
    /// </summary>
    private void EnableScoreCount()
    {
        scoreEnabled = true;
    }

    /// <summary>
    /// Disables the score count
    /// </summary>
    private void DisableScoreCount()
    {
        scoreEnabled = false;
    }

    public void SetScoreMultiplier(int multiplier, int multipliertime)
    {
        scoreMultiplier = multiplier;
        scoreMultiplierTimeout = multipliertime;
        StartScoreMultiplier();
    }

    private void StartScoreMultiplier()
    {
        scoreIsMultiplied = true;
    }

    private void CheckFuel()
    {
        if (Fuel < 0)
        {
            playerAlive = false;
        }
    }

    private void CheckPlayerAlive()
    {
        if (!playerAlive)
        {
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().mass = 2;
            GetComponent<PlayerController>().enabled = false;

            GetComponent<Rigidbody>().transform.Rotate(0.2f,0.2f,0);
        }
    }


    private void CheckPlayerCrashed()
    {
        if (transform.position.y <= 5.5f)
        {
            GameStateManager.current.GameOver();
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public int GetScore()
    {
        return score;
    }
}

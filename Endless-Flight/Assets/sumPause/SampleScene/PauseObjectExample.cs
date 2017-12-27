using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PauseObjectExample : MonoBehaviour {

    SpriteRenderer image;

    void Awake () {
        image = GetComponent<SpriteRenderer>();
    }

    // Add/Remove the event listeners
    void OnEnable () {
        SumPause.pauseEvent += OnPause;
    }

    void OnDisable () {
        SumPause.pauseEvent -= OnPause;
    }

    /// <summary>What to do when the pause button is pressed.</summary>
    /// <param name="paused">New pause state</param>
    void OnPause (bool paused) {
        if(paused) {
            // This is what we want do when the game is paused
            image.color = Color.blue;
        }
        else {
            // This is what we want to do when the game is resumed
            image.color = Color.white;
        }
    }

}

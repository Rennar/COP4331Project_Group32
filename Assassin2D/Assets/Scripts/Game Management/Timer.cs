using UnityEngine;
using UnityEngine.UI;

// Timothy Garrett

// Simple timer for the level
public class Timer : BaseClass {
    // Variables for keeping track of our Timer.
    public static int intTime;
    Text timerLabel;
    public float time;

    private void Awake()
    {
        // Creates the Timer as soon as the game is started.
        timerLabel = GetComponent<Text>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	protected override void CustomUpdate () {
        //EventManager.TriggerPause();

        // We minus time every update by deltaTime and convert it from a float to an int so that
        // the player will not have to look at a bunch of numbers changing every time the game
        // is updated and instead simulate how real time is counted down. 
        time -= Time.deltaTime;
		intTime = (int)time;
        // So long as time is greater than zero we display the Timer.
        if (time > 0)
            timerLabel.text = "Time: " + (int)time;
        // If it is not greater than zero we show that Time is zero and trigger the GameOver screen.
        else
        {
            timerLabel.text = "Time: 0";
            EventManager.TriggerOnLose();
        }
    }
}

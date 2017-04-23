using UnityEngine;
using UnityEngine.UI;

// Timothy Garrett

public class Score : MonoBehaviour
{   
    // Variables for holding how many points we have and storing them.
    public static int scorePoints;
    Text text;

    private void Awake()
    {
        // Sets the score to zero and creates the visual of it on the start up.
        text = GetComponent<Text>();
        scorePoints = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Constantly updates the score text everytime the game is updated.
        text.text = "Score: " + scorePoints;
    }
}
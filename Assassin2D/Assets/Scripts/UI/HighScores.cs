using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// James Williamson

// simple struct that contains a username and its related high score 
struct highscore{
	public string username;
	public string score;

	public highscore(string _username, string _score){
		username = _username;
		score = _score;
	}
}

// manages the high scores page
public class HighScores : MonoBehaviour {

	highscore[] l1Highscores;
	public Text[] l1Textboxes;

	highscore[] l2Highscores;
	public Text[] l2Textboxes;

	public bool l1;
	int numUsers1;
	int numUsers2;

	void Awake(){
		l1 = true;
		l1Highscores = new highscore[10];
		l2Highscores = new highscore[10];
		numUsers1 = 0;
		numUsers2 = 0;
	}

	// Changes the text boxes on the highscore page
	void UpdateHighScores(){
		if (l1) {
			for (int i = 0; i < l1Textboxes.Length; i++) {
				if (i <= numUsers1 && l1Highscores[i].score != "0") {
					l1Textboxes [i].text = (i+1) + ". " + l1Highscores [i].username + ": " + l1Highscores [i].score;
				} else {
					l1Textboxes [i].text = "";
				}
			}
		} else {
			for (int i = 0; i < l2Textboxes.Length; i++) {
				if (i <= numUsers2 && l2Highscores[i].score != "0") {
					l2Textboxes [i].text = (i+1) + ". " + l2Highscores [i].username + ": " + l2Highscores [i].score;
				} else {
					l2Textboxes [i].text = "";
				}
			}
		}
	}

	public void OnEnable(){
		ScoresOne ();
		ScoresTwo ();
		UpdateHighScores();
	}

	// changes the scores form level one to the scores form level two
	public void ToggleLevelScore(){
		l1 = !l1;
		UpdateHighScores();
	}

	// splits up a string passed in as the top ten scores formatted as alternating scores and usernames
	// separated by a space. 
	void ScoresOne(){
		char[] delimiterchars = { '\n', ' '};
		string[] scoreVals = GameManager.gm.sbL1.Split(delimiterchars);
		for (int i = 1; i < scoreVals.Length; i += 2) {
			//Debug.Log ("db1: " + scoreVals [i] + " " + scoreVals [i + 1]);
			if ((i/2) < l1Highscores.Length) {
				numUsers1++;
				l1Highscores [i / 2] = new highscore(scoreVals[i+1], scoreVals [i]);
			}
		}
	}

	// splits up a string passed in as the top ten scores formatted as alternating scores and usernames
	// separated by a space. 
	void ScoresTwo(){
		char[] delimiterchars = { '\n', ' ' };
		string[] scoreVals  = GameManager.gm.sbL2.Split(delimiterchars);
		for (int i = 1; i < scoreVals.Length; i += 2) {
			//Debug.Log ("db2: " + scoreVals [i] + " " + scoreVals [i + 1]);
			if ((i/2) < l2Highscores.Length) {
				numUsers2++;
				l2Highscores [i / 2] = new highscore(scoreVals[i+1], scoreVals [i]);
			}
		}
	}



}

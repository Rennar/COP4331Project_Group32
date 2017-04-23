using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// James Williamson

// functions for pause, quit, scene transitions, etc

public class Buttons : MonoBehaviour {

	public void LoadLevelOne(){
		GameManager.gm.inGame = true;
		GameManager.gm.currentLevel = 1;
		GameManager.gm.OnSceneChange ();
		SceneManager.LoadScene (1);
	}

	public void LoadMainMenu(){
		GameManager.gm.inGame = false;
		GameManager.gm.currentLevel = 0;
		GameManager.gm.OnSceneChange ();
		SceneManager.LoadScene (0);
	}

	public void LoadMultiplayer(){
		GameManager.gm.inGame = true;
		GameManager.gm.currentLevel = 3;
		GameManager.gm.OnSceneChange ();
		SceneManager.LoadScene (3);

	}

	public void LoadLevelTwo(){
		GameManager.gm.inGame = true;
		GameManager.gm.currentLevel = 2;
		GameManager.gm.OnSceneChange ();
		SceneManager.LoadScene (2);

	}

	public void Resume(){
		if (GameManager.gm.isPaused) {
			GameManager.gm.TogglePause ();
		}
	}

	public void Quit(){
		Application.Quit ();
	}
}

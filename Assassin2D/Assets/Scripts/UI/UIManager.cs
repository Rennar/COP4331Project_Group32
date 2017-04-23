using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// contains functions to be used by ui buttons for opening or closing ui windows
public class UIManager : MonoBehaviour {

	public GameObject currentScreen;
	public GameObject pauseScreen;
	public GameObject gameOver;
	public GameObject shop;

	// open a ui screen
	public void OpenScreen(GameObject screen){
		screen.SetActive (true);
		currentScreen = screen;
	}

	// close a ui screen
	public void DisableScreen(){
		if (currentScreen != null) {
			currentScreen.SetActive (false);
		}
	}

	// open the pause screen
	public void PauseScreen(){

		if (GameManager.gm.inGame) {
			DisableScreen ();
			pauseScreen.SetActive (true);
			currentScreen = pauseScreen;
		}
	}

	// open the shop screen
	public void ShopScreen(){
		if (!GameManager.gm.isPaused) {
			GameManager.gm.TogglePause ();
		}

		shop.SetActive (true);
	}

	// open the game over screen
	public void GameOverScreen(){
		if (!GameManager.gm.isPaused) {
			GameManager.gm.TogglePause ();
		}

		gameOver.SetActive (true);
	}

	// close the pasue screen
	public void UnPause(){
		pauseScreen.SetActive (false);
	}

	// add functions to the delegates in the eventmanager
	void OnEnable(){
		EventManager.Pause += PauseScreen;
		EventManager.Resume += UnPause;
		EventManager.OnWin += ShopScreen;
		EventManager.OnLose += GameOverScreen;
	}

	// remove when unaccessale to avoid errors
	void OnDisable(){
		EventManager.Pause -= PauseScreen;
		EventManager.Resume -= UnPause;
		EventManager.OnWin -= ShopScreen;
		EventManager.OnLose -= GameOverScreen;
	}

}

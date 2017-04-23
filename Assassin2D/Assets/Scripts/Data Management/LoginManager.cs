using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// James Williamson

// Script for logging in or registering
public class LoginManager : MonoBehaviour {
	public GameObject loginScreen;
	public GameObject createAccountScreen;
	public GameObject mainMenu;

	public GameObject invalidLogin;
	public GameObject pwDontMatch;
	public GameObject invalidRegister;

	public InputField lUsername;
	public InputField pw1;

	public InputField rUsername;
	public InputField rpw;
	public InputField rpw2;

	bool isLoggingIn;

	public void Awake(){

	}

	public void Start(){

		if (GameManager.gm.isLoggedIn) {
			loginScreen.SetActive (false);
			mainMenu.SetActive (true);
		} else {
			loginScreen.SetActive (true);
			mainMenu.SetActive (false);
		}
		isLoggingIn = false;
	}



	// this confirms that passwords match, then attempts to register the user
	public void CreateAccountQuery(){
		Debug.Log ("Button pressed");
		if (!isLoggingIn) {
			if (rpw.text == rpw2.text) {
				StartCoroutine(CoCreateAccount (rUsername.text, rpw.text));
			} else {
				pwDontMatch.SetActive (true);

			}
		} else {
			Debug.Log ("Still logging in");
		}
	}

	// adds username and password to database
	private void CreateAccount(string username, string pw){
			Debug.Log ("Account created");
			GameManager.gm.SetPW (pw);
			ConfirmLogin (username);
	}

	// Checks username and password against database
	public void LoginQuery(){
		if (!isLoggingIn) {
			StartCoroutine (CoSignIn (lUsername.text, pw1.text));
		} else {
			Debug.Log ("Still logging in");
		}
	}

	// this will pass on the username to the gamemanager, which will get the save data
	private void ConfirmLogin(string username){
		Debug.Log ("Logged in");
		// For now, we'll just pass the username to the game manager
		GameManager.gm.setPlayerName(username);

		GameManager.gm.isLoggedIn = true;

		GameManager.gm.OnLogin ();

		GoToMainMenu ();
	}

	public void GoToRegister(){
		loginScreen.SetActive (false);

		createAccountScreen.SetActive(true);
	}

	public void GoToLogin(){
		createAccountScreen.SetActive(false);

		loginScreen.SetActive (true);
	}

	public void GoToMainMenu(){
		loginScreen.SetActive (false);

		createAccountScreen.SetActive(false);

		mainMenu.SetActive(true);
	}


	// wait until the account is registered to make sure the registration is valid, then login
	IEnumerator CoCreateAccount(string username, string pw){
		Debug.Log ("starting check");
		isLoggingIn = true;
		PhPUnity db = GameManager.gm.CreateCall ();
		db.Register (username, pw);
		float timer = 0;
		while ((bool)db.getWaiting() && timer < 5) {
			Debug.Log ("still waiting");
			timer += Time.deltaTime;
			yield return null;
		}
		if (timer >= 5) {
			Destroy (db.gameObject);
			yield break;
		}
		if ((bool)db.getReturnValue()) {
			Debug.Log ("Account created");
			GameManager.gm.SetPW (pw);
			ConfirmLogin (username);
		} else {
			invalidRegister.SetActive(true);
		}
		isLoggingIn = false;
		Destroy (db.gameObject);
		Debug.Log ("end check");
		yield break;
	}

	// wait until the database confirms the login to start the game
	IEnumerator CoSignIn(string username, string pw){
		Debug.Log ("starting check");
		isLoggingIn = true;
		PhPUnity db = GameManager.gm.CreateCall ();
		db.Login (username, pw);
		float timer = 0;
		while ((bool)db.getWaiting() && timer < 5) {
			Debug.Log ("still waiting");
			timer += Time.deltaTime;
			yield return null;
		}

		if ((bool)db.getReturnValue()) {
			Debug.Log ("Account created");
			GameManager.gm.SetPW (pw);
			ConfirmLogin (username);
		} else {
			invalidLogin.SetActive (true);
		}
		isLoggingIn = false;
		Debug.Log ("end check");
		Destroy (db.gameObject);
		yield break;
	}
}

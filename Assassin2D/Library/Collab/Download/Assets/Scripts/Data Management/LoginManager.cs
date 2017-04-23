using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {
	public GameObject loginScreen;
	public GameObject createAccountScreen;
	public GameObject mainMenu;

	public InputField username;
	public InputField pw1;

	public InputField rUsername;
	public InputField rpw;
	public InputField rpw2;

	bool isLoggingIn;

	PhPUnity loginstuff;

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
		loginstuff = (PhPUnity) GameManager.gm.gameObject.GetComponent<PhPUnity>();
	}



	// this will confirm passwords match and check for clashes with any pre-existing usernames.
	public void CreateAccountQuery(){
		Debug.Log ("Button pressed");
		if (!isLoggingIn) {
			if (rpw.text == rpw2.text) {
				StartCoroutine(CoCreateAccount (rUsername.text, rpw.text));
			} else {
				Debug.Log ("passwords don't match");
			}
		} else {
			Debug.Log ("still logging in");
		}
	}

	// adds username and password to database
	private void CreateAccount(string username, string pw){
		if ((bool)loginstuff.Register (username, pw)) {
			Debug.Log ("Account created");
			GameManager.gm.SetPW (pw);
			ConfirmLogin (username);
		} else {
			Debug.Log ("Dude, where's my account!?");
		}


	}

	// Checks username and password against database
	public void LoginQuery(){
		//Do some checking
		if ((bool)loginstuff.Login (username.text, pw1.text)) {
			Debug.Log ("Fuck Yeah");
			GameManager.gm.SetPW (pw1.text);
			ConfirmLogin (username.text);
		} else {
			Debug.Log ("Fuck");
		}

			

		// otherwise send error message.
	}

	// this will pass on the username to the gamemanager, which will get the save data
	private void ConfirmLogin(string username){
		Debug.Log ("Logged in");
		// For now, we'll just pass the username to the game manager
		GameManager.gm.playerName = username;

		GameManager.gm.isLoggedIn = true;

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

	IEnumerator CoCreateAccount(string username, string pw){
		Debug.Log ("starting check");
		isLoggingIn = true;
		loginstuff.Register (username, pw);
		while ((bool)loginstuff.getWaiting()) {
			Debug.Log ("still waiting");
			yield return null;
		}
		if ((bool)loginstuff.getReturnValue()) {
			Debug.Log ("Account created");
			GameManager.gm.SetPW (pw);
			ConfirmLogin (username);
		} else {
			Debug.Log ("Dude, where's my account!?");
		}
		isLoggingIn = false;
		Debug.Log ("end check");
		yield break;
	}
}

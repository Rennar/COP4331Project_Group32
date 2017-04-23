using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

// James Williamson

// Literally just changes a textbox's text to username.
// ...didn't want to make a reference to any specific text boxes in the gamemanager.

public class UsernameScript : NetworkBehaviour {

	Text username;

	void Start(){
		username = GetComponent<Text> ();
		if (GameManager.gm.getPlayerName() != null) {
			username.text = GameManager.gm.getPlayerName();
		} else {
			username.text = "";
		}
	}
		
	void Update () {
		if (GameManager.gm.getPlayerName() != null) {
			username.text = GameManager.gm.getPlayerName();
		} else {
			username.text = "";
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// for now, this will contain the functions for most of the 
// scene transistions. You cna put it on any button that will transition.

public class Buttons : NetworkBehaviour {

	public void LoadLevelOne(){
		SceneManager.LoadScene (1);
	}

	public void LoadMainMenu(){
		SceneManager.LoadScene (0);
	}

}

using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	public string playerName;
	public bool isLoggedIn;

	int currentLevel;
	float[] levelTimes;

    public static GameManager gm;

	struct achievement{
		string name;
		float progress;
		bool achieved;

		public achievement(string _name, float _progress){
			name = _name;
			progress = _progress;
			achieved = false;
		}
	}

	Dictionary<string,achievement> achievements = new Dictionary<string,achievement>();

	// Use this for initialization
	void Awake () {
        if (gm == null)
        {
            DontDestroyOnLoad(gameObject);
			gm = this;
        } else if(gm != this)
        {
            Destroy(gameObject);
        }

		// this is where we'll fill in all of the achievements. we'll have to load it up
		// from the save file before this once we have that set up.
		if(achievements == null){

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Achievements
	// High scores

}

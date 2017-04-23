using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// James Williamson

// Manages the general operations of the game as well as calls to the database
struct dataUpdate{
	public PhPUnity database;
	public int value;
	public string type;

	public dataUpdate(PhPUnity _database, int _value, string _type){
		database = _database;
		value = _value;
		type = _type;
	}
}

public struct Achievement{
	public string name;
	public bool achieved;

	public Achievement(string _name){
		name = _name;
		achieved = false;
	}
}

public class GameManager : MonoBehaviour {
	public bool isLoggedIn;
	public bool isPaused;
	public bool isMultiplayer;
	public bool inGame;
	public int gunUpgrade;
	public int playerupgrade;
	public int totalPoints;
	private int achievementVal;
	private int shopVal;
	public string sbL1;
	public string sbL2;


	public float upgradeCost;
	public int maxLevel;

	public int currentLevel;
	float[] levelTimes;

	private int l1HS;
	private int l2HS;
	public int exp;

	private string playerName;
	private string pw;
	//private PhPUnity dataManager;

	private bool processingData;
	public List<GameObject> enemies;
	private Queue<dataUpdate> databaseCalls;

    public static GameManager gm;

	public Achievement[] achievements;

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

		processingData = false;
		enemies = new List<GameObject> ();

		currentLevel = SceneManager.GetActiveScene ().buildIndex;

		//dataManager = (PhPUnity)GetComponent<PhPUnity> ();
		databaseCalls = new Queue<dataUpdate>();

		// this is where we'll fill in all of the achievements. we'll have to load it up
		// from the save file before this once we have that set up.
		if(achievements == null){
			achievements =  new Achievement[]{
				new Achievement("Beat Level One"),
				new Achievement("Beat Level Two"),
				new Achievement("Upgraded Gun"),
				new Achievement("Upgraded Player"),
				new Achievement("Played Multiplayer"),
				new Achievement("Bested Your Opponent"),
				new Achievement("Killed 'em All")

			};
		}
		maxLevel = 5;

		upgradeCost = 30;

		gunUpgrade = 0;
		playerupgrade = 0;

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Pause") && !isMultiplayer && inGame){
			Debug.Log (isPaused);
			TogglePause();
		}

		if (currentLevel == 1 || currentLevel == 2 || currentLevel == 3) {
			inGame = true;
		} else {
			inGame = false;
		}

		// dequeues the database call objects to be checked and deleted
		if (databaseCalls != null) {
			if (databaseCalls.Count > 0 && !processingData) {
				StartCoroutine (DataUpdateCheck (databaseCalls.Dequeue ()));
			}
		}
	}

	public void TogglePause(){
		if (isPaused) {
			EventManager.TriggerResume ();
			isPaused = false;
		} else {
			EventManager.TriggerPause ();
			isPaused = true;
		}
	}

	// confirm achievments for beating the level
	public void BeatLevel(){
		if (SceneManager.GetActiveScene ().buildIndex == 1 && !achievements[0].achieved) {
			achievements [0].achieved = true;
			// achievements are using bitwise operators since all seven achievments are stored in the same
			// int. So, each bit needs to be set.
			SetData ("achievement", 1 | achievementVal);
			achievementVal |= 1;
		} else if (SceneManager.GetActiveScene ().buildIndex == 2 && !achievements[1].achieved) {
			achievements [1].achieved = true;
			SetData ("achievement", 2 | achievementVal);
			achievementVal |= 2;
		}
	}

	// confirm achievement for the full level kill
	public void Wipeout(){
		if (enemies.Count == 0 && !achievements[6].achieved) {
			achievements [6].achieved = true;
			SetData ("achievement", (int)Math.Pow(2,6) | achievementVal);
			achievementVal |= (int)Math.Pow (2, 6);
		}
	}

	// Upgrades the gun level and subtracts the cost from the total points
	public void UpgradeGun(){
		if (!achievements [2].achieved) {
			achievements [2].achieved = true;
			SetData ("achievement", (int)Math.Pow(2,2) | achievementVal);
			achievementVal |= (int)Math.Pow (2, 2);
		}
		totalPoints -= GetCost();
		SetData ("experience", totalPoints);
		gunUpgrade++;
		// upgrade levels are set in two byes. The gun upgrade is stored in the first byte
		SetData ("shop",  gunUpgrade | (shopVal & (255 << 8)));
		shopVal = gunUpgrade | (shopVal & (255 << 8));
	}

	public void UpgradePlayer(){
		if (!achievements [3].achieved) {
			achievements [3].achieved = true;
			SetData ("achievement", (int)Math.Pow(2,3) | achievementVal);
			achievementVal |= (int)Math.Pow (2, 3);
		}
		totalPoints -= GetCost();
		SetData ("experience", totalPoints);
		playerupgrade++;
		// store in second byte
		SetData ("shop", (playerupgrade << 8) | (shopVal & 255));
		shopVal = (playerupgrade << 8) | (shopVal & 255);
	}

	public void PlayedMultiplayer(){
		if (!achievements [4].achieved) {
			achievements [4].achieved = true;
			SetData ("achievement", (int)Math.Pow(2,4) | achievementVal);
			achievementVal |= (int)Math.Pow (2, 4);
		}
	}

	public void WonMultiplayer(){
		if (!achievements [5].achieved) {
			achievements [5].achieved = true;
			SetData ("achievement", (int)Math.Pow(2,5) | achievementVal);
			achievementVal |= (int)Math.Pow (2, 5);
		}
	}

	public void OnSceneChange(){
		enemies.Clear ();
		isPaused = false;
		GetScoreBoardOne ();
		GetScoreBoardTwo ();
	}
		
	public void OnEnable(){

		EventManager.OnWin += BeatLevel;
		EventManager.OnWin += Wipeout;
	}

	public void OnDisable(){

		EventManager.OnWin -= BeatLevel;
		EventManager.OnWin -= Wipeout;
	}

	public void ScoreCompare(){
		totalPoints += Score.scorePoints;
		SetData ("experience", totalPoints);
		if (currentLevel == 1) {
			if (Score.scorePoints > l1HS) {
				l1HS = Score.scorePoints;
				SetData ("highscore1", l1HS);
			}
		} else if (currentLevel == 2) {
			if (Score.scorePoints > l2HS) {
				l2HS = Score.scorePoints;
				SetData ("highscore2", l2HS);
			}
		}
	}

	public string getPlayerName(){
		return playerName;
	}

	public void setPlayerName(string name){
		playerName = name;
	}

	public void SetPW(String newString){
		pw = newString;
	}

	public void OnLogin(){
		LoadData ();
	}

	// This checks to see if a value was successfully added to the database and delets the js object
	IEnumerator DataUpdateCheck(dataUpdate du){
		processingData = true;
		while ((bool)du.database.getWaiting()) {
			yield return null;
		}
		Debug.Log (du.type);
		du.database.GetData (playerName, pw, du.type);
		while ((bool)du.database.getWaiting()) {
			yield return null;
		}
		print ("istherrorhere");
		int valComp = (int)du.database.getIntValue();
		if (valComp == du.value) {
			Debug.Log ("SUCCESS: " + valComp);
		} else {
			Debug.Log ("FAILURE: " + valComp);
		}
		Destroy (du.database.gameObject);
		processingData = false;
	}

	// There are a series of these to get data from the database; coroutines are required to allow
	// time for to connect and recieve data from the database
	IEnumerator SetBoolFromDB(Action<int> bitVal, int bitPos, string type){
		PhPUnity db = CreateCall ();
		db.GetData (playerName, pw, type);
		while ((bool)db.getWaiting()) {
			yield return null;
		}
		int value = (int)db.getIntValue();
		bitVal (value);
		if (((value & (1 << bitPos))>>bitPos) == 1) {
			achievements [bitPos].achieved = true;
		} else {
			achievements [bitPos].achieved = false;
		}
		Destroy (db.gameObject);
	}

	IEnumerator SetUpgradeLevelFromDB(Action<int> item, int bitPos, string type){
		PhPUnity db = CreateCall ();
		db.GetData (playerName, pw, type);
		while ((bool)db.getWaiting()) {
			yield return null;
		}
		shopVal = (int)db.getIntValue ();
		if (bitPos == 1) {
			item (shopVal & 255);
			Debug.Log ("Gun Upgrade Value: " + (shopVal & 255));
		} else if(bitPos == 2) {
			item (shopVal >> 8);
		}
		Destroy (db.gameObject);

	}
	 
	IEnumerator SetIntFromDB(Action<int> item, string type){
		PhPUnity db = CreateCall ();
		db.GetData (playerName, pw, type);
		while ((bool)db.getWaiting()) {
			yield return null;
		}
		item((int)db.getIntValue ());
		//Debug.Log (item);
		Destroy (db.gameObject);
	}

	// Loads all of the database info when the user logs in
	void LoadData(){
		StartCoroutine (SetIntFromDB ((x)=>l1HS=x,"highscore1"));
		StartCoroutine (SetIntFromDB ((x)=>l2HS=x,"highscore2"));

		StartCoroutine(SetUpgradeLevelFromDB((x)=>gunUpgrade = x,1, "shop"));
		StartCoroutine(SetUpgradeLevelFromDB((x)=>playerupgrade = x,2, "shop"));

		StartCoroutine(SetIntFromDB((x)=>totalPoints=x,"experience"));

		for(int i = 0; i<achievements.Length; i++){
			Debug.Log (achievements [i].achieved + " " + i); 
			StartCoroutine(SetBoolFromDB((y)=>achievementVal = y,i,"achievement"));
		}

		GetScoreBoardOne ();
		GetScoreBoardTwo ();
	}

	// Sets the data in the database through the js file. Adds the js object to a queue to
	// be verified and deleted
	void  SetData(string type, int val){
		Debug.Log (type + ": " + val);
		PhPUnity db = CreateCall ();
		print (val.ToString ());
		db.UpdateData (playerName, pw, type, val.ToString());
		databaseCalls.Enqueue(new dataUpdate(db,val,type));
	}

	// creates an object that holds the js file that communicates with the php database
	// a new object is created for each call so the iswaiting and return values don't get 
	// confused. 
	public PhPUnity CreateCall(){
		GameObject temp = new GameObject ();
		temp.transform.parent = transform;
		temp.AddComponent<PhPUnity> ();

		return (PhPUnity)temp.GetComponent<PhPUnity> ();
	}

	// returns the cost of upgrading the player/gun
	public int GetCost(){
		return (int)(upgradeCost * ((playerupgrade + gunUpgrade) / 2 + 1));
	}

	// gets the string of high scores and user names for level one
	public void GetScoreBoardOne(){
		StartCoroutine (getScoreboard ("scoreboard1"));
	}
	
	// gets the string of high scores and user names for level one
	public void GetScoreBoardTwo(){
		StartCoroutine (getScoreboard ("scoreboard2"));
	}

	// get the string that contains the top ten scores and their usernames for a level
	IEnumerator getScoreboard(string type){
		PhPUnity db = CreateCall ();
		db.GetData (playerName, pw, type);
		while ((bool)db.getWaiting()) {
			yield return null;
		}
		if (type == "scoreboard1") {
			sbL1 = (string)db.getStringValue ();
			print ("scoreboard1: " + sbL1);
		} else if(type == "scoreboard2"){
			sbL2 = (string)db.getStringValue();
			print ("scoreboard2: " + sbL2);
		}
	}

	// Achievements
	// High scores

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// James Williamson

// Contains methods for displaying certain strings in the shop and for upgrading the player
public class ShopScript : MonoBehaviour {

	public Text currentPoints;
	public Text cost;
	public Text gunLevel;
	public Text playerLevel;

	// Use this for initialization
	void Start () {
		UpdateValues ();
	}

	void OnEnable(){
		UpdateValues ();
	}
	
	// Updates the text boxes on the page
	void UpdateValues() {
		currentPoints.text = "Current Total Points: " + GameManager.gm.totalPoints;
		cost.text = "Cost to Upgrade: " + GameManager.gm.GetCost ();
		gunLevel.text = (GameManager.gm.gunUpgrade < GameManager.gm.maxLevel) ? "Gun Level: " + GameManager.gm.gunUpgrade : "Gun Level: MAX";
		playerLevel.text = (GameManager.gm.playerupgrade < GameManager.gm.maxLevel) ?  "Player Level: " + GameManager.gm.playerupgrade: "Gun Level: MAX";
	}

	// fucntions to upgrade the player (speed) or the gun (ammo)
	public void UpgradePlayer(){
		if (GameManager.gm.totalPoints >= GameManager.gm.GetCost () && GameManager.gm.gunUpgrade < GameManager.gm.maxLevel){
			GameManager.gm.UpgradePlayer ();
			UpdateValues ();
		}
	}

	public void UpgradeGun(){
		if (GameManager.gm.totalPoints >= GameManager.gm.GetCost () && GameManager.gm.playerupgrade < GameManager.gm.maxLevel) {
			GameManager.gm.UpgradeGun ();
			UpdateValues ();
		}
	}
}

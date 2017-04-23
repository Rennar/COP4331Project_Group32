using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// James Williamson

// Displays which achievements have been fulfilled
public class AchievementPage : MonoBehaviour {

	public Image[] checkboxes;

	// just sets the checkboxes to green if the achievement has een activated
	public void OnEnable(){
		int min = Mathf.Min (checkboxes.Length, GameManager.gm.achievements.Length);
		for (int i = 0; i < min; i++) {
			if (GameManager.gm.achievements [i].achieved) {
				checkboxes [i].color = Color.green;
			}
		}
	}
}

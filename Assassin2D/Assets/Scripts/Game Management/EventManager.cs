using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// ven manager for universal events
public static class EventManager {

	public delegate void DelVoid ();
	public static event DelVoid Pause;
	public static event DelVoid Resume;
	public static event DelVoid OnLose;
	public static event DelVoid OnWin;

	public static void TriggerPause(){
		if (Pause != null) {
			Pause ();
		}
	}

	public static void TriggerResume(){
		if (Resume != null) {
			Resume ();
		}
	}

	public static void TriggerOnLose(){
		if (OnLose != null) {
			OnLose ();
		}
	}

	public static void TriggerOnWin(){
		if (OnWin != null) {
			OnWin ();
		}
	}

}

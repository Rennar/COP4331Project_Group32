using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseClass : NetworkBehaviour {
    // Boolean Variable for pause.
	protected bool paused;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // If the game isn't paused then call the CustomUpdate method.
		if (!paused) {
			CustomUpdate ();
		}
	}

	protected virtual void CustomUpdate(){

	}

	void OnEnable(){
        // If OnEnable is called, we increment the EventMangers Pause and Resume variables by Pause and Resume.
        // Essentially telling the program when to pause the game.
        EventManager.Pause += Pause;
		EventManager.Resume += Resume;
	}

	void OnDisable(){
        // If OnDisable is called, we decrement the EventMangers Pause and Resume variables by Pause and Resume.
        // Essentially telling the program when to resume playing the game.
        EventManager.Pause -= Pause;
		EventManager.Resume -= Resume;
	}
    // If we call the Pause function
	public virtual void Pause()
	{
        // Our boolean is turned to true.
		paused = true;
        // Everything is frozen.
		if (GetComponent<Rigidbody2D>())
			GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

	}
    // If we call the Resume function
	public virtual void Resume()
	{
        // Our boolean is turned to false.
		paused = false;
        // Everthing is unfrozen.
		if (GetComponent<Rigidbody2D>())
			GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
	}

}

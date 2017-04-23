using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// This class makes it easy to pause everything.
public class BaseClass : NetworkBehaviour {

	protected bool paused;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!paused) {
			CustomUpdate ();
		}
	}

	protected virtual void CustomUpdate(){

	}

	void OnEnable(){
		EventManager.Pause += Pause;
		EventManager.Resume += Resume;
	}

	void OnDisable(){
		EventManager.Pause -= Pause;
		EventManager.Resume -= Resume;
	}

	public virtual void Pause()
	{
		paused = true;

		if (GetComponent<Rigidbody2D>())
			GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

	}

	public virtual void Resume()
	{
		paused = false;

		if (GetComponent<Rigidbody2D>())
			GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
	}

}

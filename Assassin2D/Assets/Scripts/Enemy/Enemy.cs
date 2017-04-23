using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// James Williamson
// Timothy Garrett

// Class for enemy behavior
public class Enemy : BaseClass {

	Unit pathManager;

	GameObject[] pathNodes;

	public bool isFollowing;
	public float followTime = 5;
    public int scoreValue = 10;
    int alertflag = 1;
    public AudioClip alert;
    private AudioSource alertSound;
	float followTimer;
	Rigidbody2D rb2d;


	public Transform target;

	void Awake(){
        AudioSource[] source = GetComponents<AudioSource>();

        alertSound = source[0];

        alertSound.clip = alert;
    }

	// Use this for initialization
	void Start () {
		pathManager = GetComponent<Unit> ();
		isFollowing = false;
		followTimer = 0;
		pathNodes = GameObject.FindGameObjectsWithTag ("PathNode");
		ReturnToPath ();
		rb2d = GetComponent<Rigidbody2D> ();
		GameManager.gm.enemies.Add (gameObject);
        
    }
	
	// Update is called once per frame
	protected override void CustomUpdate () {
		if (isFollowing) {
			followTimer += Time.deltaTime;
			if (followTimer > followTime || target == null) {
				followTimer = 0;
				ReturnToPath ();
				isFollowing = false;
			}
		}

		if (rb2d.velocity.magnitude != 0) {
			rb2d.velocity = Vector2.zero;
		}
	}

	// This will set the enemy on the nearest patrol route.
	void ReturnToPath(){
		Transform nearestPath = pathNodes[0].transform;
		float shortestDst = float.MaxValue;
		// Finds the closest node and then sets the path to that node's route
		foreach (GameObject pathPoint in pathNodes) {
			Vector3 dst = pathPoint.transform.position - transform.position;
			float sqrMag = dst.sqrMagnitude;
			if (sqrMag < shortestDst) {
				nearestPath = pathPoint.transform;
				shortestDst = sqrMag;
			}
		}
		SetTarget (nearestPath);

	}

	// sets the target for the path; if the target is the player, starts the follow timer
	public void SetTarget(Transform _target){
		if (_target.tag == "Player") {
			followTimer = 0;
			isFollowing = true;
            if (alertflag == 1)
            {
                alertSound.PlayOneShot(alert, 1f);
                alertflag = 0;
            }
        }
        target = _target;
		//Debug.Log ("Enemy target: " + target.name);
		pathManager.ChangePath(_target);
	}

	// add points on death
	public void OnDeath(){
        Score.scorePoints += scoreValue;
		GameManager.gm.enemies.Remove (gameObject);
		Destroy (gameObject);
	}
}

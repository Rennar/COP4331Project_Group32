using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseClass {

	Unit pathManager;

	GameObject[] pathNodes;

	public bool isFollowing;
	public float followTime = 5;
	float followTimer;


	public Transform target;

	void Awake(){
		pathManager = GetComponent<Unit> ();
		isFollowing = false;
		followTimer = 0;
		pathNodes = GameObject.FindGameObjectsWithTag ("PathNode");
		ReturnToPath ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	protected override void CustomUpdate () {
		if (isFollowing) {
			followTimer += Time.deltaTime;
			if (followTimer > followTime) {
				followTimer = 0;
				ReturnToPath ();
				isFollowing = false;
			}
		}
	}

	// This will set the enemy on the nearest patrol route.
	void ReturnToPath(){
		//  Need to make this smaller
		Transform nearestPath = pathNodes[0].transform;
		float shortestDst = float.MaxValue;
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
		
	public void SetTarget(Transform _target){
		if (_target.tag == "Player") {
			followTimer = 0;
			isFollowing = true;
		}
		target = _target;
		//Debug.Log ("Enemy target: " + target.name);
		pathManager.ChangePath(_target);
	}

	public void OnDeath(){
		Destroy (gameObject);
	}
}

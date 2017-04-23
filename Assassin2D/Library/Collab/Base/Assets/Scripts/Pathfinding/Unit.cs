using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : BaseClass {

	const float pathUpdateMoveThreshold = .2f;
	const float minPathUpdateTime = .2f;

	public Transform target;
	public float speed = 5;
	public float turnSpeed = 3;
	public float turnDst = .5f;
	public float stoppingDst = 3;

	bool doSlow;

	Path path;

	// Use this for initialization
	void Start () {
		doSlow = false;
		StartCoroutine (UpdatePath());
	}

	// 
	public void OnPathFound(Vector3[] waypoints, bool pathSuccessful){
		if (pathSuccessful) {
			path = new Path(waypoints,transform.position,turnDst, stoppingDst);
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	// Chane to a new path
	public void ChangePath(Transform newTarget){
		target = newTarget;
		// If following the player, slow down when nearing the destination. Otherwise, maintain speed
		// throughout the path
		if (newTarget.tag == "Player") {
			doSlow = true;
		} else {
			doSlow = false;
		}
		StopCoroutine ("UpdatePath");
		StopCoroutine ("FollowPath");
		StartCoroutine ("UpdatePath");
	}

	// Updates the path if the target has moved
	IEnumerator UpdatePath(){
		// This avoids any probelms that may occur during the call of all of the Start functions
		if (Time.timeSinceLevelLoad < .3f) {
			yield return new WaitForSeconds (.3f);
		}

		PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);

		// 
		float squareMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		// old position for referene
		Vector3 targetPosOld = target.position;

		while (!paused && target != null) {
			yield return new WaitForSeconds (minPathUpdateTime);
			if (target != null) {
				// if the target has moved signifigantly during the update time, make a new path to the 
				// new position
				if ((target.position - targetPosOld).sqrMagnitude > squareMoveThreshold) {
					PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
					targetPosOld = target.position;
				}
			}
		}
	}

	// Coroutine that controls the movement 
	IEnumerator FollowPath(){
		bool followingPath = true;
		int pathIndex = 0;
		float speedPercent = 1;

		if (path.lookPoints.Length > 0) {
			transform.eulerAngles = LookAt2D (transform, path.lookPoints [pathIndex]);
		}

		while (followingPath) {
			if (!paused) {
				Vector2 pos2D = new Vector2 (transform.position.x, transform.position.y);
				if (path.lookPoints.Length > 0) {
					while (path.turnBoundaries [pathIndex].HasCrossedLine (pos2D)) {
					
						if (pathIndex == path.finishLineIndex) {
							followingPath = false;
							break;
						} else {
							pathIndex++;
						}
						while (paused) {
							yield return new WaitForSeconds (.1f);
						}
					}

					if (followingPath) {
						if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
							speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFormPoint (pos2D) / stoppingDst);
							if (speedPercent < 0.01) {
								followingPath = false;
							}
						}

						Quaternion targetRot = Quaternion.Euler (LookAt2D (transform, path.lookPoints [pathIndex]));
						transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, Time.deltaTime * turnSpeed);
						if (doSlow) {
							transform.Translate (Vector3.up * speed * speedPercent * Time.deltaTime, Space.Self);
						} else {
							transform.Translate (Vector3.up * speed * Time.deltaTime, Space.Self);
						}
					}
				}
			}
			yield return null;
		}
	}

	// A function to have the unity look in a direction with x/y coordinates
	public Vector3 LookAt2D(Transform tf, Vector3 target){
		float rotation = Mathf.Atan2 (target.x - tf.position.x, target.y - tf.position.y) * 180 / Mathf.PI;
		return new Vector3 (tf.eulerAngles.x, tf.eulerAngles.y, rotation);
	}


	// Draws lines to show the path the unit will take
	public void OnDrawGizmos(){
		if (path != null) {
			path.DrawWithGizmos ();
			for (int i = 0; i < path.lookPoints.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path.lookPoints[i], new Vector3(.1f,.1f,.1f));
				if(i>0){
					Gizmos.DrawLine (path.lookPoints [i - 1], path.lookPoints [i]);
				}
			}
		}
	}
}

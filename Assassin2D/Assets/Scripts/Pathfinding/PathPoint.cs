using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// Type of node used for easily construction patrol paths in the editor
public class PathPoint : MonoBehaviour {

	List<PathPoint> neighbours;

	// Use this for initialization
	void Start () {
		neighbours = new List<PathPoint>();
		PathPoint[] siblings = transform.parent.gameObject.GetComponentsInChildren<PathPoint> ();
		RaycastHit2D hit;
		// gets all valid points from this one. Casts a ray to see if there is a straight line to any of its siblings.
		// this makes it easy to set up patrol paths in the editor
		foreach (PathPoint p in siblings) {
			if (p != this) {
				hit = Physics2D.Raycast (transform.position, p.gameObject.transform.position - transform.position);
				if (hit.collider.tag == "PathNode") {
					neighbours.Add (p);
				}
			}
		}
		neighbours.Remove (this);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (neighbours.Count > 0) {
			if (other.tag == "Enemy") {
				Enemy enemy = other.GetComponent<Enemy> ();
				if (enemy.target == transform) {
					// should be the direction of the incoming object
					Vector3 dir = transform.position - other.transform.position;
					float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;

					// this finds the node closest in angle to the direction the player is facing
					// I feel like this makes the patrol routes look a bit more natural, but we could simplify it 
					Transform target = neighbours [0].transform;
					float angleDif, minAngleDif = 360;
					foreach (PathPoint p in neighbours) {
						dir = p.transform.position - transform.position;
						float pAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
						angleDif = Mathf.Abs (Mathf.DeltaAngle (angle, pAngle));
						//Debug.Log (p.transform.name + " Angle: " + pAngle + " angleDif: " + angleDif + " minagleDif: " + minAngleDif); 
						if (angleDif < minAngleDif) {
							minAngleDif = angleDif;
							target = p.transform;
						}
					}
					//Debug.Log ("Made it to the end: " + target.name);
					enemy.SetTarget (target);
				}
			} else if (other.tag == "VIP") {
				VIP VIP = other.GetComponent<VIP> ();
				if (VIP.target == transform) {
					// should be the direction of the incoming object
					Vector3 dir = transform.position - other.transform.position;
					float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;

					// this finds the node closest in angle to the direction the player is facing
					// I feel like this makes the patrol routes look a bit more natural, but we could simplify it 
					Transform target = neighbours [0].transform;
					float angleDif, minAngleDif = 360;
					foreach (PathPoint p in neighbours) {
						dir = p.transform.position - transform.position;
						float pAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
						angleDif = Mathf.Abs (Mathf.DeltaAngle (angle, pAngle));
						//Debug.Log (p.transform.name + " Angle: " + pAngle + " angleDif: " + angleDif + " minagleDif: " + minAngleDif); 
						if (angleDif < minAngleDif) {
							minAngleDif = angleDif;
							target = p.transform;
						}
					}
					//Debug.Log ("Made it to the end: " + target.name);
					VIP.SetTarget (target);
				}
			}
		}
    }

	// This is for the rare case where the enemy stops following the play directly on tope of a path node
	// (inside the collider). It's identical to the OnTriggerEnter function, but it usually won;t be triggered
	// Since the node will rarely be the target.
	void OnTriggerStay2D(Collider2D other){
		if (neighbours.Count > 0) {
			if (other.tag == "Enemy") {
				Enemy enemy = other.GetComponent<Enemy> ();
				if (enemy.target == transform) {
					// should be the direction of the incoming object
					Vector3 dir = transform.position - other.transform.position;
					float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
					// for the one in a float.MaxValue^2 chance that it ends up exactly on top of the transform
					if (dir == Vector3.zero) {
						angle = other.transform.eulerAngles.y;
					}
					Transform target = neighbours [0].transform;
					float angleDif, minAngleDif = 360;
					foreach (PathPoint p in neighbours) {
						dir = p.transform.position - transform.position;
						float pAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
						angleDif = Mathf.Abs (Mathf.DeltaAngle (angle, pAngle));
						//Debug.Log (p.transform.name + " Angle: " + pAngle + " angleDif: " + angleDif + " minagleDif: " + minAngleDif); 
						if (angleDif < minAngleDif) {
							minAngleDif = angleDif;
							target = p.transform;
						}
					}
					//Debug.Log ("Made it to the end: " + target.name);
					enemy.SetTarget (target);
				}
			} else if (other.tag == "VIP") {
				VIP VIP = other.GetComponent<VIP> ();
				if (VIP.target == transform) {
					// should be the direction of the incoming object
					Vector3 dir = transform.position - other.transform.position;
					float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
					// for the one in a float.MaxValue^2 chance that it ends up exactly on top of the transform
					if (dir == Vector3.zero) {
						angle = other.transform.eulerAngles.y;
					}
					Transform target = neighbours [0].transform;
					float angleDif, minAngleDif = 360;
					foreach (PathPoint p in neighbours) {
						dir = p.transform.position - transform.position;
						float pAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
						angleDif = Mathf.Abs (Mathf.DeltaAngle (angle, pAngle));
						//Debug.Log (p.transform.name + " Angle: " + pAngle + " angleDif: " + angleDif + " minagleDif: " + minAngleDif); 
						if (angleDif < minAngleDif) {
							minAngleDif = angleDif;
							target = p.transform;
						}
					}
					//Debug.Log ("Made it to the end: " + target.name);
					VIP.SetTarget (target);
				}
			}
		}
    }
}

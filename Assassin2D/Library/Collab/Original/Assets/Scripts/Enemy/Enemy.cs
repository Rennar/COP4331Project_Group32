using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyMaster {

	public bool isFollowing;
	public float followTime = 5;
	float followTimer;
	Rigidbody2D rb2D;

	protected override void CustomAwake(){
		base.CustomAwake ();
		isFollowing = false;
		followTimer = 0;
		rb2D = GetComponent<Rigidbody2D> ();
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
		if (rb2D.velocity.magnitude != 0) {
			rb2D.velocity = Vector2.zero;
		}
	}
		
	public override void SetTarget(Transform _target){
		if (_target.tag == "Player") {
			followTimer = 0;
			isFollowing = true;
		}

		base.SetTarget (_target);
	}
}

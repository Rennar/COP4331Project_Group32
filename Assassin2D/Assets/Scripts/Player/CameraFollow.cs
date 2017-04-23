using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// simple camera follow script with smoothing; don't know if we actually ended up using this
public class CameraFollow : BaseClass {

	private Vector2 velocity;

	public float smoothTimeY;
	public float smoothTimeX;

	public Transform player;

	public Vector2 WorldSize;

	private Vector3 minCameraPos;
	private Vector3 maxCameraPos;

	private Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();

		// These are height and width/2
		float height = cam.orthographicSize*.8f;
		float width = height * cam.aspect;

		// bounds for the camera to not cross
		minCameraPos = new Vector3 (-WorldSize.x/2 + width, -WorldSize.y/2 + height, transform.position.z);
		maxCameraPos = new Vector3 (WorldSize.x / 2 - width, WorldSize.y / 2 - height, transform.position.z);
	}
	
	// smooths the camera movement so it takes a it to catch up with the character
	protected override void CustomUpdate () {
		float posX = Mathf.SmoothDamp (transform.position.x, player.position.x, ref velocity.x, smoothTimeX);
		float posY = Mathf.SmoothDamp (transform.position.y, player.position.y, ref velocity.y, smoothTimeY); 

		transform.position = new Vector3 (posX, posY, transform.position.z);

		transform.position = new Vector3 (Mathf.Clamp (transform.position.x, minCameraPos.x, maxCameraPos.x),
			Mathf.Clamp (transform.position.y, minCameraPos.y, maxCameraPos.y), transform.position.z);
	}
}

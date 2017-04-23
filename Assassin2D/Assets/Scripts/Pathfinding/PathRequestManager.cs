using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// Class that handles requests for paths from units
public class PathRequestManager : MonoBehaviour {

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;
	Pathfinding pathfinding;

	// a singleton that manages all path requests
	static PathRequestManager instance;

	bool isProcessingPath;

	void Awake(){
		instance = this;
		pathfinding = GetComponent<Pathfinding> ();
	}

	// Adds a path request to the queue
	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[],bool> callback){
		PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback);
		instance.pathRequestQueue.Enqueue (newRequest);
		instance.TryProcessNext ();
	}

	// Send a pathrequest into the pathfinding object to see if it's valid
	public void TryProcessNext(){
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue ();
			isProcessingPath = true;
			pathfinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	// Validate a successful path
	public void FinishedProcessingPath(Vector3[] path, bool success){
		currentPathRequest.callback (path, success);
		isProcessingPath = false;
		TryProcessNext ();
	}

	// Struct that constands the start point, end point, and all points in between as well as a bool that
	// shows whether the path is successful
	struct PathRequest{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[],bool> callback;
	
		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[],bool> _callback){
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}

}

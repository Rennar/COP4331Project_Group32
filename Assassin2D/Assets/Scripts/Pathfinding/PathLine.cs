using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// stores the full path, including information about how to navigate it
public class PathLine{

	public readonly Vector3[] lookPoints;
	public readonly Line[] turnBoundaries;
	public readonly int slowDownIndex;
	public readonly int finishLineIndex;

	public PathLine(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst){
		lookPoints = waypoints;
		// Path user must turn within these boundries
		turnBoundaries = new Line[lookPoints.Length];
		// The last node in the path
		finishLineIndex = turnBoundaries.Length - 1;

		Vector2 previousPoint = V3ToV2 (startPos);
		// sets up turn boundaries for each node
		for (int i = 0; i < lookPoints.Length; i++) {
			Vector2 currentPoint = V3ToV2 (lookPoints [i]);
			Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
			Vector2 turnBoundaryPoint = (i==finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;

			turnBoundaries [i] = new Line (turnBoundaryPoint, previousPoint-dirToCurrentPoint*turnDst);
			previousPoint = turnBoundaryPoint;
		}
		float dstFromEndPoint = 0;
		// determines when to start slowing the unit down
		for (int i = lookPoints.Length - 1; i > 0; i--) {
			dstFromEndPoint += Vector3.Distance (lookPoints [i], lookPoints [i - 1]);
			if (dstFromEndPoint > stoppingDst) {
				slowDownIndex = i;
				break;
			}
		}
	}

	// converts a vector3 to a vector2
	Vector2 V3ToV2(Vector3 v3){
		return new Vector2 (v3.x, v3.y);
	}

	// draws the lines while in editor to help visualize during deveopment
	public void DrawWithGizmos(){
		Gizmos.color = Color.black;
		foreach (Vector3 p in lookPoints) {
			Gizmos.DrawCube (p - Vector3.forward, new Vector3(.1f,.1f,.1f));
		}

		Gizmos.color = Color.white;
		foreach (Line l in turnBoundaries) {
			l.DrawWithGizmos (1);
		}
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// Used for path smoothing. Since we now move the units by pushing them forward
// in the direction they're facing (rather than just moving them directly to the
// node), we need to check if they pass an important point.
public struct Line{

	const float verticalGradient = 1e5f;

	float gradient;
	float yIntercept;

	Vector2 pointOnLine1;
	Vector2 pointOnLine2;

	float gradientPerpindicular;

	bool approachSide;

	// creates a "finish line" for each node to ensure that the unit is following the correct path
	// and rotates in time to pass through the node
	public Line(Vector2 pointOnLine, Vector2 pointPerpindicularToLine){
		float dx = pointOnLine.x - pointPerpindicularToLine.x;
		float dy = pointOnLine.y - pointPerpindicularToLine.y;

		if (dx == 0) {
			gradientPerpindicular = verticalGradient;
		} else {
			gradientPerpindicular = dy / dx;
		}

		if (gradientPerpindicular == 0) {
			gradient = verticalGradient;
		} else {
			gradient = -1 / gradientPerpindicular;
		}

		yIntercept = pointOnLine.y - gradient * pointOnLine.x;
		pointOnLine1 = pointOnLine;
		pointOnLine2 = pointOnLine + new Vector2 (1, gradient);

		approachSide = false;

		approachSide = GetSide (pointPerpindicularToLine);
	}

	bool GetSide(Vector2 p){
		return(p.x - pointOnLine1.x) * (pointOnLine2.y - pointOnLine1.y) > (p.y-pointOnLine1.y)*(pointOnLine2.x-pointOnLine1.x);
	}

	public bool HasCrossedLine(Vector2 p){
		return GetSide (p) != approachSide;

	}

	public float DistanceFormPoint(Vector3 p){
		float yIntercerptPerpindicular = p.y - gradientPerpindicular * p.x;
		float intersectX = (yIntercerptPerpindicular - yIntercept)/(gradient-gradientPerpindicular);
		float intersectY = gradient * intersectX + yIntercept;

		return Vector2.Distance(p, new Vector2(intersectX,intersectY));
	}

	public void DrawWithGizmos(float length){
		Vector3 LineDir = new Vector3 (1, gradient, 0).normalized;
		Vector3 lineCenter = new Vector3 (pointOnLine1.x, pointOnLine1.y, 0)- Vector3.forward;
		Gizmos.DrawLine (lineCenter - LineDir * length / 2, lineCenter + LineDir * length / 2);
	}
}

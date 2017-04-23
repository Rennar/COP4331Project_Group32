using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

// James Williamson

// Uses the A* algorithm to creat paths through a grid of nodes
public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;

	Grid grid;

	void Awake(){
		grid = GetComponent<Grid> ();
		requestManager = GetComponent<PathRequestManager> ();
	}
		

	public void StartFindPath(Vector3 startPos, Vector3 endPos){
		StartCoroutine(FindPath(startPos,endPos));
	}

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos){
		// The nodes actually used in the path
		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

		if (startNode.walkable && targetNode.walkable) {
			// For A*, the open set is the set of nodes that have not yet been checked
			// and compared to other nodes/paths, and the closed set is the set of nodes
			// that were found to be unuseful.
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node> ();

			openSet.Add (startNode);

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				// The path has completed successfully
				if (currentNode == targetNode) {
					pathSuccess = true;

					break;
				}

				foreach (Node n in grid.GetNeighbours(currentNode)) {
					if (!n.walkable || closedSet.Contains (n)) {
						continue;
					}

					// updates the g cost (distance from origin) and h (distance to destination) cost of each node around the 
					// node currently being examined. It also sets the parent of the node, which allows us to trace the path
					// later
					int newMovementCostToNeightbour = currentNode.gCost + GetDistance (currentNode, n) +n.movemnetPenalty;
					if (newMovementCostToNeightbour < n.gCost || !openSet.Contains (n)) {
						n.gCost = newMovementCostToNeightbour;
						n.hCost = GetDistance (n, targetNode);
						n.parent = currentNode;

						if (!openSet.Contains (n)) {
							openSet.Add (n);
						} else {
							openSet.UpdateItem (n);
						}
					}
				}
			}
		}
		// if the path works, make a 
		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode);
		}
		requestManager.FinishedProcessingPath (waypoints, pathSuccess);
		yield return null;
	}

	// fuction that creates an array containing the nodes used in the path
	Vector3[] RetracePath(Node startNode, Node endNode){
		List<Node> path = new List<Node> ();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}

		// More Accurate
		/*
		List<Vector3> waypoints = new List<Vector3> ();
		foreach (Node n in path) {
			waypoints.Add (n.worldPos);
		}
		waypoints.Reverse();
		*/

		// Simpler/more efficient (may be improved)
		Vector3[] waypoints = simplifyPath(path);
		Array.Reverse(waypoints);

		return waypoints;

	}

	// simplifies the path by only adding new nodes if the direction (anlge) between the cells changes
	Vector3[] simplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 dirOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 dirNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
			if(dirNew != dirOld){
				waypoints.Add (path [i].worldPos);
			}
			dirOld = dirNew;
		}
		return waypoints.ToArray ();
	}

	// for calculating g/h cost
	int GetDistance (Node a, Node b){
		int dstX = Mathf.Abs (a.gridX - b.gridX);
		int dstY = Mathf.Abs (a.gridY - b.gridY);

		if (dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		} else {
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}
}

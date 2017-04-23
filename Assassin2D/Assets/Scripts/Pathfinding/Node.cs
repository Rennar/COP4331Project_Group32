using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// Class for a "node" on the grid. These link together to form a path based on the lowest
// combined "fcost", or distance from the origin to the destination
public class Node : IHeapItem<Node>{

	public bool walkable;
	public Vector3 worldPos;
	public int gridX;
	public int gridY;
	public Node parent;
	public int movemnetPenalty;

	int heapIndex;

	// "Distance" from origin 
	public int gCost;
	// "Distance" to target
	public int hCost;

	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty){
		walkable = _walkable;
		worldPos = _worldPos;

		gridX = _gridX;
		gridY = _gridY;

		movemnetPenalty = _penalty;
	}

	public int fCost{
		get{
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get{
			return heapIndex;
		}
		set{
			heapIndex = value;
		}
	}

	public int CompareTo(Node node){
		int compare = fCost.CompareTo (node.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo (node.hCost);
		}

		return -compare;
	}


}

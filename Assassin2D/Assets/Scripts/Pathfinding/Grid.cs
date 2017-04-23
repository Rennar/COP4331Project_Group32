using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// James Williamson

// Class that sets up a grid of weighted transversable and untransversible cells
public class Grid : NetworkBehaviour {

	Node[,] grid;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public LayerMask unwalkableMask;
	public Transform player;
	// When assigning these, use only ONE LAYER PER MASK, even if they have the same terrain penalty.
	public TerrainType[] walkableRegions;
	LayerMask walkableMask;
	int obstacleProximityPenalty = 15;

	Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int,int> ();

	float nodeDiameter;
	int gridSizeX;
	int gridSizeY;

	public bool displayGridGizmos;
	int penaltyMin = int.MaxValue;
	int penaltyMax = int.MinValue; 

	void Awake(){
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

		foreach (TerrainType region in walkableRegions) {
			// Adds layers to the walkable mask with biwise or using the properties of unity layers (2^1 - 2^32)
			walkableMask.value |= region.terrainMask.value;
			walkableRegionsDictionary.Add ((int)Mathf.Log(region.terrainMask.value,2),region.terrainPenalty);
		}

		CreateGrid ();
	}

	public int MaxSize{
		get{
			return gridSizeX*gridSizeY;
		}
	}

	// Helps units follow the weighted path more easily.
	// Uses the box blur algorithm to assign heavier weights to areas near unwalkable nodes
	void BlurPenaltyMap(int blurSize){
		int kernalSize = 2 * blurSize + 1;
		int kernalExtents = blurSize;

		int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
		int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

		// horizontal pass
		for(int y = 0; y<gridSizeY; y++){
			for (int x = -kernalExtents; x <= kernalExtents; x++) {
				int sampleX = Mathf.Clamp (x,0, kernalExtents);
				penaltiesHorizontalPass [0, y] += grid[sampleX,y].movemnetPenalty;
			}
			for (int x = 1; x < gridSizeX; x++) {
				int removeIndex = Mathf.Clamp (x - kernalExtents - 1, 0, gridSizeX);
				int addIndex = Mathf.Clamp (x + kernalExtents, 0, gridSizeX-1);

				penaltiesHorizontalPass [x, y] = penaltiesHorizontalPass [x - 1, y] - grid[removeIndex,y].movemnetPenalty + grid[addIndex,y].movemnetPenalty;
			}
		}

		// vertical pass
		for(int x = 0; x<gridSizeX; x++){
			for (int y = -kernalExtents; y <= kernalExtents; y++) {
				int sampleY = Mathf.Clamp (y,0, kernalExtents);
				penaltiesVerticalPass [x, 0] += penaltiesHorizontalPass[x,sampleY];
			}

			int blurredPenalty = Mathf.RoundToInt((float)(penaltiesVerticalPass [x, 0]/(kernalSize * kernalSize)));
			grid [x, 0].movemnetPenalty = blurredPenalty;

			for (int y = 1; y < gridSizeY; y++) {
				int removeIndex = Mathf.Clamp (y - kernalExtents - 1, 0, gridSizeY);
				int addIndex = Mathf.Clamp (y + kernalExtents, 0, gridSizeY-1);

				penaltiesVerticalPass [x, y] = penaltiesVerticalPass [x, y-1] - penaltiesHorizontalPass[x,removeIndex] + penaltiesHorizontalPass[x,addIndex];
				blurredPenalty = Mathf.RoundToInt((float)(penaltiesVerticalPass [x, y]/(kernalSize * kernalSize)));
				grid [x, y].movemnetPenalty = blurredPenalty;

				if (blurredPenalty > penaltyMax) {
					penaltyMax = blurredPenalty;
				}
				if (blurredPenalty < penaltyMin) {
					penaltyMin = blurredPenalty;
				}
			}
		}
	}

	// Sets up the node grid for pathfinding
	void CreateGrid(){
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

		// this casts rays down on the level every nodediameter's worth of units to determine if that is a walkable square.
		// If a ray hits something in the unwalkable set, then the node becomes unwalkable, and the A* algorithm will avoid it.
		for(int x = 0; x<gridSizeX; x++){
			for (int y = 0; y < gridSizeY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up*(y*nodeDiameter+nodeRadius);
				bool walkable = !Physics2D.OverlapCircle (new Vector2(worldPoint.x, worldPoint.y), nodeRadius, unwalkableMask);

				int movementPenalty = 0;

				Collider2D hit = Physics2D.OverlapPoint (worldPoint,walkableMask);
				if (hit) {
					walkableRegionsDictionary.TryGetValue (hit.gameObject.layer, out movementPenalty);
				}

				if (!walkable) {
					movementPenalty += obstacleProximityPenalty;
				}

				grid [x, y] = new Node (walkable, worldPoint, x, y, movementPenalty);

			}
		}
		BlurPenaltyMap (3);
	}

	// Returns all neightbours of a node. Used to cycle through to find the lowest fCost
	public List<Node> GetNeighbours(Node node){
		List<Node> neighbours = new List<Node> ();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) {
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY>=0 && checkY <gridSizeY) {
					if (grid [checkX, checkY].walkable) {
						neighbours.Add (grid [checkX, checkY]);
					}
				}
			}
		}

		return neighbours;
	}

	// Takes in a world point and returns the corresponding node
	public Node NodeFromWorldPoint(Vector3 WorldPos){
		float percentX = (WorldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (WorldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid [x, y];
	}

	// Framerate issues AHOY!

	// Gizmos for visualization of walkable and blocked nodes
	void OnDrawGizmos(){
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x,gridWorldSize.y,-1));

		if (grid != null && displayGridGizmos) {
			Node playerNode = NodeFromWorldPoint (player.position);

			foreach (Node n in grid) {
				
				Gizmos.color = Color.Lerp(Color.white,Color.black,Mathf.InverseLerp(penaltyMin,penaltyMax,n.movemnetPenalty));
				Gizmos.color = n.walkable ? Gizmos.color : Color.red;

				if (playerNode == n) {
					Gizmos.color = Color.cyan;
				}

				Gizmos.DrawCube (n.worldPos, Vector2.one * (nodeDiameter));

			}
		}
	}

	// Terrain types for path penalties
	[System.Serializable]
	public class TerrainType{
		// ONE LAYER PER MASK
		public LayerMask terrainMask;
		public int terrainPenalty;
	}

}

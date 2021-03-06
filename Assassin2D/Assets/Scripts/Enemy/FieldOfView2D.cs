using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson

// Class for the vidual field of view of enemies, and to allow them to detet
// the player within that field of view
public class FieldOfView2D : BaseClass {

public float radius;
	[Range(0,360)]
	public float viewAngle;

	// What things are registered as targets if they're hit
	public LayerMask targetMask;
	// What things will block the field of view
	public LayerMask obstacleMask;

	// Targtets in FoV
	public List<Transform> visibleTargets;

	// Number of vertices in viewmesh
	public float meshResolution;

	// Helps the view mesh around corners
	public int EdgeResolveIterations;
	public float edgeDistanceThreshold;

	// For light effect; prevents the viewmesh from obscuring the whole object
	public float maskCutawayDst = .2f;

	// The mesh to be built
	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	// The object that's using this
	Enemy enemy; 

	// Use this for initialization
	void Start () {
		
		enemy = GetComponent<Enemy> ();

		// the mesh that will be drawn along the rays
		viewMesh = new Mesh();
		viewMesh.name = "ViewMesh";
		viewMeshFilter.mesh = viewMesh;

		StartCoroutine ("FindTargetsWithDelay", .2f);
	}

	// Update is called once per frame
	protected override void CustomUpdate () {
		DrawFieldofView ();

	}

	// checks with delay so resources aren't wasted check on every update
	IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true) {
			yield return new WaitForSeconds(delay);
			FindVisibleTargets ();
		}

	}

	// This uses a 2d physics circle to detect any objects in the target mask around it, then 
	// adds any of those objects within the view sector to the visible targets list. If
	// the player is seen, then it has the enemy target the player
	void FindVisibleTargets()
	{
		visibleTargets.Clear ();
		Collider2D[] TargetsinViewRadius = Physics2D.OverlapCircleAll (transform.position, radius, targetMask);

		for(int i =0; i< TargetsinViewRadius.Length; i++)
		{
			Transform target = TargetsinViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle (transform.up, dirToTarget) < viewAngle/2) 
			{
				float dstToTarget = Vector3.Distance (transform.position, target.position);
				if (!Physics2D.Raycast (transform.position, dirToTarget, dstToTarget, obstacleMask)) 
				{
					visibleTargets.Add(target);
					if (target.tag == "Player") {
						Debug.Log(target.name + " found");
						enemy.SetTarget (target.transform);
					}
				}
			}
		}
	}

	public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{

		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.z; 
		}

			return new Vector2 (1*Mathf.Sin (angleInDegrees*Mathf.Deg2Rad),Mathf.Cos(angleInDegrees*Mathf.Deg2Rad));
	}

	void DrawFieldofView()
	{
		int stepCount = Mathf.RoundToInt (viewAngle * meshResolution);
		
		float stepAngleSize = viewAngle / stepCount;
		ViewCastInfo oldViewCast = new ViewCastInfo();
		List<Vector3> ViewPoints = new List<Vector3>();

		for (int i = 0; i <= stepCount; i++) {
			// the angle of the sector that acts as the view
			float angle = 1*(transform.eulerAngles.z - viewAngle / 2 + stepAngleSize*i);

			// creates the spread of rays
			ViewCastInfo newViewCast = ViewCast(angle);
			if (i > 0) {
				bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.dst-newViewCast.dst) > edgeDistanceThreshold;
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded)) {
					EdgeInfo edge = FindEdge (oldViewCast, newViewCast);
					if (edge.pointA != Vector3.zero) {
					
						ViewPoints.Add (edge.pointA);
					}
					if (edge.pointB != Vector3.zero) {
						ViewPoints.Add (edge.pointB);
					}
				}
			}
			
			ViewPoints.Add (newViewCast.point);
			oldViewCast = newViewCast;

		}

		// Set up stuff to make tringles. There will be one triangle for each ray
		int vertexCount = ViewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices [0] = Vector3.zero;

		// Draws a mesh along the rays that were cast. This allows the mesh to be drawn arround objects in a 
		// way that looks like light shining. Meshes in untiy are made up of triangle, so these are done 3 vertices
		// at a time. This array stores the vertices, which wil then be passed to the meshrenderer.
		for (int i = 0; i < vertexCount - 1; i++) {
			vertices [i + 1] = transform.InverseTransformPoint(ViewPoints [i])+Vector3.up*maskCutawayDst;
			if (i < vertexCount - 2) {
				triangles [i * 3] = 0;
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}

		}

		// Clear the old mesh and update it with the new one (i.e. vertices to make a new one)
		viewMesh.Clear ();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals ();

	}

	// creates a ray and returns the info for the ray
	ViewCastInfo ViewCast(float globalAngle)
	{
		Vector2 dir = DirFromAngle (globalAngle, true);
		Vector2 pos2D = new Vector2 (transform.position.x, transform.position.y);
		RaycastHit2D hit = Physics2D.Raycast (pos2D, dir, radius, obstacleMask);

		// if the ray hits something, include where it hits and how far it travelled
		// Otherwise, include the end point according the the previously set max length
		// and the endpoint where that would lead.
		if (hit) {

			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		} else {

			return new ViewCastInfo (false, pos2D + dir * radius, radius, globalAngle);
		}
	}

	// Finds the edge along the base of the  
	EdgeInfo FindEdge(ViewCastInfo min, ViewCastInfo max){
		float minAngle = min.angle;
		float maxAngle = max.angle;

		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < EdgeResolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast (angle);

			bool edgeDistanceThresholdExceeded = Mathf.Abs(min.dst-newViewCast.dst) > edgeDistanceThreshold;
			if (newViewCast.hit = min.hit && !edgeDistanceThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast.point; 
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}

	// struct to hold in info for each ray in the view range
	public struct ViewCastInfo
	{
		public bool hit;
		public Vector2 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool _hit, Vector2 _point, float _dst, float _angle)
		{
			hit = _hit;
			point = _point;
			dst = _dst;
			angle = _angle;
		}
	}

	public struct EdgeInfo{
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB){
			pointA = _pointA;
			pointB = _pointB;
		}
	}


}

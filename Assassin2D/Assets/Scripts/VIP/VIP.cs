using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// James Williamson
// Timothy Garrett

// Class for the final enemy of the level
public class VIP : BaseClass {
    // Variables for the VIP.
    Unit pathManager;

    GameObject[] pathNodes;

    public bool isFollowing;
    public float followTime = 5;
    public int scoreValue = 15;
    float followTimer;


    public Transform target;

    void Awake(){
        
    }

    // Use this for initialization
    void Start()
    {
        // The VIP is created and given the values and what path to take.
		pathManager = GetComponent<Unit>();
		isFollowing = false;
		followTimer = 0;
		pathNodes = GameObject.FindGameObjectsWithTag("PathNode");
		ReturnToPath();
		GameManager.gm.enemies.Add (gameObject);
    }

    // Update is called once per frame
    protected override void CustomUpdate()
    {
        // If the VIP isFollowing is true then the amount of time he has been following is incremented.
        if (isFollowing)
        {
            followTimer += Time.deltaTime;
            // If the Follow time is greater then the allowed followerTime they quit following and
            // return to the nearest node reseting isFollowing to false.
            if (followTimer > followTime)
            {
                followTimer = 0;
                ReturnToPath();
                isFollowing = false;
            }
        }
    }

    // This will set the enemy on the nearest patrol route.
    void ReturnToPath()
    {
        
        Transform nearestPath = pathNodes[0].transform;
        float shortestDst = float.MaxValue;

        // Finds the nearest path and moves toward it.
        foreach (GameObject pathPoint in pathNodes)
        {
            Vector3 dst = pathPoint.transform.position - transform.position;
            float sqrMag = dst.sqrMagnitude;
            if (sqrMag < shortestDst)
            {
                nearestPath = pathPoint.transform;
                shortestDst = sqrMag;
            }
        }
        // Sets the target to be the next pathNode.
        SetTarget(nearestPath);
    }

    public void SetTarget(Transform _target)
    {
        // Sets the VIP target.
        target = _target;
        //Debug.Log ("VIP target: " + target.name);
        pathManager.ChangePath(_target);
    }
    // When the VIP is killed.
    public void OnDeath()
    {
        // Increment the Score and remove the object pausing the game and winning the Level.
		Score.scorePoints += scoreValue + Timer.intTime;
		GameManager.gm.ScoreCompare();
		GameManager.gm.enemies.Remove (gameObject);
		EventManager.TriggerPause ();
		EventManager.TriggerOnWin ();
        Destroy(gameObject);
    }
}

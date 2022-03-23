using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathfindingAI : MonoBehaviour
{
    // Pathfinding helper variables
    private Path path;
    private int currentWaypoint = 0;
    private Seeker seeker;

    private Vector2 bestDirection;

    [Header("Pathfinding")]
    [SerializeField] protected Transform target;
    [SerializeField] protected float nextWaypointDistance = 3f;
    [SerializeField] protected float activateDistance = 50f;
    [SerializeField] protected float pathUpdateSeconds = 0.5f;

    [Header("Custom Behavior")]
    [SerializeField] protected bool enablePathfinding = true;


    private void Start()
    {
        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void Update()
    {
        if (TargetInDistance() && enablePathfinding)
        {
            PathFollow();
        }
    }

    protected virtual void UpdatePath()
    {
        if (enablePathfinding && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }

    protected virtual void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // Go through all possible directions for enemy to move
        bestDirection = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;

        // Get Next Waypoint
        float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }  

    protected virtual bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    protected virtual void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public Vector2 getBestDirection() => bestDirection;
}

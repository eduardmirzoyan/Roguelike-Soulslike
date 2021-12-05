using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private float maxDistance;

    [SerializeField] private float currentDistance;

    public void setTarget(Transform target) => this.target = target;
    public bool canSeeTarget()
    {
        if (target == null)
            throw new System.Exception("TARGET WAS NULL FOR LINE OF SIGHT");

        Vector2 directionBetween = (target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionBetween, maxDistance, obstacleLayer);


        // If you hit nothing, then return false
        if (!hit)
            return false;

        // If you have a movement component
        var mv = GetComponent<Movement>();
        if (mv != null)
        {
            // Compare names
            if(hit.collider.name == target.name)
                // If the enemy is looking in the same direction that the target is then...
                return (directionBetween.x < -0.1f && mv.getFacingDirection() < 0) || (directionBetween.x > 0.1f && mv.getFacingDirection() > 0);
            else
                return false;
        }
        else
            return hit.collider.name == target.name;
    }

    public float distanceFromTarget()
    {
        if (target == null)
            return Mathf.Infinity;

        // Temp
        currentDistance = Vector2.Distance(transform.position, target.position);

        return Vector2.Distance(transform.position, target.position);
    }

    public void setMaxDistance(float max) => maxDistance = max;
}

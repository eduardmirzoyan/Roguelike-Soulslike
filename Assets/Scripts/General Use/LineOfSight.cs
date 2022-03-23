using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    [SerializeField] private LayerMask targetLayer;
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
                return (directionBetween.x < -0.1f && mv.getFacingDirection() < 0) 
                    || (directionBetween.x > 0.1f && mv.getFacingDirection() > 0);
            else
                return false;
        }
        else
            return hit.collider.name == target.name;
    }

    public Transform canSeeATarget() {
        var hits = Physics2D.OverlapCircleAll(transform.position, maxDistance, targetLayer);

        // If nothing is within range, then return null
        if (hits.Length < 0) {
            return null;
        }

        Transform result = null;
        float shortestDistance = Mathf.Infinity;
        var mv = GetComponent<Movement>();

        // Find the closest target that is also in line of sight
        foreach (var collider in hits) {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            Vector2 directionBetween = (collider.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionBetween, maxDistance, obstacleLayer);

            if (hit && distance < shortestDistance 
                && ((directionBetween.x < -0.1f && mv.getFacingDirection() < 0) 
                || (directionBetween.x > 0.1f && mv.getFacingDirection() > 0))) {

                shortestDistance = distance;
                result = collider.transform;
            }
        }

        return result;
    }


    public float distanceFromTarget()
    {
        if (target == null)
            return Mathf.Infinity;

        // Temp
        currentDistance = Vector2.Distance(transform.position, target.position);

        return currentDistance;
    }

    public void setMaxDistance(float max) => maxDistance = max;

    public static Collider2D getClosestCollider(Collider2D[] colliders, Transform transform) {
        if (colliders.Length < 0) {
            return null;
        }

        var result = colliders[0];
        float shortestDistance = Mathf.Infinity;
        foreach (var collider in colliders) {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < shortestDistance) {
                shortestDistance = distance;
                result = collider;
            }
        }
        return result;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}

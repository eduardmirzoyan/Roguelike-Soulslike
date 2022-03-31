using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float maxDistance;
    private float currentDistance;

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

    public bool canSeePoint(Vector2 point) {
        return !Physics2D.Linecast(transform.position, point, obstacleLayer);
    }

    public Collider2D[] getAllEnemiesInSight() {
        // Get all enemies within range
        var hits = Physics2D.OverlapCircleAll(transform.position, maxDistance, targetLayer);

        // If nothing is within range, then return null
        if (hits.Length < 0) {
            return null;
        }

        List<Collider2D> result = new List<Collider2D>();

        // Get movement
        var mv = GetComponent<Movement>();
        // Make sure that you can see every enemy
        foreach (var collider in hits) {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            Vector2 directionBetween = (collider.transform.position - transform.position).normalized;

            // Raycast towards each enemy
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionBetween, distance, obstacleLayer);

            // If the raycast doesn't hit an obstacle and the character is facing the correct way, then keep the
            if (!hit && ((directionBetween.x < -0.1f && mv.getFacingDirection() < 0) 
                || (directionBetween.x > 0.1f && mv.getFacingDirection() > 0))) {
                result.Add(collider); // Add to result
            }
        }

        return result.ToArray();
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

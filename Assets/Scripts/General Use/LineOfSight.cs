using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    
    public bool canSeePoint(Vector2 point) {
        return !Physics2D.Linecast(transform.position, point, obstacleLayer);
    }

    public Collider2D[] getAllEnemiesInSight(float maxDistance) {
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
}

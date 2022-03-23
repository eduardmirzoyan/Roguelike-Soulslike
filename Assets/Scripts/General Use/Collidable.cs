using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collidable : MonoBehaviour
{
    protected Collider2D collider2D;
    [SerializeField] private LayerMask layer;
    private ContactFilter2D whatToCheck;
    private Collider2D[] hits = new Collider2D[10]; // We only want up to 10 different collisions at each frame

    public bool hasCollided;

    protected void Start()
    {
        collider2D = GetComponent<Collider2D>();
        whatToCheck.SetLayerMask(layer);
    }

    // Calls on oncollide function on everything it detects
    public void checkCollisions(Action<Collider2D> onCollide)
    {
        collider2D.OverlapCollider(whatToCheck, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) // If you hit nothing, then do nothing
            {
                continue;
            }

            // takes in arguments
            onCollide(hits[i]); // Calls collide func 

            // Clean up array
            hits[i] = null;
        }
    }

    public void checkOnlyOneCollision(Action<Collider2D> onCollide)
    {
        if (hasCollided)
            return;

        collider2D.OverlapCollider(whatToCheck, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) // If you hit nothing, then do nothing
            {
                continue;
            }

            // takes in arguments
            onCollide(hits[i]); // Calls collide func 

            // Clean up array
            hits[i] = null;

            hasCollided = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NewCollider : MonoBehaviour
{
    [SerializeField] private Collider2D coll;
    [SerializeField] private LayerMask layer;
    [SerializeField] private ContactFilter2D whatToCheck;
    [SerializeField] private Collider2D[] hits = new Collider2D[10]; // We only want up to 10 different collisions at each frame

    [SerializeField] private bool hasCollided;
    [SerializeField] private float rehitCooldown;

    protected void Awake()
    {
        coll = GetComponent<Collider2D>();
        whatToCheck.SetLayerMask(layer);
    }

    private void FixedUpdate()
    {
        if (!hasCollided)
            coll.OverlapCollider(whatToCheck, hits);         
    }

    public Collider2D[] getHits() => hits;

    public void resetHits()
    {
        hits = new Collider2D[10];
        StartCoroutine(rehitTimer());
    }

    // Temp fix
    public void enableHit()
    {
        Debug.Log("rest hit");
        hits = new Collider2D[10];
        hasCollided = false;
        rehitCooldown = 3f;
    }

    private IEnumerator rehitTimer()
    {
        hasCollided = true;
        yield return new WaitForSeconds(rehitCooldown);
        hasCollided = false;
    }

    // Set new layer and reset the hasCollided check
    public void setNewLayer(LayerMask layer)
    {
        whatToCheck.SetLayerMask(layer);
        hasCollided = false;
    }
}

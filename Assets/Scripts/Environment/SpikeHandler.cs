using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHandler : MonoBehaviour
{
    [SerializeField] protected Transform spikeCheck;
    [SerializeField] protected float checkRadius;
    [SerializeField] protected LayerMask spikeLayer;
    [SerializeField] protected Vector2 bounds;

    [SerializeField] private int damage;
    [SerializeField] private Rigidbody2D body;

    [SerializeField] private Damage spikeDamage;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (touchingSpikes())
        {
            if(GetComponent<Damageable>() != null)
            {
                spikeDamage.origin = transform;
                GetComponent<Damageable>().takeDamage(spikeDamage);
            }
        }
    }

    private bool touchingSpikes()
    {
        return Physics2D.OverlapBox(spikeCheck.position, bounds, 0, spikeLayer) && body.velocity.y < -0.05f;
    }
}

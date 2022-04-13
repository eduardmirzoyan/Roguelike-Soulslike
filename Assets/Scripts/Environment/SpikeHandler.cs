using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] protected Transform spikeCheck;
    [SerializeField] protected LayerMask spikeLayer;
    [SerializeField] protected Vector2 bounds;

    [Header("Settings")]
    [SerializeField] private int damage;
    [SerializeField] private bool isImmune;
    

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // If immune, don't do anything
        if (isImmune)
            return;

        if (touchingSpikes() && TryGetComponent(out Damageable damageable))
        {
            Damage dmg = new Damage {
                damageAmount = damage,
                source = DamageSource.fromEnvironment,
                origin = transform,
                color = Color.red
            };
            damageable.takeDamage(dmg);
        }
    }

    public void setImmune(bool state) {
        isImmune = state;
    }

    private bool touchingSpikes()
    {
        return Physics2D.OverlapBox(spikeCheck.position, bounds, 0, spikeLayer) && body.velocity.y < -0.1f;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(spikeCheck.position, bounds);
    }
}

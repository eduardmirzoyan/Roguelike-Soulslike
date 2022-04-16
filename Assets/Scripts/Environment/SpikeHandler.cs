using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Transform spikeCheck;
    [SerializeField] private LayerMask spikeLayer;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private BaseEffect bleedEffect;

    [Header("Settings")]
    [SerializeField] private int damage;
    [SerializeField] private bool isImmune;
    
    private Transform spikes;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spikes = GameObject.Find("Spikes").gameObject.transform;
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
                origin = spikes,
                effects = new List<BaseEffect>() { bleedEffect },
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

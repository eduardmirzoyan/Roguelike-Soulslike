using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Displacable : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Movement mv;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private GameObject particles;

    [Header("Settings")]
    [SerializeField] private bool stunImmune;
    [SerializeField] private bool knockbackImmune;
    [SerializeField] public bool twoDimensionalKnockback;
    [SerializeField] private float wallCheckDistance = 0.1f;

    // Knockback counters
    private float knockbackSpeed = 0;
    private float knockbackDuration = 0;
    private Vector2 knockbackDirection;
    private float startTime = 0;

    // Stun counters
    private float stunDuration = 0;
    private float hitBoxWidth = 0;
    private bool isPinned = false;

    private void Awake()
    {
        mv = GetComponent<Movement>();
        collider2d = GetComponent<Collider2D>();
        hitBoxWidth = collider2d.bounds.extents.x;
    }

    public void triggerKnockback(float pushForce, float duration, Vector3 origin)
    {
        if (knockbackImmune) {
            PopUpTextManager.instance.createPopup(gameObject.name + " is immune to knockback.", Color.gray, transform.position);
            return;
        }

        startTime = 0f;
        knockbackSpeed += pushForce;
        knockbackDuration = duration;
        knockbackDirection = (transform.position - origin).normalized;
        isPinned = false;
    }

    public void triggerStun(float duration)
    {
        if (stunImmune) {
            PopUpTextManager.instance.createPopup(gameObject.name + " is immune to stun.", Color.gray, transform.position);
            return;
        }

        stunDuration += duration;
        if (particles != null) {
            var ps = Instantiate(particles, transform).GetComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = duration;
            ps.Play();
        }
    }

    public bool isDisplaced() {
        return stunDuration > 0 || knockbackSpeed > 0.1f;
    }

    public bool isStunned() {
        return stunDuration > 0;
    }

    public bool isKnockedback() {
        return knockbackSpeed > 0.1f;
    }

    public void performDisplacement()
    {
        // Stun
        performStun();

        // Knockback
        performKnockback();
    }

    private void performStun() {
        if(stunDuration > 0) {
            stunDuration -= Time.deltaTime;
        }
    }

    private void performKnockback() {
        if (startTime <= knockbackDuration) {
            mv.setFacingDirection(-knockbackDirection.x);

            if(knockbackSpeed > 0.1f) {
                knockbackSpeed = Mathf.SmoothStep(knockbackSpeed, 0, startTime / knockbackDuration);
            }
                
            // Check to see if collided with wall
            
            if (!isPinned) {
                var hit = Physics2D.Raycast(collider2d.bounds.center, Vector2.right * Mathf.Sign(knockbackDirection.x), 
                                        hitBoxWidth + wallCheckDistance, 1 << LayerMask.NameToLayer("Ground"));

                // If you hit a wall while knockedback, then stun
                if (hit) {
                    // Remove knocback
                    resetKnockback();

                    // Trigger stun
                    triggerStun(0.5f);

                    // Deal damage
                    if (TryGetComponent(out Health health)) {
                        health.reduceHealth(5);
                        PopUpTextManager.instance.createPopup("" + 5, Color.red, transform.position);
                    }

                    isPinned = true;
                }
            }

            startTime += Time.deltaTime;
        }

        if (twoDimensionalKnockback)
            mv.twoDDash(knockbackSpeed, knockbackDirection);
        else 
            mv.dash(knockbackSpeed, knockbackDirection.x);
    }

    public float getStunDuration() {
        return stunDuration;
    }

    private void resetKnockback() {
        knockbackDirection = Vector2.zero;
        knockbackDuration = 0;
        knockbackSpeed = 0;
    }

    private void OnDrawGizmosSelected() {
        if (knockbackDirection != null && collider2d != null) {
            // Test
            Gizmos.color = Color.red;
            Gizmos.DrawRay(collider2d.bounds.center, Vector2.right * Mathf.Sign(knockbackDirection.x) * (hitBoxWidth + wallCheckDistance));
        }
    }
}

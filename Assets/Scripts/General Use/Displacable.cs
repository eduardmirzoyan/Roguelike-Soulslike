using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Displacable : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Movement mv;

    [Header("Settings")]
    [SerializeField] private bool stunImmune;
    [SerializeField] private bool knockbackImmune;
    [SerializeField] public bool twoDimensionalKnockback;

    // Knockback counters
    private float knockbackSpeed = 0;
    private float knockbackDuration = 0;
    private Vector2 knockbackDirection;
    private float startTime = 0;

    // Stun counters
    private float stunDuration = 0;

    private void Start()
    {
        mv = GetComponent<Movement>();
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
        // knockbackDirection = getDirectionFromPoint(origin);
    }

    public void triggerStun(float duration)
    {
        if (stunImmune) {
            PopUpTextManager.instance.createPopup(gameObject.name + " is immune to stun.", Color.gray, transform.position);
            return;
        }
        stunDuration += duration;
        GameManager.instance.stunAnimation(transform, duration);
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
        if(stunDuration > 0)
            stunDuration -= Time.deltaTime;
    }

    private void performKnockback() {
        if (startTime <= knockbackDuration) {
            mv.setFacingDirection(-knockbackDirection.x);

            if(knockbackSpeed > 0.1f)
                knockbackSpeed = Mathf.Lerp(knockbackSpeed, 0, startTime / knockbackDuration);

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

    public int getDirectionFromPoint(Vector3 origin)
    {
        float normalizedXPush = (transform.position - origin).normalized.x;
        return (normalizedXPush > 0.1f) ? 1 : (normalizedXPush < -0.1f) ? -1 : 0;
    }

    
}

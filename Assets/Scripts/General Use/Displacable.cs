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
    [SerializeField] private float knockbackSpeed = 0;
    [SerializeField] private float knockbackDuration = 0;
    [SerializeField] private float stunDuration = 0;

    private float knockbackDirection;
    private float startTime = 0;

    private void Start()
    {
        mv = GetComponent<Movement>();
    }

    public void triggerKnockback(float pushForce, float duration, Vector3 origin)
    {
        if (knockbackImmune) {
            GameManager.instance.CreatePopup(gameObject.name + " is immune to knockback.", transform.position);
            return;
        }
        startTime = 0f;
        knockbackSpeed += pushForce;
        knockbackDuration = duration;
        knockbackDirection = getDirectionFromPoint(origin);
    }

    public void triggerStun(float duration)
    {
        if (stunImmune) {
            GameManager.instance.CreatePopup(gameObject.name + " is immune to stun.", transform.position);
            return;
        }
        stunDuration += duration;
        GameManager.instance.stunAnimation(transform, duration);
    }

    public bool isDisplaced() {
        return stunDuration > 0 || knockbackSpeed > 0.1f;
    }

    public void performDisplacement()
    {
        if (startTime <= knockbackDuration || stunDuration > 0)
        {
            mv.setFacingDirection(-knockbackDirection);

            if(knockbackSpeed > 0.1f)
                knockbackSpeed = Mathf.Lerp(knockbackSpeed, 0, startTime / knockbackDuration);

            startTime += Time.deltaTime;

            if(stunDuration > 0)
                stunDuration -= Time.deltaTime;
        }

        mv.dash(knockbackSpeed, knockbackDirection);
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

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

    [SerializeField] private string savedState;
    [SerializeField] private string knockbackState;

    private float knockbackDirection;
    private float startTime = 0;

    private void Start()
    {
        mv = GetComponent<Movement>();
    }

    public void triggerKnockback(float pushForce, float duration, Vector3 origin)
    {
        Debug.Log("knock");
        startTime = 0f;
        knockbackSpeed += pushForce;
        knockbackDuration = duration;
        knockbackDirection = getDirectionFromPoint(origin);
    }

    public void triggerStun(float duration)
    {
        Debug.Log("stun");
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
            if(knockbackSpeed > 0.1f)
                knockbackSpeed = Mathf.Lerp(knockbackSpeed, 0, startTime / knockbackDuration);

            startTime += Time.deltaTime;

            if(stunDuration > 0)
                stunDuration -= Time.deltaTime;
        }

        mv.dash(knockbackSpeed, knockbackDirection);
    }

    public float getKnockbackDirection() {
        return knockbackDirection;
    }

    public float getStunDuration() {
        return stunDuration;
    }

    public void saveState(string state) {
        if (state != knockbackState) {
            savedState = state;
        }
    }

    public string loadState() {
        return savedState;
    }

    public int getDirectionFromPoint(Vector3 origin)
    {
        float normalizedXPush = (transform.position - origin).normalized.x;
        return (normalizedXPush > 0.1f) ? 1 : (normalizedXPush < -0.1f) ? -1 : 0;
    }
}

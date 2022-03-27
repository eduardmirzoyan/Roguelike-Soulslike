using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Animator))]
public class Displacable : MonoBehaviour
{
    [Header("Changeable Values")]
    [SerializeField] private bool useAnimations;
    [SerializeField] private int poiseThreshold;

    [SerializeField] private bool stunImmune;
    [SerializeField] private bool knockbackImmune;
    [SerializeField] private bool staggerImmune;

    [SerializeField] private Movement mv;

    [SerializeField] private float knockbackSpeed;

    [SerializeField] private float knockbackDuration;

    [SerializeField] private float stunDuration;

    private float knockbackDirection;
    public bool isDisplaced { get; private set; }
    [SerializeField] private float startTime;

    private void Start()
    {
        mv = GetComponent<Movement>();
    }

    public void triggerStagger(Vector3 origin)
    {
        if (staggerImmune)
        {
            Debug.Log(name + " is immune to being staggered.");
            return;
        }

        float normalizedXPush = (transform.position - origin).normalized.x;
        int horizontalPushDirection = ((normalizedXPush > 0.1f) ? 1 : (normalizedXPush < -0.1f) ? -1 : 0);


        // Store constant knockback speed
        knockbackSpeed = 5;
        knockbackDirection = horizontalPushDirection;
    }

    //*************************************************************************

    // Assume knockback
    public void triggerKnockback(float pushForce, float duration, Vector3 origin)
    {
        Debug.Log("knock");
        startTime = 0f;
        knockbackSpeed += pushForce;
        knockbackDuration = duration;
        knockbackDirection = getDirectionFromPoint(origin);
        isDisplaced = true;
    }

    // Assume stun
    public void triggerStun(float duration)
    {
        Debug.Log("stun");
        stunDuration += duration;
        isDisplaced = true;
        GameManager.instance.stunAnimation(transform, duration);
    }

    // New displacement logic
    public void performDisplacement()
    {
        //var startTime = 0f;
        if (startTime <= knockbackDuration || stunDuration > 0)
        {
            if(knockbackSpeed > 0.1f)
                knockbackSpeed = Mathf.Lerp(knockbackSpeed, 0, startTime / knockbackDuration);

            startTime += Time.deltaTime;

            if(stunDuration > 0)
                stunDuration -= Time.deltaTime; // Reduce stun

            //Debug.Log("Speed: " + knockbackSpeed + " Dur: " + knockbackDuration);
            //Debug.Log("Stun Dur: " + stunDuration);

            //mv.dashWithVelocity(knockbackSpeed, knockbackDirection);
        }
        else
        {
            knockbackDuration = 0;
            isDisplaced = false;
        }
    }

    public int getDirectionFromPoint(Vector3 origin)
    {
        float normalizedXPush = (transform.position - origin).normalized.x;
        return (normalizedXPush > 0.1f) ? 1 : (normalizedXPush < -0.1f) ? -1 : 0;
    }
}

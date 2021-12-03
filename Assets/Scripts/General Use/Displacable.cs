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
    
    private float knockbackDecay = 0.05f;
    public float knockbackSpeed { get; private set; }
    private float knockbackDirection;

    public float knockbackDuration { get; private set; }

    private float stunDuration;

    public bool stunRequest { get; private set; }
    public bool knockbackRequest { get; private set; }
    public bool isStaggered { get; private set; }

    public bool isDisplaced { get; private set; }
    private float startTime;
    public enum DisplacedState
    {
        Free,
        Stunned,
        Knockedback,
        Staggered
    }
    public DisplacedState state;

    private void Start()
    {
        mv = GetComponent<Movement>();
    }

    public IEnumerator perfromKnockback()
    {
        knockbackRequest = false;
        while (Mathf.Abs(knockbackSpeed) > 2f)
        {
            knockbackSpeed = Mathf.Lerp(knockbackSpeed, 0, knockbackDecay);
            mv.dashWithVelocity(knockbackSpeed, knockbackDirection);   
            yield return null;
        }
        knockbackSpeed = 0;
        mv.Stop();
    }

    public IEnumerator performStun()
    {
        stunRequest = false;
        yield return new WaitForSeconds(knockbackDuration);
        knockbackDuration = 0;
    }

    public bool isFree()
    {
        return state == DisplacedState.Free;
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
        knockbackRequest = true;
    }

    public void triggerKnockback(float pushForce, Vector3 origin)
    {
        if (knockbackImmune)
        {
            Debug.Log(name + " is immune to being knockback.");
            return;
        }

        float normalizedXPush = (transform.position - origin).normalized.x;
        int horizontalPushDirection = ((normalizedXPush > 0.1f) ? 1 : (normalizedXPush < -0.1f) ? -1 : 0);

        // Store knockback speed
        knockbackSpeed = pushForce;
        knockbackDirection = horizontalPushDirection;
        knockbackRequest = true;
    }

    public void triggerStun(float duration)
    {
        if (stunImmune)
        {
            Debug.Log(name + " is immune to being stunned.");
            return;
        }

        // Store duration
        knockbackDuration = duration;
        stunRequest = true;
    }

    // Assume knockback
    public void triggerDisplace(float pushForce, float duration)
    {
        startTime = 0f;
        knockbackSpeed += pushForce;
        knockbackDuration = duration;
        isDisplaced = true;
    }

    // Assume stun
    public void triggerSStun(float duration)
    {
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
            knockbackSpeed = Mathf.Lerp(knockbackSpeed, 0, startTime / knockbackDuration);
            startTime += Time.deltaTime; // New logic?

            if(stunDuration > 0)
                stunDuration -= Time.deltaTime; // Reduce stun

            //Debug.Log("Speed: " + knockbackSpeed + " Dur: " + knockbackDuration);
            //Debug.Log("Stun Dur: " + stunDuration);

            mv.dashWithVelocity(knockbackSpeed, 1);
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

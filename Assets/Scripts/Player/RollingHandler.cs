using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(AnimationHandler))]
public class RollingHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Movement mv;

    [Header("Settings")]
    [SerializeField] private float rollCooldown = 1f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollSpeed = 350f;
    private float rollCooldownTimer;
    private float rollTimer;
    private float rollDirection;

    [SerializeField] private float lerpFactor;

    private float workingRollSpeed;

    private void Start() {
        mv = GetComponent<Movement>();
    }

    private void FixedUpdate() {
        if (rollCooldownTimer > 0) {
            rollCooldownTimer -= Time.deltaTime;
        }
    }

    // Rolling values
    public void startRoll(float direction) {
        // Set roll direction based on input
        if (direction > 0.2f)
            rollDirection = 1;
        else if (direction < -0.2f)
            rollDirection = -1;
        else 
            rollDirection = mv.getFacingDirection();

        // Set roll speed to lerp
        workingRollSpeed = rollSpeed;

        // Start timer
        rollTimer = rollDuration;
    }

    public bool canRoll() {
        return rollCooldownTimer <= 0f;
    }

    public void roll() {
        if (rollTimer > 0) {
            rollTimer -= Time.deltaTime;

            // Lerp speed
            mv.dash(workingRollSpeed, rollDirection);
            // workingRollSpeed = Mathf.Lerp(workingRollSpeed, 0, lerpFactor);
        }
    }

    public void startCooldown() {
        rollCooldownTimer = rollCooldown;
    }

    public bool isDoneRolling() {
        return rollTimer <= 0;
    }

    public void setRollSpeed(float newSpeed) {
        rollSpeed = newSpeed;
    }

    public float getRollSpeed() {
        return rollSpeed;
    }

    public void setRollDuration(float duration) {
        rollDuration = duration;
    }

    public float getRollDuration() {
        return rollDuration;
    }

    public float getCooldownRatio() {
        if (rollCooldown <= 0) {
            return 0;
        }
        return rollCooldownTimer / rollCooldown;
    }
}

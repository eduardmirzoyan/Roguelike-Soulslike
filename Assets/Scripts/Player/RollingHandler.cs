using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(AnimationHandler))]
public class RollingHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Movement mv;
    [SerializeField] private AnimationHandler animationHandler;

    [Header("Settings")]
    [SerializeField] private float rollCooldown = 1f;
    [SerializeField] private string rollAnimation = "Roll";
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollSpeed = 350f;
    private float rollCooldownTimer;
    private float rollTimer;
    private float rollDirection;

    private void Start() {
        mv = GetComponent<Movement>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    private void FixedUpdate() {
        if (rollCooldownTimer > 0) {
            rollCooldownTimer -= Time.deltaTime;
        }
    }

    // Rolling values
    public void startRoll(float direction) {
        rollTimer = rollDuration;
        rollDirection = direction;
    }

    public bool canRoll() {
        return rollCooldownTimer <= 0f;
    }

    public void roll() {
        animationHandler.changeAnimationState(rollAnimation);
        mv.WalkAtSpeed(rollDirection, rollSpeed);
        if (rollTimer > 0) {
            rollTimer -= Time.deltaTime;
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

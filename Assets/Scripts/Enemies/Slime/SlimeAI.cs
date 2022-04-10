using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
public class SlimeAI : EnemyAI
{
    [Header("Slime Settings")]
    [SerializeField] private float prepareDuration;
    [SerializeField] private GameObject deathParticles;

    [Header("Slime Animations")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walk";
    [SerializeField] private string riseAnimation = "Rise";
    [SerializeField] private string fallAnimation = "Fall";
    [SerializeField] private string prepareToJumpAnimation = "Prepare";
    [SerializeField] private string stunnedAnimation = "Stunned";

    private float prepareTimer;

    private enum SlimeState {
        Idle,
        Preparing,
        Stunned,
        Dead
    }

    [SerializeField] private SlimeState slimeState;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        wanderTimer = wanderRate;
        slimeState = SlimeState.Idle;
    }

    private void Update() {
        if (slimeState != SlimeState.Dead && health.isEmpty())
        {
            // Call it's death method
            Die();

            // Prevent movement
            mv.Walk(0);

            // Change state to dead
            slimeState = SlimeState.Dead;

            // Spawn particles
            Instantiate(deathParticles, transform.position, Quaternion.identity);

            // Destroy instantly
            Destroy(gameObject);
        }
    }

    protected void FixedUpdate()
    {
        switch(slimeState) {
        case SlimeState.Idle:

            handleAnimation();

            wander();

            handleDisplacement();

            handleRetaliation();

        break;
        case SlimeState.Preparing:
            if (prepareTimer > 0) {
                animationHandler.changeAnimationState(prepareToJumpAnimation);
                prepareTimer -= Time.deltaTime;
            }
            else {
                bounce(mv.getFacingDirection());
                wanderTimer = wanderRate;
                slimeState = SlimeState.Idle;
            }

            handleDisplacement();

        break;
        case SlimeState.Stunned:
            displacable.performDisplacement();

            if (!displacable.isDisplaced()) {
                // Return to Idle
                slimeState = SlimeState.Idle;
            }

        break;
        }
    }

    private void wander() {
        // Don't move while on the ground
        if (mv.isGrounded()) {
            // Don't move
            mv.Walk(0);
        }

        // If you are on cooldown, then skip
        if (wanderTimer > 0) {
            wanderTimer -= Time.deltaTime;
            return;
        }

        // Randomly choose a direction
        int randomDirection = Random.Range(0, 2) == 1 ? 1 : -1;

        // Face that direction
        mv.setFacingDirection(randomDirection);

        // Change states
        prepareTimer = prepareDuration;
        slimeState = SlimeState.Preparing;
    }

    private void handleAnimation() {
        // Handle grounded animations
        if (mv.isGrounded()) {
            // If moving or not
            if (Mathf.Abs(body.velocity.x) < 0.1f)
                animationHandler.changeAnimationState(idleAnimation);
            else
                animationHandler.changeAnimationState(walkAnimation);
        }
        else {
            // Handle airborne animations
            if (mv.checkRising()) {
                animationHandler.changeAnimationState(riseAnimation);
            }
            else if (mv.checkFalling()) {
                animationHandler.changeAnimationState(fallAnimation);
            }
        }
    }

    private void handleRetaliation() {
        // Handle any attacker
        if (attacker != null) {
    
            // If ready to be stunned
            if (hitStun) {
                // Make entity not stunnable
                hitStun = false;
                // Add knockback
                displacable.triggerKnockback(400f, 0.25f, attacker.transform.position);
                // Play anim
                animationHandler.changeAnimationState(stunnedAnimation);
                // Start cooldown for another hitstun
                StartCoroutine(hitStunCooldown(1f));
                // Change state
                slimeState = SlimeState.Stunned;
            }
            
            // Reset attacker
            attacker = null;
        }
    }

    private void handleDisplacement() {
        if (displacable.isDisplaced()) {
            // Reset values
            wanderTimer = wanderRate;
            prepareTimer = prepareDuration;
            // Set animation
            animationHandler.changeAnimationState(stunnedAnimation);
            slimeState = SlimeState.Stunned;
        }
    }

    private void bounce(float direction)
    {
        if (mv.isGrounded())
        {
            mv.Walk(direction);
            mv.Jump();
        }
    }
}

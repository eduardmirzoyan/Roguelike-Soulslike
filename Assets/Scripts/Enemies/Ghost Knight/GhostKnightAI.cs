using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
public class GhostKnightAI : EnemyAI
{
    [Header("Components")]
    [SerializeField] private PathfindUser pathfindUser;
    [SerializeField] private ParticleSystem sleepingParticles;

    [Header("Animation")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string deadAnimation = "Dead";
    [SerializeField] private string attackAnimation = "Attack";
    [SerializeField] private string sleepAnimation = "Sleep";

    private enum KnightState {
        Sleeping,
        Searching,
        Attacking,
        Dead
    }
    [SerializeField] private KnightState knightState;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // Get required components
        pathfindUser = GetComponent<PathfindUser>();
        sleepingParticles = GetComponent<ParticleSystem>();
        pathfindUser = GetComponent<PathfindUser>();

        sleep();
        // Set starting state
        knightState = KnightState.Sleeping;
    }

    private void Update() {
        if (knightState != KnightState.Dead && health.isEmpty())
        {
            // Call it's death method
            Die();

            // Destroy body in 1 second
            Destroy(gameObject, 1f);

            // Prevent movement
            mv.Walk(0);

            // Change state to dead
            knightState = KnightState.Dead;
        }
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        // If enemy is killed, then set to dead
        switch(knightState) {
            case KnightState.Sleeping:
                animationHandler.changeAnimationState(sleepAnimation);

                // Don't move while sleeping
                mv.Walk(0);

                searchForEnemies();
                
                handleRetaliation();

                // If an enemy has been found, change states
                if (target != null) {
                    pathfindUser.setPathTo(target.position);
                    awaken();

                    knightState = KnightState.Searching;
                    return;
                }

            break;
            case KnightState.Searching:
                animationHandler.changeAnimationState(idleAnimation);
                
                // If target is removed during travel, then cancel path
                if (target == null) {

                    pathfindUser.stopTraveling();
                    knightState = KnightState.Sleeping;
                    return;
                }
                
                // Move towards target
                pathfindUser.moveToLocation();
                
                if (Vector2.Distance(transform.position, target.position) < attackRange) {

                    // Stop moving
                    pathfindUser.stopTraveling();

                    // Change to Attack State!
                    knightState = KnightState.Attacking;
                    return;
                }

                // If target goes too far
                if (Vector2.Distance(target.position, transform.position) > aggroRange) {
                    target = null;
                    
                    sleep();

                    // Go back sleep state
                    knightState = KnightState.Sleeping;
                    return;
                }

                // If entity has reached the end of the path and has not reacted to target, then go back to being idle
                if (pathfindUser.isDonePathing()) {
                    target = null;
                    sleep();

                    knightState = KnightState.Sleeping;
                    return;
                }

            break;
            case KnightState.Attacking:

                // If target is gone (IE dead)
                if (target == null) {
                    sleep();

                    // Go back sleep state
                    knightState = KnightState.Sleeping;
                    return;
                }
                
                // If target goes too far
                if (Vector2.Distance(target.position, transform.position) > aggroRange) {
                    target = null;

                    sleep();

                    // Go back sleep state
                    knightState = KnightState.Sleeping;
                    return;
                }

                // If you are in the middle of an attack, then let it play
                if (attackTimer > 0) {
                    animationHandler.changeAnimationState(attackAnimation);

                    attackTimer -= Time.deltaTime;
                    if (attackTimer < 1f && attackTimer > 0.7f) {
                        mv.dash(attackDashSpeed, mv.getFacingDirection());
                    }
                    else {
                        mv.Walk(0);
                    }

                }
                else { // Else chase target until it is in range or too far
                    animationHandler.changeAnimationState(idleAnimation);

                    // Always face target
                    faceTarget();

                    // If you have an attack cooldown, then reduce it
                    if (attackCooldownTimer > 0) {
                        attackCooldownTimer -= Time.deltaTime;
                    }

                    // If you are in range
                    if (Vector3.Distance(transform.position, target.position) < attackRange) {
                        // Don't move
                        mv.Walk(0);

                        // Check if cooldown is over
                        if (attackCooldownTimer <= 0) {
                            // Start the attack
                            attack();
                        }
                    }
                    else {
                        handleForwardMovement(mv.getFacingDirection());
                    }
                }
            break;
            case KnightState.Dead:
                animationHandler.changeAnimationState(deadAnimation);
            break;
        }
    }

    private void handleForwardMovement(float direction)
    {
        // Too far
        mv.Walk(direction); // Move toward the player

        // Jump if reached a wall and is grounded
        if (mv.isGrounded() && mv.onWall())
            mv.Jump();
    }

    private void attack() {
        attackTimer = attackDuration;
        attackCooldownTimer = attackCooldown;
    }

    private void searchForEnemies() {
        // Search to see if any player enters range
        var colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, 1 << LayerMask.NameToLayer("Player"));

        if (colliders.Length != 0) {
            // Get closest enemy that meats criteria

            Transform closest = null;
            float shortestDistance = Mathf.Infinity;

            foreach (var collider in colliders) {
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                if (distance < shortestDistance) {
                    // If the collider is a player
                    if (collider.TryGetComponent(out Player player)) {
                        shortestDistance = distance;
                        closest = collider.transform;
                    }
                }
            }

            // If a possible enemy is found, then set it as target
            if (closest != null) {
                target = closest;
            }
        }
    }

    private void awaken() {
        stats.defense = 0;
        sleepingParticles.Stop();
    }

    private void sleep() {
        stats.defense = 100;
        sleepingParticles.Play();
    }

    private void handleRetaliation() {
        // If entity was attacked...
        if (attacker != null) {
            // If the attacker is far, then don't care
            if (Vector2.Distance(transform.position, attacker.position) > aggroRange) {
                attacker = null;
                return;
            }

            // Set target to the attacker
            target = attacker;

            // Find path to attacker
            pathfindUser.setPathTo(target.position);
            knightState = KnightState.Searching;

            // Reset attacker
            attacker = null;
        }
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject, 3f);
        knightState = KnightState.Dead;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
public class GoblinAI : EnemyAI
{
    [Header("Goblin Components")]
    [SerializeField] private PathfindUser pathfindUser;

    [Header("Animation")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walk";
    [SerializeField] private string deadAnimation = "Dead";
    [SerializeField] private string stunnedAnimation = "Stunned";
    [SerializeField] private string attackAnimation = "Attack";

    private enum GoblinState {
        Idle,
        Searching,
        Attacking,
        Stunned,
        Dead
    }
    [SerializeField] private GoblinState goblinState;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Get required components
        pathfindUser = GetComponent<PathfindUser>();

        // Set starting state
        goblinState = GoblinState.Idle;
    }

    private void Update() {
        if (goblinState != GoblinState.Dead && health.isEmpty())
        {
            // Call it's death method
            Die();

            // Destroy body in 3 second(s)
            Destroy(gameObject, 3f);

            // Prevent movement
            mv.Walk(0);

            // Add knockback to corpse
            if (target != null)
                displacable.triggerKnockback(400f, 2f, target.position);

            // Change state to dead
            goblinState = GoblinState.Dead;
        }
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        switch(goblinState) {
            case GoblinState.Idle:
                // Set animation based on movement
                if (Mathf.Abs(body.velocity.x) < 0.1f)
                    animationHandler.changeAnimationState(idleAnimation);
                else
                    animationHandler.changeAnimationState(walkAnimation);

                searchForEnemies();

                // If an enemy has been gound, change states
                if (target != null) {

                    // Find path to attacker
                    pathfindUser.setPathTo(target.position);

                    // Change state
                    goblinState = GoblinState.Searching;
                    return;
                }

                wander();

                // Handle any displacement
                if (displacable.isDisplaced()) {
                    // Reset values
                    wanderTimer = wanderRate;
                    attackTimer = attackDuration;
                    attackCooldownTimer = attackCooldown;
                    animationHandler.changeAnimationState(stunnedAnimation);
                    goblinState = GoblinState.Stunned;
                    break;
                }

                handleRetaliation();

            break;
            case GoblinState.Searching:
                // Set animation based on movement
                if (Mathf.Abs(body.velocity.x) < 0.1f)
                    animationHandler.changeAnimationState(idleAnimation);
                else
                    animationHandler.changeAnimationState(walkAnimation);

                // If target is removed during travel, then cancel path
                if (target == null) {
                    pathfindUser.stopTraveling();
                    goblinState = GoblinState.Idle;
                    return;
                }
                
                // Move towards target
                pathfindUser.moveToLocation();
                
                if (Vector2.Distance(transform.position, target.position) < attackRange) {
                    // Stop moving
                    pathfindUser.stopTraveling();

                    // Change to Attack State!
                    goblinState = GoblinState.Attacking;
                    return;
                }

                // If entity has reached the end of the path and has not reacted to target, then go back to being idle
                if (pathfindUser.isDonePathing()) {
                    target = null;
                    goblinState = GoblinState.Idle;
                    return;
                }

                // Handle any displacement
                if (displacable.isDisplaced()) {
                     print("REAL DISPALCE");
                    // Reset values
                    wanderTimer = wanderRate;
                    attackTimer = attackDuration;
                    attackCooldownTimer = attackCooldown;
                    animationHandler.changeAnimationState(stunnedAnimation);
                    goblinState = GoblinState.Stunned;
                    break;
                }

                handleRetaliation();

            break;
            case GoblinState.Attacking:
                // If you are in the middle of an attack, then let it play
                if (attackTimer > 0) {
                    animationHandler.changeAnimationState(attackAnimation);
                    
                    attackTimer -= Time.deltaTime;
                    if (attackTimer < attackDuration / 2) {
                        mv.dash(attackDashSpeed, mv.getFacingDirection());
                    }

                }
                else { // Else chase target until it is in range or too far

                    // If target is gone (IE dead)
                    if (target == null) {
                        // Go back to search state
                        goblinState = GoblinState.Idle;
                        return;
                    }

                    // If you get farther than aggro range, remove target
                    if (Vector2.Distance(transform.position, target.position) > aggroRange) {
                        target = null;
                        goblinState = GoblinState.Idle;
                        return;
                    }

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

                        // Change animation
                        animationHandler.changeAnimationState(idleAnimation);

                        // Check if cooldown is over
                        if (attackCooldownTimer <= 0) {
                            // Start the attack
                            attack();
                        }
                    }
                    else {
                        animationHandler.changeAnimationState(walkAnimation);
                        handleForwardMovement(mv.getFacingDirection());
                    }

                    // Handle any displacement
                    if (displacable.isDisplaced()) {
                        // Reset values
                        wanderTimer = wanderRate;
                        attackTimer = attackDuration;
                        attackCooldownTimer = attackCooldown;
                        animationHandler.changeAnimationState(stunnedAnimation);
                        goblinState = GoblinState.Stunned;
                        break;
                    }

                    handleRetaliation();
                }
            break;
            case GoblinState.Stunned:
                // Any addition knockbacks are incoperated here
                displacable.performDisplacement();

                if (!displacable.isDisplaced()) {
                    
                    // Set to attacking
                    goblinState = GoblinState.Attacking;
                }
                
            break;
            case GoblinState.Dead:
                animationHandler.changeAnimationState(deadAnimation);

                displacable.performDisplacement();
                
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
        // Aggressive, hunts any enemy in sight
        var colliders = lineOfSight.getAllEnemiesInSight(aggroRange);

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
                pathfindUser.setPathTo(target.position);
            }
        }
    }

    private void wander() {
        // If the thief is not already going somewhere, find a new spot
        if (pathfindUser.isDonePathing()) {
            // Stop moving
            mv.Walk(0);
            
            // If you are on cooldown, then skip this frame
            if (wanderTimer > 0 || !mv.isGrounded()) {
                wanderTimer -= Time.deltaTime;
                return;
            }
            
            Vector3 randomPoint = getRandomPointInRadius(wanderRadius, 0, true);
            
            pathfindUser.setPathTo(randomPoint);
            wanderTimer = wanderRate;
        }
        else {
            // If you are already on the path
            
            pathfindUser.moveToLocation();
        }
    }

    private Vector2 getRandomPointInRadius(float maxRadius, float minRadius, bool mustSee) {
        Assert.IsTrue(maxRadius > minRadius);

        var allPossibleCells = pathfindUser.getAllOpenTiles(transform.position, (int)maxRadius);

        while (allPossibleCells.Count > 0) {
            int randomIndex = Random.Range(0, allPossibleCells.Count);
            var randomPoint = allPossibleCells[randomIndex];

            if (!mustSee || lineOfSight.canSeePoint(randomPoint)) {
                // Cast from the random point down to the ground
                var hit = Physics2D.Raycast(randomPoint, Vector2.down, 100f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Platform"));
                if (hit) {
                    // Displace hit location up to make sure it's not inside the ground
                    hit.point += Vector2.up * 0.3f;

                    var distance = Vector2.Distance(transform.position, hit.point);
                    // Make sure that the raycast hit is within radius
                    if (distance <= maxRadius && distance >= minRadius) {
                        return hit.point;
                    }
                }
            }

            // Else remove that cell, and try another one
            allPossibleCells.RemoveAt(randomIndex);
        }

        print("no valid location was found");
        // Return 0 vector if all else fails
        return Vector2.zero;
    }

    private void handleRetaliation() {
        // Handle any attacker
        if (attacker != null) {
            
            // If the entity did not have a target before, then set new target and go after it
            if (target == null)  {
                // Set target to the attacker
                target = attacker;
            }
            
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
                goblinState = GoblinState.Stunned;
            }
            
            // Reset attacker
            attacker = null;
        }
    }
    
}

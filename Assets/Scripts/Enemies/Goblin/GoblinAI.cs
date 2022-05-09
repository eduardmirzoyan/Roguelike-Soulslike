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
    [SerializeField] private float reCalculateDelay = 0.5f;

    private float calculateTimer;

    [Header("Goblin Settings")]
    [SerializeField] private Transform currentCamp;
    [SerializeField] private Transform targetCamp;
    [SerializeField] private float campSiteRange;
    [SerializeField] private float patrolMultiplier;

    [Header("Animation")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walk";
    [SerializeField] private string deadAnimation = "Dead";
    [SerializeField] private string stunnedAnimation = "Stunned";
    [SerializeField] private string attackAnimation = "Attack";

    private enum GoblinState {
        Idle,
        Patroling,
        AtCamp,
        Chasing,
        Attacking,
        Stunned,
        Dead
    }
    [SerializeField] private GoblinState goblinState;

    protected override void Awake()
    {
        base.Awake();
        // Get required components
        pathfindUser = GetComponent<PathfindUser>();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        // Go to nearest camp
        goToNearestCamp();

        // Reset cooldown
        attackCooldownTimer = attackCooldown;

        // Set starting state
        goblinState = GoblinState.Patroling;
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

            // Spawn particles
            if (TryGetComponent(out DamageParticles damageParticles)) {
                damageParticles.spawnDeathParticles();
            }

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
                if (Mathf.Abs(body.velocity.x) < 0.05f)
                    animationHandler.changeAnimationState(idleAnimation);
                else
                    animationHandler.changeAnimationState(walkAnimation);

                // Search for targets
                searchForEnemies();

                // Retaliate on being hit
                handleRetaliation();
                
                // If target was found then chase
                if (target != null) {
                    pathfindUser.setPathTo(target.position);
                    stats.movespeedMultiplier = 0;
                    enemyUI.enableIndicator(GameManager.instance.aggroIndicatorSprite);
                    goblinState = GoblinState.Chasing;
                    break;
                }

                if (wanderTimer > 0) {
                    mv.Walk(0);
                    wanderTimer -= Time.deltaTime;
                }
                else {
                    // If no target, then go to camp
                    if (targetCamp == null) {
                        goToNearestCamp();
                        stats.movespeedMultiplier = 0;
                        goblinState = GoblinState.Patroling;
                        break;
                    }
                }
                
                // Handle any displacement
                if (displacable.isDisplaced()) {
                    if (displacable.isStunned()) {
                        // Reset values
                        resetValues();
                    }
                    
                    animationHandler.changeAnimationState(stunnedAnimation);
                    goblinState = GoblinState.Stunned;
                    break;
                }

            break;
            case GoblinState.Patroling:
                // Set animation based on movement
                if (Mathf.Abs(body.velocity.x) < 0.1f)
                    animationHandler.changeAnimationState(idleAnimation);
                else
                    animationHandler.changeAnimationState(walkAnimation);

                // If you have a camp to go to, then go to it
                if (targetCamp != null) {
                    // Move towards target
                    pathfindUser.moveToLocation(1 + stats.movespeedMultiplier);

                    // If you arrived within campsite range
                    if (Vector2.Distance(transform.position, targetCamp.position) < campSiteRange) {
                        // Stop moving
                        pathfindUser.stopTraveling();

                        // Leave current camp if possible
                        if (currentCamp != null && currentCamp.TryGetComponent(out GoblinCamp campToLeave)) {
                            campToLeave.leaveCamp(this);
                            currentCamp = null;
                        }

                        // Join new camp
                        if (targetCamp.TryGetComponent(out GoblinCamp campToJoin)) {
                            campToJoin.joinCamp(this);
                        }

                        // Set current camp
                        currentCamp = targetCamp;

                        // Remove target camp
                        targetCamp = null;

                        goblinState = GoblinState.AtCamp;
                        break;
                    }

                    // Still search for enemies
                    searchForEnemies();

                    
                }
                else { // If no camp target, then go back to idle
                    stats.movespeedMultiplier = 0f;
                    goblinState = GoblinState.Idle;
                }

                // Handle being attacked
                handleRetaliation();

                // If you have an enemy target, then switch to chasing it
                if (target != null) {
                    // Leave camp
                    if (currentCamp != null && currentCamp.TryGetComponent(out GoblinCamp camp)) {
                        camp.leaveCamp(this);
                        currentCamp = null;
                    }
                    // Remove target camp
                    targetCamp = null;

                    pathfindUser.setPathTo(target.position);
                    stats.movespeedMultiplier = 0f;
                    enemyUI.enableIndicator(GameManager.instance.aggroIndicatorSprite);
                    calculateTimer = reCalculateDelay;
                    goblinState = GoblinState.Chasing;
                    break;
                }

                // Handle any displacement
                if (displacable.isDisplaced()) {
                    if (displacable.isStunned()) {
                        // Reset values
                        resetValues();
                    }

                    animationHandler.changeAnimationState(stunnedAnimation);
                    goblinState = GoblinState.Stunned;
                    break;
                }

            break;
            case GoblinState.AtCamp:
                // Set animation based on movement
                if (Mathf.Abs(body.velocity.x) < 0.1f)
                    animationHandler.changeAnimationState(idleAnimation);
                else
                    animationHandler.changeAnimationState(walkAnimation);


                if (currentCamp != null) {
                    // Else wander until camp sets target
                    wanderAtCamp();

                    // If assigned a new camp then patrol to it
                    if (targetCamp != null) {
                        // Set path
                        pathfindUser.setPathTo(targetCamp.position);
                        // Set patrol speed
                        stats.movespeedMultiplier = -0.5f;
                        // Change states
                        goblinState = GoblinState.Patroling;
                        break;
                    }
                }
                else { // If you are not at a current camp, then go back to idle 
                    goblinState = GoblinState.Idle;
                    break;
                }

                // Still search for enemies
                searchForEnemies();

                // Handle any enemies attacking
                handleRetaliation();

                // If enemy was found, then chase
                if (target != null) {
                    
                    // Leave camp
                    if (currentCamp != null && currentCamp.TryGetComponent(out GoblinCamp camp)) {
                        camp.leaveCamp(this);
                        currentCamp = null;
                    }
                    // Remove target camp
                    targetCamp = null;

                    // Set path
                    pathfindUser.setPathTo(target.position);
                    // Set patrol speed
                    stats.movespeedMultiplier = 0;
                    // Change states
                    goblinState = GoblinState.Chasing;
                    // Set visual indicator
                    enemyUI.enableIndicator(GameManager.instance.aggroIndicatorSprite);
                    calculateTimer = reCalculateDelay;
                    break;
                }

                // Handle any displacement
                if (displacable.isDisplaced()) {
                    if (displacable.isStunned()) {
                        // Reset values
                        resetValues();
                    }

                    animationHandler.changeAnimationState(stunnedAnimation);
                    goblinState = GoblinState.Stunned;
                    break;
                }

            break;
            case GoblinState.Chasing:
                // Set animation based on movement
                if (Mathf.Abs(body.velocity.x) < 0.1f)
                    animationHandler.changeAnimationState(idleAnimation);
                else
                    animationHandler.changeAnimationState(walkAnimation);
                
                // If target is gone
                if (target == null) {
                    goblinState = GoblinState.Idle;
                    break;
                }

                // If target is too far
                if (Vector2.Distance(target.transform.position, transform.position) > deAggroRange) {
                    target = null;
                    enemyUI.enableIndicator(GameManager.instance.deaggroIndicatorSprite);
                    wanderTimer = wanderRate;
                    goblinState = GoblinState.Idle;
                    break;
                }

                

                // Reduce attack timer
                if (attackCooldownTimer > 0) {
                    attackCooldownTimer -= Time.deltaTime;
                }

                // If you go within attack range and can attack
                if (Vector2.Distance(transform.position, target.position) < attackRange) {
                    // Face target
                    faceTarget();

                    // Stop moving
                    mv.Walk(0);

                    // If time to attack, then attack
                    if (attackCooldownTimer <= 0) {
                        // Check if you are on the same y-level
                        var hits = Physics2D.RaycastAll(transform.position, mv.getFacingDirection() * Vector2.right, attackRange);
                        foreach (var hit in hits) {
                            // If the target is on the same level, then attack
                            if (hit.transform == target.transform) {
                                // Stop moving
                                pathfindUser.stopTraveling();

                                // Begin attack
                                attack();
                                
                                // Change to Attack State!
                                goblinState = GoblinState.Attacking;
                                return;
                            }
                        }
                    }
                }
                else {
                    // Move towards target
                    pathfindUser.moveToLocation(1 + stats.movespeedMultiplier);
                }

                // Handle any displacement
                if (displacable.isDisplaced()) {
                    if (displacable.isStunned()) {
                        // Reset values
                        resetValues();
                    }

                    animationHandler.changeAnimationState(stunnedAnimation);
                    goblinState = GoblinState.Stunned;
                    break;
                }

                // Count down
                if (calculateTimer > 0) {
                    calculateTimer -= Time.deltaTime;
                }
                else {
                    // Generate new path if grounded
                    if (mv.isGrounded()) {
                        // If target can move and is grounded, then you make recalculate
                        if (target.TryGetComponent(out Movement movement)) {
                            if (movement.isGrounded()) {
                                pathfindUser.setPathTo(target.position);
                                calculateTimer = reCalculateDelay;
                            }
                            // else dont
                        }
                        else {
                            pathfindUser.setPathTo(target.position);
                            calculateTimer = reCalculateDelay;
                        }
                        
                    }   
                }

            break;
            case GoblinState.Attacking:

                // If you are in the middle of an attack, then let it play out
                if (attackTimer > 0) {
                    animationHandler.changeAnimationState(attackAnimation);
                    
                    attackTimer -= Time.deltaTime;
                    if (attackTimer < attackDuration / 2) {
                        mv.dash(attackDashSpeed, mv.getFacingDirection());
                    }
                }
                else {
                    mv.Walk(0);
                    goblinState = GoblinState.Chasing;
                }
                // else { // Else chase target until it is in range or too far

                //     // If target is gone (IE dead)
                //     if (target == null) {
                //         mv.Walk(0);
                //         // Go back to search state
                //         goblinState = GoblinState.Idle;
                //         return;
                //     }

                //     // If you get farther than aggro range, remove target
                //     if (Vector2.Distance(transform.position, target.position) > aggroRange + 1) {
                //         target = null;
                //         wanderTimer = wanderRate;
                //         enemyUI.enableIndicator(GameManager.instance.deaggroIndicatorSprite);
                //         goblinState = GoblinState.Idle;
                //         return;
                //     }

                //     // Always face target
                //     faceTarget();

                //     // If you have an attack cooldown, then reduce it
                //     if (attackCooldownTimer > 0) {
                //         attackCooldownTimer -= Time.deltaTime;
                //     }

                //     // If you are in range
                //     if (Vector3.Distance(transform.position, target.position) < attackRange) {
                //         // Don't move
                //         mv.Walk(0);

                //         // Change animation
                //         animationHandler.changeAnimationState(idleAnimation);

                //         // Check if cooldown is over
                //         if (attackCooldownTimer <= 0) {
                //             // Raycast ahead
                //             var hits = Physics2D.RaycastAll(transform.position, mv.getFacingDirection() * Vector2.right, attackRange);
                //             foreach (var hit in hits) {
                //                 // If the target is on the same level, then attack
                //                 if (hit.transform == target.transform) {
                //                     // Start the attack
                //                     attack();
                //                     return;
                //                 }
                //             }
                //         }
                //     }
                //     else {
                //         animationHandler.changeAnimationState(walkAnimation);
                //         handleForwardMovement(mv.getFacingDirection());
                //     }
                // }
                
                // Handle any displacement
                if (displacable.isDisplaced()) {
                    if (attackTimer > 0 || displacable.isStunned()) {
                        // Reset values
                        resetValues();
                    }

                    animationHandler.changeAnimationState(stunnedAnimation);
                    goblinState = GoblinState.Stunned;
                    break;
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
        mv.Walk(direction * (1 + stats.movespeedMultiplier)); // Move toward the player

        // Jump if reached a wall and is grounded
        if (mv.isGrounded() && mv.onWall())
            mv.Jump();
    }

    public void patrolTo(Transform camp) {
        // If the goblin has no target, then go to camp
        if (targetCamp == null) {
            stats.movespeedMultiplier = -0.5f;
            targetCamp = camp;
            pathfindUser.setPathTo(targetCamp.position);
        }
    }

    private void goToNearestCamp() {
        var goblinCamps = GameObject.FindObjectsOfType<GoblinCamp>();
        if (goblinCamps.Length != 0) {
            // Get closest enemy that meats criteria

            Transform closest = null;
            float shortestDistance = Mathf.Infinity;

            foreach (var camp in goblinCamps) {
                float distance = Vector3.Distance(transform.position, camp.transform.position);

                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    closest = camp.transform;
                }
            }

            // If a possible enemy is found, then set it as target
            if (closest != null) {
                targetCamp = closest;
                pathfindUser.setPathTo(targetCamp.position);
            }
        }
        
    }

    private void attack() {
        attackTimer = attackDuration;
        attackCooldownTimer = attackCooldown;
    }

    private void searchForEnemies() {
        // Aggressive, hunts any enemy in sight
        var colliders = lineOfSight.getAllEnemiesInSight(aggroRange);

        if (colliders.Length != 0) {
            // Get closest enemy that meets criteria

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

    private void wanderAtCamp() {
        // If the thief is not already going somewhere, find a new spot
        if (pathfindUser.isDonePathing()) {
            // Stop moving
            mv.Walk(0);
            
            // If you are on cooldown, then skip this frame
            if (wanderTimer > 0 || !mv.isGrounded()) {
                wanderTimer -= Time.deltaTime;
                return;
            }
            
            // If you are not at a campsite, then dip
            if (currentCamp == null) {
                return;
            }

            Vector3 randomPoint = getRandomPointInRadius(wanderRadius, 0, currentCamp.position, true);
            
            pathfindUser.setPathTo(randomPoint);
            wanderTimer = wanderRate;
        }
        else {
            // If you are wandering, then keep moving
            pathfindUser.moveToLocation();
        }
    }

    private Vector2 getRandomPointInRadius(float maxRadius, float minRadius, Vector2 position, bool mustSee) {
        Assert.IsTrue(maxRadius > minRadius);

        var allPossibleCells = pathfindUser.getAllOpenTiles(position, (int)maxRadius);

        while (allPossibleCells.Count > 0) {
            int randomIndex = Random.Range(0, allPossibleCells.Count);
            var randomPoint = allPossibleCells[randomIndex];

            if (!mustSee || lineOfSight.canSeePoint(randomPoint)) {
                // Cast from the random point down to the ground
                var hit = Physics2D.Raycast(randomPoint, Vector2.down, 100f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Platform"));
                if (hit) {
                    // Displace hit location up to make sure it's not inside the ground
                    hit.point += Vector2.up * 0.3f;

                    var distance = Vector2.Distance(position, hit.point);
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
               
                // Remove camp
                if (currentCamp != null && currentCamp.TryGetComponent(out GoblinCamp camp)) {
                    camp.leaveCamp(this);
                    currentCamp = null;
                }
                
                // Remove target camp
                targetCamp = null;

                // Set movespeed
                stats.movespeedMultiplier = 0f;

                // Set target to the attacker
                target = attacker;
            }
            
            // Reset attacker
            attacker = null;
        }
    }
    
    private void OnDestroy() {
        // Create corpse
        if (health.currentHealth <= 0)
            GameManager.instance.spawnCorpse(gameObject);
    }

    protected override void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        if (targetCamp != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(targetCamp.transform.position, wanderRadius);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, campSiteRange);
    }
}

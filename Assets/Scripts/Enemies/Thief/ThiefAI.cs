using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Rigidbody2D))]
public class ThiefAI : EnemyAI
{
    [Header("Components")]
    [SerializeField] private PathfindUser pathfindUser;
    [SerializeField] private Inventory inventory;

    [Header("Looting Values")]
    [SerializeField] private float maxWanderRadius;
    [SerializeField] private int maxInventorySize = 1;
    [SerializeField] private Image lootingCircle;
    [SerializeField] private Sprite hasLootSprite;
    [SerializeField] private float lootDuration = 1f;
    [SerializeField] private float maxItemDetectionRange = 3;
    [SerializeField] protected WorldItem dropLoot; // REPLACE THIS WITH 'RESOURCE LOADING'

    [Header("Animation")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walk";
    [SerializeField] private string deadAnimation = "Dead";
    [SerializeField] private string attackAnimation = "Attack";
    [SerializeField] private string riseAnimation = "Rise";
    [SerializeField] private string fallAnimation = "Fall";
    [SerializeField] private string lootAnimation = "Loot";
    [SerializeField] private string stunnedAnimation = "Stunned";
    private float lootTimer;

    private enum ThiefState {
        Idle,
        Searching,
        Attacking,
        Looting,
        Stunned,
        Dead
    }

    [SerializeField] private ThiefState thiefState;

    protected override void Start()
    {
        base.Start();
        inventory = GetComponent<Inventory>();
        inventory.setMax(maxInventorySize);
        pathfindUser = GetComponent<PathfindUser>();

        lootingCircle.fillAmount = 0;
        wanderTimer = wanderRate;
        
        // The thief spawns with no item and is aggressive
        thiefState = ThiefState.Idle;
    }

    private void Update() {
        if (thiefState != ThiefState.Dead && health.isEmpty())
        {   
            // Drop held item on death if has one
            if (!inventory.isEmpty()) {
                var prefab = Instantiate(dropLoot, transform.position, Quaternion.identity);
                prefab.setItem(inventory.getItem(0));
            }
            
            // Call it's death method
            Die();

            // Destroy body in 1 second
            Destroy(gameObject, 2f);

            // Prevent movement
            mv.Walk(0);

            // Add knockback to corpse
            if (target != null)
                displacable.triggerKnockback(400f, 2f, target.position);

            // Change state to dead
            thiefState = ThiefState.Dead;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        switch(thiefState) {
            case ThiefState.Idle:
                // Set animations
                handleAnimation();

                // If thief has no item
                if (!hasItem()) {
                    //  Search for loot, if found set targetT
                    searchForItem();
                    
                    // Search for enemies, if found set targetT
                    searchForEnemies();

                    if (target != null) {
                        // Set target
                        pathfindUser.setPathTo(target.position);
                        // Set state
                        thiefState = ThiefState.Searching;
                        return;
                    }
                }
                
                wander();

                // Handle displacement
                if (displacable.isDisplaced()) {
                    // Reset values
                    wanderTimer = wanderRate;
                    attackTimer = attackDuration;
                    lootTimer = lootDuration;
                    attackCooldownTimer = attackCooldown;
                    lootingCircle.fillAmount = 0;
                    animationHandler.changeAnimationState(stunnedAnimation);
                    thiefState = ThiefState.Stunned;
                    break;
                }

                handleRetaliation();

            break;
            case ThiefState.Searching:
                // Set animations
                handleAnimation();

                // If target is removed during travel, then cancel path
                if (target == null) {
                    pathfindUser.stopTraveling();
                    thiefState = ThiefState.Idle;
                    return;
                }
                
                // Move towards target
                pathfindUser.moveToLocation();

                if (target.TryGetComponent(out WorldItem worldItem)) { // If target is an item...

                    searchForEnemies(); // Keep a lookout for enemies

                    if (Vector2.Distance(transform.position, target.position) < 
                        target.GetComponent<Collider2D>().bounds.extents.x + boxCollider2D.bounds.extents.x) { // When you get close to the item, then begin looting
                        lootTimer = lootDuration;
                        pathfindUser.stopTraveling();
                        // Change to Loot state!
                        thiefState = ThiefState.Looting;
                        return;
                    }
                }
                else if (Vector2.Distance(transform.position, target.position) < attackRange) { // If you are within range...
                        pathfindUser.stopTraveling();
                        // Change to Attack State!
                        thiefState = ThiefState.Attacking;
                        return;
                    }
                
                if (pathfindUser.isDonePathing()) {
                    target = null;
                    thiefState = ThiefState.Idle;
                    return;
                }

                // Handle displacement
                if (displacable.isDisplaced()) {
                    // Reset values
                    wanderTimer = wanderRate;
                    attackTimer = attackDuration;
                    lootTimer = lootDuration;
                    attackCooldownTimer = attackCooldown;
                    lootingCircle.fillAmount = 0;
                    animationHandler.changeAnimationState(stunnedAnimation);
                    thiefState = ThiefState.Stunned;
                    break;
                }

                handleRetaliation();

            break;
            case ThiefState.Attacking:
                // Attack State

                // If you are in the middle of an attack, then let it play
                if (attackTimer > 0) {
                    attackTimer -= Time.deltaTime;
                    if (attackTimer < attackDuration / 2) {
                        mv.dash(attackDashSpeed, mv.getFacingDirection());
                    }
                    // Playout attack animation
                }
                else { 
                    // Else chase target until it is in range or too far

                    // If target is gone (IE dead)
                    if (target == null) {
                        // Go back to search state
                        thiefState = ThiefState.Idle;
                        return;
                    }

                    // If you get farther than aggro range, remove target
                    if (Vector2.Distance(transform.position, target.position) > aggroRange) {
                        target = null;
                        thiefState = ThiefState.Idle;
                        return;
                    }

                    // Always face target
                    faceTarget();

                    // Cooldown between attacks
                    if (attackCooldownTimer > 0) {
                        attackCooldownTimer -= Time.deltaTime;
                    }

                    // If you are in range
                    if (Vector3.Distance(transform.position, target.position) < attackRange) {
                        //print("dile");
                        // Attack!
                        mv.Walk(0);

                        // Set animation
                        animationHandler.changeAnimationState(idleAnimation);

                        // If the cooldown between attacks is over, then start new attack
                        if (attackCooldownTimer <= 0) {
                            // Start the attack
                            attack();
                        }
                    }
                    else {
                        //print("walke");
                        animationHandler.changeAnimationState(walkAnimation);
                        handleForwardMovement(mv.getFacingDirection());
                    }

                    // Handle displacement
                    if (displacable.isDisplaced()) {
                        // Reset values
                        wanderTimer = wanderRate;
                        attackTimer = attackDuration;
                        lootTimer = lootDuration;
                        attackCooldownTimer = attackCooldown;
                        lootingCircle.fillAmount = 0;
                        animationHandler.changeAnimationState(stunnedAnimation);
                        thiefState = ThiefState.Stunned;
                        break;
                    }

                    handleRetaliation();
                }

            break;
            case ThiefState.Looting:
                // Loot State
                if (target == null) {
                    lootingCircle.fillAmount = 0;
                    thiefState = ThiefState.Idle;
                }

                // Always face target
                faceTarget();

                // Loot animation!
                animationHandler.changeAnimationState(lootAnimation);

                // Loot timer!
                if (lootTimer > 0) {
                    // Fill looting circle based on timer
                    lootingCircle.fillAmount = 1 - lootTimer / lootDuration;
                    lootTimer -= Time.deltaTime;
                }
                else {
                    lootItem();
                    target = null;
                    thiefState = ThiefState.Idle;
                }

                // Handle displacement
                if (displacable.isDisplaced()) {
                    // Reset values
                    wanderTimer = wanderRate;
                    attackTimer = attackDuration;
                    lootTimer = lootDuration;
                    attackCooldownTimer = attackCooldown;
                    lootingCircle.fillAmount = 0;
                    animationHandler.changeAnimationState(stunnedAnimation);
                    thiefState = ThiefState.Stunned;
                    break;
                }

                handleRetaliation();

            break;
            case ThiefState.Stunned:

                displacable.performDisplacement();

                if (!displacable.isDisplaced()) {
                    // Stop walking
                    mv.Walk(0);
                    // Change to attacking state
                    thiefState = ThiefState.Attacking;
                    break;
                }

            break;
            case ThiefState.Dead:
                // Do nothin
                animationHandler.changeAnimationState(deadAnimation);
                displacable.performDisplacement();

            break;
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
            
            Vector3 randomPoint = getRandomPointInRadius(wanderRadius + 3, wanderRadius, false);
            
            pathfindUser.setPathTo(randomPoint);
            wanderTimer = wanderRate;
        }
        else {
            pathfindUser.moveToLocation();
        }
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
        // If entity was attacked...
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
                thiefState = ThiefState.Stunned;
            }

            // Reset attacker
            attacker = null;
        }
    }

    private void handleForwardMovement(float direction)
    {
        mv.Walk(direction); // Move toward the player

        // Jump if reached a wall and is grounded
        if (mv.isGrounded() && mv.onWall())
            mv.Jump();
    }

    private void attack() {
        animationHandler.changeAnimationState(attackAnimation);
        mv.Walk(0);
        attackTimer = attackDuration;
        attackCooldownTimer = attackCooldown;
    }

    private void searchForItem() {
        // Circle cast and capture all items in radius
        var hits = Physics2D.OverlapCircleAll(transform.position, maxItemDetectionRange, 1 << LayerMask.NameToLayer("Loot"));
        
        // If you hit nothing, then leave
        if (hits.Length == 0)
            return;

        // Store the closest item that was captured
        WorldItem closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (var hit in hits) {
            Vector3 diff = hit.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < minDistance && hit.TryGetComponent(out WorldItem worldItem))
            {
                closest = worldItem;
                minDistance = curDistance;
            }
        }
        
        if (closest != null) {
            // Begin to path to the item here
            target = closest.transform;
            pathfindUser.setPathTo(target.position);
        }
    }

    public bool hasItem() => inventory.isFull();

    private void lootItem() {
        var hit = Physics2D.OverlapBox(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0, 1 << LayerMask.NameToLayer("Loot"));
        // If an item is found then begin looting process
        if (hit)
        {
            // Add the item to inventory
            if (hit.TryGetComponent(out WorldItem worldItem)) {
                inventory.addItem(worldItem.GetItem());
                Destroy(worldItem.gameObject);
                lootingCircle.sprite = hasLootSprite;

                // Buff thief
                var weapon = GetComponentInChildren<EnemyWeapon>();
                if (weapon != null) {
                    // Increase damage by 25%
                    weapon.setDamage((int) (weapon.getDamage() * 0.25f));

                    // Increase movespeed by 25%
                    mv.setMoveSpeed(mv.getMovespeed() * 1.25f);
                }
            }
            else
                print("Item doesn't exist here");
        }
        else
            print("no items found");
    }

    private void searchForEnemies() {
        // If thief does not have an item, then he should be aggressive and hunt targets
        var colliders = lineOfSight.getAllEnemiesInSight(aggroRange);

        if (colliders.Length != 0) {
            // Get closest enemy that meats criteria

            Transform closest = null;
            float shortestDistance = Mathf.Infinity;

            foreach (var collider in colliders) {
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                if (distance < shortestDistance) {
                    // Only consider thieves with items
                    if (collider.TryGetComponent(out ThiefAI thiefAI) && thiefAI.hasItem()) {
                        shortestDistance = distance;
                        closest = collider.transform;
                    }
                    // Only go after the player if they have an item in their inventory
                    else if (collider.TryGetComponent(out Player player)) {
                        var playerInventory = player.GetComponentInChildren<Inventory>();
                        if (playerInventory != null && !playerInventory.isEmpty()) {
                            shortestDistance = distance;
                            closest = collider.transform;
                        }
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

    protected override void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, maxItemDetectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxWanderRadius);
    }
}

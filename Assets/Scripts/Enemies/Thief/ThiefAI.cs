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
    [SerializeField] private float lootTime = 1f;
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
    private float lootTimer;

    private enum ThiefState {
        Idle,
        Searching,
        Attacking,
        Looting,
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
        
         // The thief spawns with no item and is aggressive
        thiefState = ThiefState.Idle;
    }

    private void Update() {
        if (health.isEmpty()) {
            thiefState = ThiefState.Dead;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        switch(thiefState) {
            case ThiefState.Idle:
                // If thief has no item
                if (!hasItem()) {
                    //  Search for loot, if found set targetT
                    searchForItem();
                    
                    // Search for enemies, if found set targetT
                    searchForEnemies();

                    if (target != null) {
                        thiefState = ThiefState.Searching;
                        return;
                    }
                }
                
                wander();

                handleRetaliation();

            break;
            case ThiefState.Searching:
                // Search state

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
                        lootTimer = lootTime;
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

                // Handle airborne animations
                if (mv.checkRising()) {
                    animationHandler.changeAnimationState(riseAnimation);
                }
                else if (mv.checkFalling()) {
                    animationHandler.changeAnimationState(fallAnimation);
                }
                
                handleRetaliation();

            break;
            case ThiefState.Attacking:
                // Attack State

                // If you are in the middle of an attack, then let it play
                if (attackTimer > 0) {
                    attackTimer -= Time.deltaTime;
                    // Playout attack animation
                }
                else { 
                    // Else chase target until it is in range or too far

                    // Always face target
                    faceTarget();

                    // If you get farther than aggro range, remove target
                    if (Vector2.Distance(transform.position, target.position) > aggroRange) {
                        target = null;
                    }

                    // If target is gone (IE dead)
                    if (target == null) {
                        // Go back to search state
                        thiefState = ThiefState.Idle;
                        return;
                    }

                    // Cooldown between attacks
                    if (attackCooldownTimer > 0) {
                        attackCooldownTimer -= Time.deltaTime;
                    }

                    // If you are in range
                    if (Vector3.Distance(transform.position, target.position) < attackRange) {
                        // Attack!
                        mv.Walk(0);
                        animationHandler.changeAnimationState(idleAnimation);

                        // If the cooldown between attacks is over, then start new attack
                        if (attackCooldownTimer <= 0) {
                            // Start the attack
                            attack();
                        }
                    }
                    else {
                        animationHandler.changeAnimationState(walkAnimation);
                        handleForwardMovement(mv.getFacingDirection());
                    }
                }
            break;
            case ThiefState.Looting:
                // Loot State

                // Always face target
                faceTarget();

                // Loot animation!
                animationHandler.changeAnimationState(lootAnimation);

                // Loot timer!
                if (lootTimer > 0) {
                    // Fill looting circle based on timer
                    lootingCircle.fillAmount = 1 - lootTimer / lootTime;
                    lootTimer -= Time.deltaTime;
                }
                else {
                    lootItem();
                    target = null;
                    thiefState = ThiefState.Idle;
                }

                handleRetaliation();
            break;
            case ThiefState.Dead:
                // Do nothin
                animationHandler.changeAnimationState(deadAnimation);

                // Drop held item on death
                var prefab = Instantiate(dropLoot, transform.position, Quaternion.identity);
                prefab.setItem(inventory.getItem(0));

                // Destroy itself
                Destroy(gameObject);

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

            // Change animation
            animationHandler.changeAnimationState(idleAnimation);

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
            animationHandler.changeAnimationState(walkAnimation);

            pathfindUser.moveToLocation();
        }
    }

    private void handleRetaliation() {
        // If entity was attacked...
        if (attacker != null) {
            // And if entity has no target or is going after an item
            if (target == null || target.TryGetComponent(out WorldItem worldItem)) {
                // If the attacker is a player or thief
                target = attacker;
                pathfindUser.setPathTo(target.position);
                thiefState = ThiefState.Searching;
            }
            attacker = null;
        }
    }

    private void handleForwardMovement(float direction)
    {
        animationHandler.changeAnimationState(walkAnimation);
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
            }
            else
                print("Item doesn't exist here");
        }
        else
            print("no items found");
    }

    private void searchForEnemies() {
        // If thief does not have an item, then he should be aggressive and hunt targets
        var colliders = lineOfSight.getAllEnemiesInSight();

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

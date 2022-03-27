using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Rigidbody2D))]
public class ThiefAI : EnemyAI
{
    [Header("Pathfinding")]
    [SerializeField] private PathfindUser pathfindUser;

    [Header("Thief Stats")]
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private Health health;
    [SerializeField] private Inventory inventory;
    [SerializeField] private int maxInventorySize = 1;
    [SerializeField] private float wanderRadius = 2f;
    [SerializeField] private float wanderRate = 1f;

    [Header("Looting Values")]
    [SerializeField] private LayerMask itemMask;
    [SerializeField] private Image lootingCircle;
    [SerializeField] private Sprite hasLootSprite;
    [SerializeField] private float lootTime = 1f;
    [SerializeField] private float maxItemDetectionRange = 3;
    [SerializeField] protected WorldItem dropLoot; // REPLACE THIS WITH 'RESOURCE LOADING'

    // Private fields
    private float lootTimer;
    
    private float attackDuration;
    [SerializeField] private float wanderTimer;

    private enum ThiefState {
        Idle,
        Searching,
        Attacking,
        Looting,
        Dead
    }

    [SerializeField] private ThiefState thiefState;

    [SerializeField] private Vector3 randomPoint;
    // Movement States: Idle, Moving, Jumping

    // Emotional States: Passive, Neutral, Aggro

    // Unique states: Searching, Found, Not found

    // Possible actions: Walking, Idle, Attacking, Getting hit, looting item

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        inventory = GetComponent<Inventory>();
        inventory.setMax(maxInventorySize);

        boxCollider2D = GetComponent<BoxCollider2D>();
        health = GetComponent<Health>();
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
                //animationHandler.changeAnimationState("Idle");

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
                    print("target is gone");
                    pathfindUser.stopTraveling();
                    thiefState = ThiefState.Idle;
                    return;
                }
                
                // Move towards targetT
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
                else if (target.TryGetComponent(out ThiefAI enemy) || target.TryGetComponent(out Player player)) { // If target is enemy, then check to see if you are in attack range
                    if (Vector2.Distance(transform.position, target.position) < maxAttackRange) {
                        pathfindUser.stopTraveling();
                        // Change to Attack State!
                        thiefState = ThiefState.Attacking;
                        return;
                    }
                }
                
                if (pathfindUser.isDonePathing() || target == null) {
                    target = null;
                    thiefState = ThiefState.Idle;
                    return;
                }

                if (mv.checkRising()) {
                    animationHandler.changeAnimationState("Rise");
                }
                else if (mv.checkFalling()) {
                    animationHandler.changeAnimationState("Fall");
                }
                
                handleRetaliation();

            break;
            case ThiefState.Attacking:
                // Attack State

                // If target is gone (IE dead)
                if (target == null) {
                    // Go back to search state
                    thiefState = ThiefState.Idle;
                    return;
                }

                // If you are in the middle of an attack, then let it play
                if (attackDuration > 0) {
                    attackDuration -= Time.deltaTime;
                    // Playout attack animation
                }
                else { // Else chase target until it is in range or too far
                    // Always face target
                    faceTarget();

                    // If you are in range
                    if (Vector3.Distance(transform.position, target.position) < maxAttackRange) {
                        // Attack!
                        attack();
                    }
                    else {
                        handleForwardMovement(mv.getFacingDirection());
                    }
                }
            break;
            case ThiefState.Looting:
                // Loot State

                // Always face target
                faceTarget();

                // Loot animation!
                animationHandler.changeAnimationState("Loot");

                // Loot timer!
                if (lootTimer > 0) {
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
                animationHandler.changeAnimationState("Dead");
                var prefab = Instantiate(dropLoot, transform.position, Quaternion.identity);
                prefab.setItem(inventory.getItem(0));
                Destroy(gameObject);
                
            break;
        }
    }

    private void wander() {
        // If the thief is not already going somewhere, find a new spot
        if (pathfindUser.isDonePathing()) {
            mv.Walk(0);
            animationHandler.changeAnimationState("Idle");
            // If you are on cooldown, then skip this frame
            if (wanderTimer > 0 || !mv.isGrounded()) {
                wanderTimer -= Time.deltaTime;
                return;
            }
            
            // Find a random point not inside the ground and can be seen
            randomPoint = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
            while (!pathfindUser.isPointValid(randomPoint)) {
                
                randomPoint = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
            }
            
            pathfindUser.setPathTo(randomPoint);
            wanderTimer = wanderRate;
        }
        else {
            pathfindUser.moveToLocation();
        }
    }

    private void handleRetaliation() {
        // If entity was attacked...
        if (attacker != null) {
            // And if entity has no target or is going after an item
            if (target == null || target.TryGetComponent(out WorldItem worldItem)) {
                // If the attacker is a player or thief
                if (attacker.TryGetComponent(out ThiefAI thiefAI) || attacker.TryGetComponent(out Player player)) {
                    // Then switch targets to attacker and pathfind
                    target = attacker;
                    pathfindUser.setPathTo(target.position);
                    thiefState = ThiefState.Searching;
                }
            }
            attacker = null;
        }
    }

    private void handleForwardMovement(float direction)
    {
        animationHandler.changeAnimationState("Walk");
        mv.Walk(direction); // Move toward the player

        // Jump if reached a wall and is grounded
        if (jumpEnabled && mv.isGrounded() && mv.onWall())
            mv.Jump();
    }

    private void attack() {
        animationHandler.changeAnimationState("Attack");
        mv.Walk(0);
        attackDuration = animationHandler.getAnimationDuration();
    }

    private void searchForItem() {
        // Circle cast and capture all items in radius
        var hits = Physics2D.OverlapCircleAll(transform.position, maxItemDetectionRange, itemMask);
        
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
        var hit = Physics2D.OverlapBox(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0, itemMask);
        // If an item is found then begin looting process
        if (hit)
        {
            // Add the item to inventory
            var worldItem = hit.GetComponent<WorldItem>();
            if (worldItem != null) {
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

            var mv = GetComponent<Movement>();
            foreach (var collider in colliders) {
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                if (distance < shortestDistance) {
                    // Only consider thieves with items
                    if (collider.TryGetComponent(out ThiefAI thiefAI) && thiefAI.hasItem()) {
                        shortestDistance = distance;
                        closest = collider.transform;
                    }
                    else if (collider.TryGetComponent(out Player player)) {
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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, maxItemDetectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
        
        if (target != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(target.position, 0.4f);
        }

        if (randomPoint != null) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(randomPoint, 0.1f);
        }
    }
}

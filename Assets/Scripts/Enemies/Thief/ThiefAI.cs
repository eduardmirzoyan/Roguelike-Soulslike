using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Rigidbody2D))]
public class ThiefAI : EnemyAI
{
    [SerializeField] private PathfindUser pathfindUser;

    [Header("Thief Stats")]
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private Inventory inventory;
    [SerializeField] private int maxInventorySize = 1;
    [SerializeField] private float maxItemDetectionRange = 3;
    [SerializeField] private LayerMask itemMask;
    
    [SerializeField] private float lootTime;
    private float lootTimer;
    private float attackDuration;
    [SerializeField] private Movement mv;

    [SerializeField] private Transform targetT;

    private enum ThiefState {
        Idle,
        Searching,
        Attacking,
        Looting,
        Dead
    }

    [SerializeField] private ThiefState thiefState;

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
        mv = GetComponent<Movement>();
        pathfindUser = GetComponent<PathfindUser>();

        thiefState = ThiefState.Idle;
        // The thief spawns with no item and is aggressive
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Psudeo code:

        /*
        On spawn: Wander until it finds an item in range or another thief with an item.
        Or until the player comes into range

            If item find, then pathfind to item/entity

            Upon reaching
                If it is an item, begin to loot (takes about 1 second)
                If it is a thief, attack

            After item is looted, show bag icon and empower thief attacks
                Taking an item heals 50%, and increase attack speed/ damage by 25%
            
            Thief without an item is aggro (Attacks player on sight)
            Thief with an item is neutral (Attacks upon being hit)

        */
        
        switch(thiefState) {
            case ThiefState.Idle:
                // If thief has no item
                if (!hasItem()) {
                    //  Search for loot, if found set targetT
                    searchForItem();
                    
                    // Search for enemies, if found set targetT
                    searchForEnemies();

                    if (targetT != null) {
                        pathfindUser.setPathTo(targetT.position);
                        print("found something!");
                        thiefState = ThiefState.Searching;
                        return;
                    }
                }
                else {  // If you do have item, then just vibe
                    // Wander
                }

            break;
            case ThiefState.Searching:
                // Search state

                // Move towards targetT
                pathfindUser.moveToLocation();


                if (targetT.TryGetComponent(out WorldItem worldItem)) { // If target is an item...
                    //searchForEnemies(); // Keep a lookout for enemies

                    if (Vector2.Distance(transform.position, targetT.position) < 
                        targetT.GetComponent<Collider2D>().bounds.extents.x + boxCollider2D.bounds.extents.x) { // When you get close to the item, then begin looting
                        lootTimer = lootTime;
                        pathfindUser.stopTraveling();
                        // Change to Loot state!
                        thiefState = ThiefState.Looting;
                        return;
                    }
                }
                else if (targetT.TryGetComponent(out ThiefAI enemy) || targetT.TryGetComponent(out Player player)) { // If target is enemy, then check to see if you are in attack range
                    if (Vector2.Distance(transform.position, targetT.position) < maxAttackRange) {
                        pathfindUser.stopTraveling();
                        // Change to Attack State!
                        thiefState = ThiefState.Attacking;
                        return;
                    }
                }
                
                if (pathfindUser.isDonePathing() || targetT == null) {
                    targetT = null;
                    thiefState = ThiefState.Idle;
                    return;
                }
                
            break;
            case ThiefState.Attacking:
                // Attack State

                // If target is gone (IE dead)
                if (targetT == null) {
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
                    if (Vector3.Distance(transform.position, targetT.position) < maxAttackRange) {
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
                // animationHandler.changeAnimationState("Loot");

                // Loot timer!
                if (lootTimer > 0) {
                    lootTimer -= Time.deltaTime;
                }
                else {
                    lootItem();
                    targetT = null;
                    thiefState = ThiefState.Idle;
                }
            break;
            case ThiefState.Dead:
            // Do nothin
            break;
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
        mv.Walk(0);
        animationHandler.changeAnimationState("Attack");
        attackDuration = animationHandler.getAnimationDuration();
    }

    protected void faceTarget()
    {
        mv.setFacingDirection(targetT.transform.position.x - transform.position.x);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, maxItemDetectionRange);
        if (targetT != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetT.position, 0.4f);
        }
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
            targetT = closest.transform;
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
            }
            else
                print("Item doesn't exist here");
        }
        else
            print("no items found");
    }

    private void searchForEnemies() {
        // If thief does not have an item, then he should be aggressive and hunt targets
         
        if (targetT == null) {
            // Then get target and pathfind to it, until you get to min attack range
            targetT = lineOfSight.canSeeATarget();
        }
    }
}

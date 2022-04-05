using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomGolemAI : BossAI
{
    [Header("Mushroom Golem Assets")]
    [SerializeField] private GameObject bigShockwave;
    [SerializeField] private GameObject smallShockwave;
    [SerializeField] private GameObject boulder;

    [SerializeField] private Transform frontSpawnPoint;
    [SerializeField] private Transform rearSpawnPoint;
    [SerializeField] private Transform boulderSpawnPoint;

    [Header("Mushroom Golem Attacks")]
    [SerializeField] private string bigSmashAnimation;
    [SerializeField] private string jumpSmashAnimation;
    [SerializeField] private string boulderTossAnimation;
    [SerializeField] private float farAttackRange;
    [SerializeField] private float followRange;

    [Header("Mushroom Golem Animation")]
    [SerializeField] private string sleepAnimation = "Sleep";
    [SerializeField] private string awakenAnimation = "Awaken";

    private bool awake = false;

    private enum MushroomGolemState {
        Asleep,
        Idle,
        Aggro,
        BigSmash,
        JumpSmash,
        BoulderToss,
        Reposition,
        Dead
    }
    [SerializeField] private MushroomGolemState mushroomGolemState; 

    private void FixedUpdate()
    {
        switch(mushroomGolemState) {
            case MushroomGolemState.Asleep:
                
                searchForEnemies();

                // If an enemy is found
                if (!awake && target != null) {
                    awake = true;
                    StartCoroutine(wakeUp(1.5f));
                }

            break;
            case MushroomGolemState.Idle:
                // Handle animations
                handleMovementAnimations();

                // Just vibe out
                searchForEnemies();

                // If an enemy is found
                if (target != null) {
                    // If the target is a player, then show boss health bar
                    if (target.TryGetComponent(out Player player)) {
                        bossHealthBarUI.setBoss(this);
                    }

                    // Reset cooldown
                    attackCooldownTimer = attackCooldown;

                    // Change state
                    mushroomGolemState = MushroomGolemState.Aggro;
                }
            break;
            case MushroomGolemState.Aggro:
                // Always face target
                faceTarget();

                // If you get farther than aggro range, remove target
                if (Vector2.Distance(transform.position, target.position) > aggroRange) {
                    target = null;
                }

                // If target is gone (IE dead)
                if (target == null) {
                    // Go back to search state
                    mushroomGolemState = MushroomGolemState.Idle;
                    return;
                }

                // Cache distance
                var distance = Vector3.Distance(transform.position, target.position);

                // Face target
                faceTarget();

                // If you have an attack cooldown, then reduce it
                if (attackCooldownTimer > 0) {
                    attackCooldownTimer -= Time.deltaTime;

                    // Check whether to walk towards target or not
                    if (distance < followRange) {
                        animationHandler.changeAnimationState(idleAnimation);
                        mv.Walk(0);
                    }
                    else if (distance > attackRange) {
                        // Walk towards enemy
                        animationHandler.changeAnimationState(walkAnimation);
                        handleForwardMovement(mv.getFacingDirection());
                    }
                }
                else { // Cooldown is over

                    // Stop moving
                    animationHandler.changeAnimationState(idleAnimation);

                    mv.Walk(0);

                    if (distance < attackRange) { // Close range
                        // Big smash attack
                        attackTimer = 1.25f;
                        animationHandler.changeAnimationState(bigSmashAnimation);
                        StartCoroutine(shakeTimer(0.667f));
                        mushroomGolemState = MushroomGolemState.BigSmash;
                    }
                    else if (distance > attackRange && distance < farAttackRange) { // Mid range
                        // Toss Boulder attack
                        attackTimer = 1f;
                        animationHandler.changeAnimationState(boulderTossAnimation);
                        mushroomGolemState = MushroomGolemState.BoulderToss;
                    }
                    else if (distance > farAttackRange) { // Far range
                        // Jump smash attack
                        attackTimer = 0.5f;
                        animationHandler.changeAnimationState(jumpSmashAnimation);
                        mushroomGolemState = MushroomGolemState.JumpSmash;
                    }
                }                
            break;
            case MushroomGolemState.BigSmash:
                if (attackTimer > 0) {
                    attackTimer -= Time.deltaTime;

                    // Don't move
                    mv.Walk(0);
                    
                }
                else {
                    // Should summon shockwave by now

                    // Reset cooldown
                    attackCooldownTimer = attackCooldown;

                    animationHandler.changeAnimationState(idleAnimation);

                    // Change back to attacking state
                    mushroomGolemState = MushroomGolemState.Aggro;
                }
            break;
            case MushroomGolemState.JumpSmash:
                if (attackTimer > 0) {
                    attackTimer -= Time.deltaTime;

                    // Don't move
                    mv.Walk(0);

                }
                else {
                    // Jump towards target
                    mv.jumpReposition( (Vector2.Distance(transform.position, target.transform.position) - 4f) * mv.getFacingDirection(), 20);

                    // Change back to attacking state
                    mushroomGolemState = MushroomGolemState.Reposition;
                }
            break;
            case MushroomGolemState.BoulderToss:
                if (attackTimer > 0) {
                    attackTimer -= Time.deltaTime;

                    // Don't move
                    mv.Walk(0);

                }
                else {
                    // Aim a boulder at the player and throw it
                    aimBoulderAtPlayer();
                    var projectile = Instantiate(boulder, boulderSpawnPoint.transform.position, boulderSpawnPoint.transform.rotation).GetComponent<Projectile>();
                    projectile.setCreator(this.gameObject);

                    // Reset cooldown
                    attackCooldownTimer = attackCooldown;

                    // Change back to attacking state
                    mushroomGolemState = MushroomGolemState.Aggro;
                }
            break;
            case MushroomGolemState.Reposition:
                if (!mv.isGrounded()) {

                    // Handle animations
                    if (mv.checkRising())
                        animationHandler.changeAnimationState(risingAnimation);
                    else if (mv.checkFalling())
                        animationHandler.changeAnimationState(fallingAnimation);
                }
                else {
                    // Shake camera
                    GameManager.instance.shakeCamera(0.5f, 0.5f);

                    // Create small shockwaves
                    var obj = Instantiate(smallShockwave, frontSpawnPoint.transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)));
                    obj.GetComponent<Projectile>().setCreator(gameObject);

                    obj = Instantiate(smallShockwave, rearSpawnPoint.transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y - 180, 0)));
                    obj.GetComponent<Projectile>().setCreator(gameObject);

                    // Reset cooldown
                    attackCooldownTimer = attackCooldown;

                    // Change back to attacking state
                    mushroomGolemState = MushroomGolemState.Aggro;
                }

            break;
            case MushroomGolemState.Dead:
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
            }
        }
    }

    private void aimBoulderAtPlayer()
    {
        Vector2 direction = target.position - boulderSpawnPoint.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        boulderSpawnPoint.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }

    private IEnumerator wakeUp(float time) {
        animationHandler.changeAnimationState(awakenAnimation);

        // Wait for animation to play out
        yield return new WaitForSeconds(time);

        // If the target is a player, then show boss health bar
        if (target.TryGetComponent(out Player player)) {
            bossHealthBarUI.setBoss(this);
        }
        
        // Reset cooldown
        attackCooldownTimer = attackCooldown;

        // Change state
        mushroomGolemState = MushroomGolemState.Aggro;
    }

    private IEnumerator shakeTimer(float time) {
        // Wait
        yield return new WaitForSeconds(time);

        // Shake camera
        GameManager.instance.shakeCamera(0.5f, 0.5f);

        // Summon big shockwave after attack
        var projectile = Instantiate(bigShockwave, frontSpawnPoint.transform.position, frontSpawnPoint.transform.rotation).GetComponent<Projectile>();
        projectile.setCreator(gameObject);
    }

    protected override void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, farAttackRange);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}

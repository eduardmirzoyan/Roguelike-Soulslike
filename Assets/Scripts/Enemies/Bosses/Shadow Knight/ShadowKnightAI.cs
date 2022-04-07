using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
public class ShadowKnightAI : BossAI
{
    [Header("Shadow Knight Bow Values")]
    [SerializeField] private GameObject knightBow;
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int turnSpeed;

    [Header("Shadow Knight Attacks")]
    [SerializeField] private string overheadSwingAnimation;
    [SerializeField] private string frontStabAnimation;
    [SerializeField] private string shootBowAnimation;
    [SerializeField] private string summonShadowFlameAnimation;

    [Header("Shadow Knight Cast Values")]
    [SerializeField] private GameObject flame;
    [SerializeField] private Transform spawnpoint;

    [Header("Shadow Knight Settings")]
    [SerializeField] private float bowRange;
    [SerializeField] private float followRange;

    private enum ShadowKnightState {
        Idle,
        Aggro,
        FrontStab,
        OverheadSlash,
        ShadowFlame,
        ShootBow,
        Reposition,
        Dead
    }
    [SerializeField] private ShadowKnightState shadowKnightState;    
    
    private void Update() {
        if (shadowKnightState != ShadowKnightState.Dead && health.isEmpty())
        {
            // Call it's death method
            Die();

            // Destroy body in 1 second
            Destroy(gameObject, 10f);

            // Prevent movement
            mv.Walk(0);

            // Change state to dead
            shadowKnightState = ShadowKnightState.Dead;
        }
    }

    private void FixedUpdate()
    {
        switch(shadowKnightState) {
            case ShadowKnightState.Idle:
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

                    // Change state
                    shadowKnightState = ShadowKnightState.Aggro;
                }
            break;
            case ShadowKnightState.Aggro:
                // Always face target
                faceTarget();

                // If you get farther than aggro range, remove target
                if (Vector2.Distance(transform.position, target.position) > aggroRange) {
                    target = null;
                }

                // If target is gone (IE dead)
                if (target == null) {
                    // Go back to search state
                    shadowKnightState = ShadowKnightState.Idle;
                    return;
                }

                // If you have an attack cooldown, then reduce it
                if (attackCooldownTimer > 0) {
                    attackCooldownTimer -= Time.deltaTime;
                }

                // Cache distance
                var distance = Vector3.Distance(transform.position, target.position);

                // If you are in range
                if (distance < attackRange) {
                    // Don't move
                    mv.Walk(0);

                    // Change animation
                    animationHandler.changeAnimationState(idleAnimation);

                    // Check if cooldown is over
                    if (attackCooldownTimer <= 0) {
                        // Randomly pick an attack
                        int choice = Random.Range(0, 3); // 0 - 3
                        switch(choice) {
                            case 0:
                                attackTimer = 1.25f;
                                animationHandler.changeAnimationState(frontStabAnimation);
                                shadowKnightState = ShadowKnightState.FrontStab;
                            break;
                            case 1:
                                attackTimer = 1.083f;
                                animationHandler.changeAnimationState(overheadSwingAnimation);
                                shadowKnightState = ShadowKnightState.OverheadSlash;
                            break;
                            case 2:
                                attackTimer = 0.5f;
                                mv.jumpReposition(-mv.getFacingDirection() * attackRange * 2.5f, 10);
                                shadowKnightState = ShadowKnightState.Reposition;
                            break;
                        }
                    }
                }
                else {
                    // If you are in bow range
                    if (attackCooldownTimer <= 0 && distance > bowRange) {
                        // Stop moving
                        mv.Walk(0);

                        // Randomly choose between bow or cast
                        int choice = Random.Range(0, 3);

                        switch(choice) {
                            case 0:
                                attackTimer = 4f;
                                animationHandler.changeAnimationState(summonShadowFlameAnimation);
                                StartCoroutine(summonShadowFlame(3));
                                shadowKnightState = ShadowKnightState.ShadowFlame;
                            break;
                            default: // case 1 or 2 does the same thing
                                attackTimer = 0.75f;
                                animationHandler.changeAnimationState(shootBowAnimation);
                                startBowAttack();
                                shadowKnightState = ShadowKnightState.ShootBow;
                            break;
                        }
                    }
                    else {
                        if (distance > followRange) {
                            // Walk towards target
                            animationHandler.changeAnimationState(walkAnimation);
                            handleForwardMovement(mv.getFacingDirection());
                        }
                    }
                }

            break;
            case ShadowKnightState.FrontStab:
                if (attackTimer > 0) {
                    attackTimer -= Time.deltaTime;

                    if (attackTimer < 0.5f && attackTimer > 0.25f) {
                        mv.dash(attackDashSpeed, mv.getFacingDirection());
                    }
                    else {
                        mv.Walk(0);
                    }

                }
                else {
                    // Reset cooldown
                    attackCooldownTimer = attackCooldown;

                    // Change back to attacking state
                    shadowKnightState = ShadowKnightState.Aggro;
                }
            break;
            case ShadowKnightState.OverheadSlash:
                if (attackTimer > 0) {
                    attackTimer -= Time.deltaTime;

                    if (attackTimer < 0.5f && attackTimer > 0.25f) {
                        mv.dash(-attackDashSpeed, mv.getFacingDirection());
                    }
                    else {
                        mv.Walk(0);
                    }

                }
                else {
                    // Reset cooldown
                    attackCooldownTimer = attackCooldown;

                    // Change back to attacking state
                    shadowKnightState = ShadowKnightState.Aggro;
                }
            break;
            case ShadowKnightState.ShadowFlame:
                if (attackTimer > 0) {                
                    attackTimer -= Time.deltaTime;
                }
                else {
                    // Reset cooldown
                    attackCooldownTimer = attackCooldown;

                    // Change back to attacking state
                    shadowKnightState = ShadowKnightState.Aggro;
                }
            break;
            case ShadowKnightState.Reposition:
                // While still in the air, don't change states
                if (attackTimer > 0 || !mv.isGrounded()) {                
                    attackTimer -= Time.deltaTime;
                    
                    // Handle animations
                    if (mv.checkRising())
                        animationHandler.changeAnimationState(risingAnimation);
                    else if (mv.checkFalling())
                        animationHandler.changeAnimationState(fallingAnimation);
                }
                else {
                    // Do NOT Reset cooldown

                    // Change back to attacking state
                    shadowKnightState = ShadowKnightState.Aggro;
                }
            break;
            case ShadowKnightState.ShootBow:
                if (attackTimer > 0) {
                    
                    aimAtTarget();

                    attackTimer -= Time.deltaTime;
                }
                else {
                    // Release an arrow
                    fireArrow();

                    // Reset cooldown
                    attackCooldownTimer = attackCooldown;

                    // Change back to attacking state
                    shadowKnightState = ShadowKnightState.Aggro;
                }
            break;
            case ShadowKnightState.Dead:
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

    private void searchForEnemies() {
        // Aggressive, hunts any enemy in sight
       var colliders = Physics2D.OverlapCircleAll(transform.position, bowRange);

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

    private void aimAtTarget()
    {
        Vector2 direction = target.position - knightBow.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        knightBow.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }

    private void startBowAttack()
    {
        knightBow.GetComponent<Animator>().Play("Arm Bow");
    }

    private void fireArrow()
    {
        var projectile = Instantiate(arrowPrefab, firepoint.position, firepoint.rotation).GetComponent<Projectile>();
        projectile.setCreator(this.gameObject);
    }

    private IEnumerator summonShadowFlame(int count) {
        while (count > 0) {
            yield return new WaitForSeconds(1f);
            // Create the shadowflames
            var proj = Instantiate(flame, spawnpoint.transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0))).GetComponent<Projectile>();
            proj.setCreator(gameObject);
            proj = Instantiate(flame, spawnpoint.transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y - 180, 0))).GetComponent<Projectile>();
            proj.setCreator(gameObject);
            count--;
        }
    }

    protected override void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, bowRange);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}

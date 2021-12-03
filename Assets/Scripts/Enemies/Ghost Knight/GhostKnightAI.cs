using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Displacable))]
public class GhostKnightAI : EnemyAI
{
    [SerializeField] protected Health health;
    [SerializeField] protected Movement mv;
    [SerializeField] protected Displacable displacable;
    [SerializeField] protected Collidable collidable;
    [SerializeField] protected Damageable damageable;
    [SerializeField] protected EnemyHitbox hitbox;

    [SerializeField] protected EnemyShield shield;

    [Header("Ghost Knight values")]
    [SerializeField] private EnemyAttack swordSlash;
    [SerializeField] private EnemyAttack shieldRam;
    private float attackDashSpeed;

    [SerializeField] private float ramDistance;

    [Header("Ghost Knight Animations")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string deadAnimation = "Dead";

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Get required components
        damageable = GetComponent<Damageable>();
        mv = GetComponent<Movement>();
        displacable = GetComponent<Displacable>();
        collidable = GetComponent<Collidable>();
        health = GetComponent<Health>();
        shield = GetComponentInChildren<EnemyShield>();
        hitbox = GetComponentInChildren<EnemyHitbox>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // If enemy is killed, then set to dead
        if(health.isEmpty() && state != EnemyState.dead)
        {
            Die();
        }

        if (!displacable.isFree())
        {
            state = EnemyState.knockedback;
        }

        switch (state)
        {
            case EnemyState.knockedback:
                shield.lowerShield();
                if (displacable.isFree())
                {
                    shield.raiseShield();
                    state = EnemyState.aggro;
                }
                break;
            case EnemyState.idle:
                animationHandler.changeAnimationState(idleAnimation);

                if (lineOfSight.distanceFromTarget() < aggroRange && lineOfSight.canSeeTarget()) // TODO: add aggro if player is too close
                {
                    onAggro();
                    roamTimer = currentCooldownTimer;
                }

                if (!roamEnabled) // If roaming is disabled, then end the idle state here
                {
                    mv.Stop();
                    break;
                }

                if (roamTimer > 0)
                {
                    idleTimer = idleCooldown;
                    roamTimer -= Time.deltaTime;
                    mv.Walk(roamDirection); // Roam in given direction
                    if (jumpEnabled && mv.isGrounded() && mv.onWall())
                    {
                        mv.Jump();
                    }
                }
                else
                {   // After roaming, pause for a brief time before deciding next direction
                    if (idleTimer > 0)
                    {
                        mv.Walk(0);
                        idleTimer -= Time.deltaTime;
                    }
                    else
                    {
                        roamTimer = roamCooldown; // Reset roaming time
                        roamDirection = Random.Range(-1, 2); // Change roaming direction from -1 - 1
                        idleTimer = idleCooldown;
                    }
                }

                break;
            case EnemyState.aggro:
                animationHandler.changeAnimationState(idleAnimation);

                // Chase player type-beat
                float distanceFromPlayer = Vector2.Distance(transform.position, target.transform.position);
                
                facePlayer(); // Face the player while aggro'd on him

                // Increment attack timer, even if goblin is far
                if (currentCooldownTimer > 0)
                    currentCooldownTimer -= Time.deltaTime;

                if (lineOfSight.distanceFromTarget() > aggroRange)
                    aggroTimer -= Time.deltaTime;

                if (aggroTimer <= 0)
                    state = EnemyState.idle;

                if (distanceFromPlayer < maxAttackRange)
                {
                    if (distanceFromPlayer > minAttackRange) // Sweet spot
                    {
                        // Check if ready to attack
                        if (currentCooldownTimer <= 0)
                        { 
                            // Prepare to attack
                            setUpSequenceOfAttacks(new List<EnemyAttack> { swordSlash });
                        }

                        mv.Stop(); // Don't move and wait
                    }
                    else if (distanceFromPlayer < minAttackRange) // too close
                    {
                        mv.walkBackwards(-mv.getFacingDirection()); // Move away from player
                    }
                }
                else // Only move towards the player
                {
                    // Too far
                    mv.Walk(mv.getFacingDirection()); // Move toward the player

                    // Jump if reached a wall and is grounded
                    if (jumpEnabled && mv.isGrounded() && mv.onWall())
                        mv.Jump();

                    if(distanceFromPlayer > ramDistance && currentCooldownTimer <= 0)
                        if(lineOfSight.canSeeTarget())
                            setUpSequenceOfAttacks(new List<EnemyAttack> { shieldRam });
                }

                break;
            case EnemyState.charging: // Enemy charges for attack
                if (delayTimer > 0)
                {
                    delayTimer -= Time.deltaTime;
                }
                else
                {   // After enemy finishes charging, set required values and do the attack
                    attackTimer = currentAttack.attackDuration;
                    state = EnemyState.attacking;
                }
                break;
            case EnemyState.attacking: // Enemy is in the action of attacking
                if (attackTimer > 0)
                {
                    if(currentAttack.attackName == "Ram")
                    {
                        // If knight collides a while while dashing, then cancel dash and stun
                        if (mv.onWall())
                        {
                            animationHandler.changeAnimationState(idleAnimation);
                            currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown);
                            // Stun HERE
                            attackTimer = 0;
                        }
                    }

                    mv.dash(attackDashSpeed, mv.getFacingDirection());

                    // Reduced movespeed during attack in the direction of the attack
                    attackTimer -= Time.deltaTime;
                }
                else
                { // After attack is finished, reset enemy values and set to aggro
                    currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown); // Reset cooldown

                    recoveryTimer = currentAttack.attackRecovery; // Set reoovery time
                    state = EnemyState.recovering; // Change enemy state
                }
                break;
            case EnemyState.recovering: // Enemy recovery time after attacking
                if (recoveryTimer > 0)
                {
                    mv.Stop();
                    recoveryTimer -= Time.deltaTime;
                }
                else
                {
                    // Turn shield back on
                    shield.raiseShield();
                    currentSequenceOfAttacks.RemoveAt(0);
                    if (currentSequenceOfAttacks.Count > 0)
                    {
                        setUpSequenceOfAttacks(currentSequenceOfAttacks);
                        state = EnemyState.charging;
                    }
                    else
                    {
                        state = EnemyState.aggro;
                    }
                }
                break;
            case EnemyState.dead:
                // Do nothing so far
                animationHandler.changeAnimationState(deadAnimation);
                break;
        }
    }

    public override void onAggro()
    {
        base.onAggro();
        shield.raiseShield();
    }

    protected void facePlayer()
    {
        mv.setFacingDirection(target.transform.position.x - transform.position.x);
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject, 1f);
        state = EnemyState.dead;
    }

    protected override void setUpSequenceOfAttacks(List<EnemyAttack> enemyAttacks)
    {
        base.setUpSequenceOfAttacks(enemyAttacks);
        switch (currentAttack.attackName)
        {
            case "Slash":
                attackDashSpeed = 5;
                break;
            case "Ram":
                attackDashSpeed = 15;
                break;
        }
        animationHandler.changeAnimationState(currentAttack.attackName);
        shield.lowerShield();
    }
}

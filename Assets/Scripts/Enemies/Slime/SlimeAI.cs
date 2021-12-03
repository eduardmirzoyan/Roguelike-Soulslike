using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Displacable))]
[RequireComponent(typeof(Collidable))]
[RequireComponent(typeof(DamageDealer))]
public class SlimeAI : EnemyAI
{
    [SerializeField] protected Health health;
    [SerializeField] protected Movement mv;
    [SerializeField] protected Displacable displacable;
    [SerializeField] protected Collidable collidable;
    [SerializeField] protected Damageable damageable;
    [SerializeField] protected DamageDealer damageDealer;

    [Header("Slime specific values")]
    [SerializeField] protected EnemyAttack slimeLaunchAttack;
    [SerializeField] protected EnemyAttack slimeBounce;
    [SerializeField] protected float dashSpeed;

    private bool preparedToJump;

    [Header("Slime Animations")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string risingAnimation = "Rise";
    [SerializeField] private string fallingAnimation = "Fall";
    [SerializeField] private string prepareToJumpAnimation = "Prepare";

    // Start is called before the first frame update
    protected override void Start()
    {
        damageable = GetComponent<Damageable>();
        mv = GetComponent<Movement>();
        displacable = GetComponent<Displacable>();
        collidable = GetComponent<Collidable>();
        health = GetComponent<Health>();
        damageDealer = GetComponent<DamageDealer>();

        // Set the damage of the slime
        damageDealer.setDamage(slimeLaunchAttack.damage);
        base.Start();
    }

    protected void FixedUpdate()
    {
        // If enemy is killed, then set to dead
        if (health.isEmpty() && state != EnemyState.dead)
        {
            Die();
        }
        switch (state)
        {
            case EnemyState.knockedback:
                
                break;
            case EnemyState.idle:
                handleMovementAnimations();

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
                        mv.Stop();
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
                handleMovementAnimations();

                if (lineOfSight.distanceFromTarget() > aggroRange)
                    aggroTimer -= Time.deltaTime;

                if (aggroTimer <= 0)
                    state = EnemyState.idle;

                if (idleTimer > 0) // While idle, dont move
                {
                    if (!mv.isGrounded()) // Some cyote time, jump in the direction of the player
                    {
                        mv.Walk(mv.getFacingDirection());
                    }
                    if (mv.isGrounded()) // Face the player while grounded
                    {
                        mv.Stop();
                        facePlayer();
                        if(idleTimer < 1f && !preparedToJump)
                        {
                            animationHandler.changeAnimationState(prepareToJumpAnimation);
                            preparedToJump = true;
                        }
                    }
                    idleTimer -= Time.deltaTime;
                }
                else
                {   
                    bounce(mv.getFacingDirection()); // hop towards the player
                    idleTimer = idleCooldown;
                    preparedToJump = false;
                }

                // Decrement cooldown while enemy is normal aggro
                if (currentCooldownTimer > 0 && mv.isGrounded())
                {
                    currentCooldownTimer -= Time.deltaTime;
                }
                else
                {   // Once cooldown is done, check if player is in range for attack
                    float distanceFromPlayer = Vector2.Distance(transform.position, target.transform.position);
                    if (distanceFromPlayer < maxAttackRange && currentCooldownTimer <= 0) // If player is in range, set state to charging attack
                    {
                        mv.Stop();
                        setUpSequenceOfAttacks(new List<EnemyAttack> { slimeLaunchAttack });
                    }
                }

                break;
            case EnemyState.charging:
                if (delayTimer > 0)
                {
                    // While charging, don't move or do anything
                    delayTimer -= Time.deltaTime;
                }
                else
                {
                    // Transition to attacking while setting values
                    slimeAttack();
                    attackTimer = currentAttack.attackDuration;
                    state = EnemyState.attacking;
                }
                break;
            case EnemyState.attacking:
                handleMovementAnimations();

                if (attackTimer > 0)
                {
                    // Don't actually do anything during the attack
                    if (mv.isGrounded() && currentAttack.attackDuration * 0.9f > attackTimer)
                    {
                        mv.Stop();
                    } 
                    attackTimer -= Time.deltaTime;
                }
                else
                {
                    mv.Stop();
                    currentCooldownTimer = Random.Range(maxAttackCooldown, minAttackCooldown);
                    recoveryTimer = currentAttack.attackRecovery;
                    state = EnemyState.recovering;
                }
                break;
            case EnemyState.recovering: // Enemy recovery time after attacking
                handleMovementAnimations();

                if (recoveryTimer > 0)
                {
                    mv.Stop();
                    recoveryTimer -= Time.deltaTime;
                }
                else
                {
                    idleTimer = idleCooldown;
                    state = EnemyState.aggro;
                }
                break;
        }
        enableSpikesWhenFalling(); // At any point, if the slime is falling, the enable spikes
    }

    protected override void setUpSequenceOfAttacks(List<EnemyAttack> enemyAttacks)
    {
        preparedToJump = false;
        base.setUpSequenceOfAttacks(enemyAttacks);
        animationHandler.changeAnimationState(currentAttack.attackName);
    }

    private void handleMovementAnimations()
    {
        if (!mv.isGrounded())
        {
            if (body.velocity.y > 0)
                animationHandler.changeAnimationState(risingAnimation);
            else
                animationHandler.changeAnimationState(fallingAnimation);
        }
        else if(!preparedToJump)
            animationHandler.changeAnimationState(idleAnimation);
    }

    private void bounce(float direction)
    {
        if (jumpEnabled && mv.isGrounded())
        {
            mv.Walk(direction);
            mv.Jump();
        }
    }

    private void slimeAttack()
    {
        mv.angledDash(dashSpeed, mv.getFacingDirection());
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }

    private void enableSpikesWhenFalling()
    {
        if (mv.checkFalling())
        {
            collidable.checkCollisions(damageEnemies);
        }
    }

    // Fix this...
    private void damageEnemies(Collider2D coll)
    {
        if (coll.GetComponent<Damageable>() != null)
            damageDealer.dealDamageTo(coll.GetComponent<Damageable>());
    }

    public override void onAggro()
    {
        base.onAggro();
        idleTimer = idleCooldown;
    }
    protected void facePlayer() => mv.setFacingDirection(target.transform.position.x - transform.position.x);

}

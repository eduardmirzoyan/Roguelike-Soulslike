using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Displacable))]
public class GoblinAI : EnemyAI
{
    [Header("Goblin Specific Stats")]
    [SerializeField] private float dashSpeed;

    [Header("Preset attacks")]
    [SerializeField] protected EnemyAttack dashingStabAttack; // ID 1
    [SerializeField] protected EnemyAttack overHeadSlashAttack; // ID 2
    [SerializeField] protected List<EnemyAttack> threePartAttack; // ID 3, 4, 5

    [SerializeField] protected Health health;
    [SerializeField] protected Movement mv;
    [SerializeField] protected Displacable displacable;
    [SerializeField] protected Collidable collidable;
    [SerializeField] protected Damageable damageable;

    [Header("Goblin Animations")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walk";
    [SerializeField] private string deadAnimation = "Dead";
    [SerializeField] private string staggerAnimation = "Stagger";

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
    }

    // Update is called once per frame
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
                animationHandler.changeAnimationState(staggerAnimation);

                displacable.performDisplacement();

                handleDisplacement();

                if (!displacable.isDisplaced)
                    state = EnemyState.aggro;

                break;
            case EnemyState.idle:
                handleMovementAnimation();

                handleDisplacement();

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
                    handleForwardMovement(roamDirection);
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
                handleMovementAnimation();

                // Chase player type-beat
                float distanceFromPlayer = lineOfSight.distanceFromTarget();

                facePlayer(); // Face the player while aggro'd on him

                if (distanceFromPlayer > aggroRange)
                    aggroTimer -= Time.deltaTime;
        
                if (aggroTimer <= 0)
                    state = EnemyState.idle;

                // Increment attack timer, even if goblin is far
                if (currentCooldownTimer > 0)
                    currentCooldownTimer -= Time.deltaTime;

                if (distanceFromPlayer < maxAttackRange)
                {      
                    if (distanceFromPlayer > minAttackRange) // Sweet spot //!isBlocking && 
                    {
                        // Check if ready to attack
                        if (currentCooldownTimer <= 0)
                        { // Prepare to attack
                            if (distanceFromPlayer < (maxAttackRange + minAttackRange) / 2)
                            {
                                // 33% chance to do 3 part attack and 66% to do regular slash
                                if (Random.Range(1, 4) == 1)
                                    setUpSequenceOfAttacks(new List<EnemyAttack>(threePartAttack));
                                else
                                    setUpSequenceOfAttacks(new List<EnemyAttack> { overHeadSlashAttack });
                            }
                            else
                            {
                                setUpSequenceOfAttacks(new List<EnemyAttack> { dashingStabAttack });
                            }
                        }

                        mv.Stop(); // Don't move and wait
                    }
                    else if(distanceFromPlayer < minAttackRange) // too close // !isBlocking &&
                    {
                        mv.walkBackwards(-mv.getFacingDirection()); // Move away from player

                        if (mv.backAgainstWall()) // If the enemies back is against a wall do...
                        {
                            setUpSequenceOfAttacks(new List<EnemyAttack> { overHeadSlashAttack }); // Attack the player if driven to corner
                        }
                    }
                }
                else // Only move towards the player
                {
                    handleForwardMovement(mv.getFacingDirection());
                }
                handleDisplacement();
                break;
            case EnemyState.charging: // Enemy charges for attack
                if (delayTimer > 0)
                {
                    // While charging, don't move or do anything
                    mv.Stop();
                    delayTimer -= Time.deltaTime;
                }
                else
                {   // After enemy finishes charging, set required values and do the attack
                    attackTimer = currentAttack.attackDuration;
                    
                    state = EnemyState.attacking;
                }
                handleDisplacement();
                break;
            case EnemyState.attacking: // Enemy is in the action of attacking
                if (attackTimer > 0)
                {
                    mv.dash(dashSpeed, mv.getFacingDirection());
                       
                    // Reduced movespeed during attack in the direction of the attack
                    attackTimer -= Time.deltaTime;
                }
                else
                { // After attack is finished, reset enemy values and set to aggro

                    currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown); // Reset cooldown

                    recoveryTimer = currentAttack.attackRecovery; // Set reoovery time
                    state = EnemyState.recovering; // Change enemy state
                }
                handleDisplacement();
                break;
            case EnemyState.recovering: // Enemy recovery time after attacking
                handleDisplacement();
                if (recoveryTimer > 0)
                {
                    mv.Stop();
                    recoveryTimer -= Time.deltaTime;
                } 
                else
                {
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
                handleDisplacement();
                break;
            case EnemyState.dead:
                animationHandler.changeAnimationState(deadAnimation);
                break;
        }
    }

    private void handleDisplacement()
    {
        if (displacable.isDisplaced)
        {
            state = EnemyState.knockedback;
        }
    }

    private void resetCombatValues()
    {
        delayTimer = 0;
        attackTimer = 0;
        recoveryTimer = 0;
        currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown);
    }

    private void handleForwardMovement(float direction)
    {
        // Too far
        mv.Walk(direction); // Move toward the player

        // Jump if reached a wall and is grounded
        if (jumpEnabled && mv.isGrounded() && mv.onWall())
            mv.Jump();

        if (mv.aboutToDrop())
            mv.Stop();
    }

    private void handleMovementAnimation()
    {
        if (Mathf.Abs(body.velocity.x) > 0.2f)
            animationHandler.changeAnimationState(walkAnimation);
        else
            animationHandler.changeAnimationState(idleAnimation);
    }

    protected override void setUpSequenceOfAttacks(List<EnemyAttack> enemyAttacks)
    {
        base.setUpSequenceOfAttacks(enemyAttacks);
        switch (currentAttack.attackName)
        {
            case "Dash Stab":
                dashSpeed = 15;
                break;
            case "Overhead Slash":
                dashSpeed = 8;
                break;
            case "Triple Attack 1":
                dashSpeed = 8;
                break;
            case "Triple Attack 2":
                dashSpeed = 8;
                break;
            case "Triple Attack 3":
                dashSpeed = 10;
                break;
        }
        animationHandler.changeAnimationState(currentAttack.attackName);
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject, 3f);
        state = EnemyState.dead;
    }

    protected void facePlayer()
    {
        mv.setFacingDirection(target.transform.position.x - transform.position.x);
    }
}

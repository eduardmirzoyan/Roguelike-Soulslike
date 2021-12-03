using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Displacable))]
public class FireMageAI : EnemyAI
{
    [SerializeField] protected Health health;
    [SerializeField] protected Movement mv;
    [SerializeField] protected Displacable displacable;
    [SerializeField] protected Collidable collidable;
    [SerializeField] protected Damageable damageable;

    [Header("Fire Mage Attacks")]
    [SerializeField] private EnemyAttack castFireball; // ID 1
    [SerializeField] private EnemyAttack castBigFireball; // ID 2
    [SerializeField] private EnemyAttack castTeleport; // ID 3

    [Header("Fire ball Prefab")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bigFireBallPrefab;
    [SerializeField] private GameObject fireBallPrefab;

    [Header("Particles")]
    [SerializeField] private GameObject teleportParticles;
    [SerializeField] private GameObject fireballChargeParticles;

    [Header("Fire Mage Animations")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walk";
    [SerializeField] private string risingAnimation = "Rise";
    [SerializeField] private string fallingAnimation = "Fall";
    [SerializeField] private string deadAnimation = "Dead";

    // Start is called before the first frame update
    protected override void Start()
    {
        // Get required components
        damageable = GetComponent<Damageable>();
        mv = GetComponent<Movement>();
        displacable = GetComponent<Displacable>();
        collidable = GetComponent<Collidable>();
        health = GetComponent<Health>();

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

                if (displacable.isFree())
                    state = EnemyState.aggro;

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
                    handleForwardMovement(roamDirection);
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
                handleMovementAnimations();

                // Chase player type-beat
                float distanceFromPlayer = Vector2.Distance(transform.position, target.transform.position);

                facePlayer(); // Face the player while aggro'd on him

                if (lineOfSight.distanceFromTarget() > aggroRange)
                    aggroTimer -= Time.deltaTime;

                if (aggroTimer <= 0)
                    state = EnemyState.idle;

                // Increment attack timer, even if goblin is far
                if (currentCooldownTimer > 0)
                    currentCooldownTimer -= Time.deltaTime;

                if (distanceFromPlayer < maxAttackRange)
                {
                    if (distanceFromPlayer > minAttackRange && lineOfSight.canSeeTarget()) // Sweet spot //!isBlocking && 
                    {
                        // Check if ready to attack
                        if (currentCooldownTimer <= 0)
                        {
                            int random = Random.Range(0, 2);
                            switch (random)
                            {
                                case 0:
                                    // Prepare to attack
                                    setUpSequenceOfAttacks(new List<EnemyAttack> { castFireball, castFireball });
                                    break;
                                case 1:
                                    // Prepare to attack
                                    setUpSequenceOfAttacks(new List<EnemyAttack> { castBigFireball });
                                    break;
                            }
                        }

                        mv.Stop(); // Don't move and wait
                    }
                    else if (distanceFromPlayer < minAttackRange && lineOfSight.canSeeTarget()) // too close // !isBlocking &&
                    {
                        mv.walkBackwards(-mv.getFacingDirection()); // Move away from player
                        if (currentCooldownTimer <= 0)
                        {
                            setUpSequenceOfAttacks(new List<EnemyAttack> { castTeleport, castBigFireball });
                        }
                    }
                }
                else // Only move towards the player
                {
                    handleForwardMovement(mv.getFacingDirection());
                }

                break;
            case EnemyState.charging: // Enemy charges for attack
                if (delayTimer > 0)
                {   
                    // Aim firepoint at the player
                    if(currentAttack.attackName == "Cast Fireball" || currentAttack.attackName == "Cast Big Fireball")
                        aimAtTarget();

                    // While charging, don't move or do anything
                    mv.Stop();
                    delayTimer -= Time.deltaTime;
                }
                else
                {   // After enemy finishes charging, set required values and do the attack
                    attackTimer = currentAttack.attackDuration;

                    if (currentAttack.attackName == "Cast Fireball")
                        spawnFireBall();

                    if (currentAttack.attackName == "Cast Big Fireball")
                        spawnBigFireBall();

                    if (currentAttack.attackName == "Teleport")
                        teleport(-mv.getFacingDirection(), 6f);
                        

                    state = EnemyState.attacking;
                }
                break;
            case EnemyState.attacking: // Enemy is in the action of attacking
                if (attackTimer > 0)
                {
                    // Simulate knockback
                    if (currentAttack.attackName == "Cast Big Fireball")
                        mv.dashWithVelocity(3, -mv.getFacingDirection());

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
                animationHandler.changeAnimationState(deadAnimation);
                break;
        }
    }

    private void handleForwardMovement(float direction)
    {
        // Too far
        mv.Walk(direction); // Move toward the player

        // Jump if reached a wall and is grounded
        if (jumpEnabled && mv.isGrounded() && mv.onWall())
            mv.Jump();

        // Implement later
/*        if (mv.aboutToDrop())
        {
            mv.Stop();
        }*/
    }

    protected override void setUpSequenceOfAttacks(List<EnemyAttack> enemyAttacks)
    {
        base.setUpSequenceOfAttacks(enemyAttacks);
        switch (currentAttack.attackName)
        {
            case "Cast Fireball":
                Instantiate(fireballChargeParticles, firePoint.position, Quaternion.identity);
                break;
            case "Cast Big Fireball":
                Instantiate(fireballChargeParticles, firePoint.position, Quaternion.identity);
                break;
            case "Teleport":
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

    private void handleMovementAnimations()
    {
        if (!mv.isGrounded())
        {
            if (body.velocity.y > 0)
                animationHandler.changeAnimationState(risingAnimation);
            else
                animationHandler.changeAnimationState(fallingAnimation);
        }
        else
        {
            if (Mathf.Abs(body.velocity.x) > 0.2f)
                animationHandler.changeAnimationState(walkAnimation);
            else
                animationHandler.changeAnimationState(idleAnimation);
        }
    }

    private void aimAtTarget()
    {
        Vector2 direction = target.position - firePoint.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }

    private void spawnBigFireBall()
    {
        var fireball = Instantiate(bigFireBallPrefab, firePoint.position, firePoint.rotation);
        fireball.GetComponent<Projectile>().setCreator(gameObject);
    }

    private void spawnFireBall()
    {
        var fireball = Instantiate(fireBallPrefab, firePoint.position, firePoint.rotation);
        fireball.GetComponent<Projectile>().setCreator(gameObject);
    }

    private void teleport(int direction, float distance)
    {
        // Intial destination is x distance infront of the entity in the chosen direction
        Vector2 destination = (Vector2)transform.position + direction * Vector2.right * distance;
        
        RaycastHit2D hit = Physics2D.Linecast(transform.position, destination);
        // If you hit something with the linecast, then set your teleport destination to be right in front of it
        if (hit)
            destination = transform.position + transform.right * (hit.distance - 0.5f);

        // Then raycast from the current chosen destination downwards
        hit = Physics2D.Raycast(destination, -Vector2.up);
        if (hit)
        {
            // Set destination to be at the top of the contact point
            destination = hit.point;

            // Then give some buffer space
            destination.y += 1.75f;

            // Create teleport particles
            Instantiate(teleportParticles, transform.position, Quaternion.identity);

            // Move entity to destination
            transform.position = destination;
        }
    }
}

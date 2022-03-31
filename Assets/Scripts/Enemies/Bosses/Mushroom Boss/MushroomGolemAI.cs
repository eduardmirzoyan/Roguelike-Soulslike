using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomGolemAI : BossAI
{
    [Header("Mushroom Golem Gameobjects")]
    [SerializeField] private GameObject bigShockwave;
    [SerializeField] private GameObject smallShockwave;
    [SerializeField] private GameObject boulder;

    [SerializeField] private Transform frontSpawnPoint;
    [SerializeField] private Transform rearSpawnPoint;
    [SerializeField] private Transform boulderSpawnPoint;

    [Header("Mushroom Golem Attacks")]
    [SerializeField] private EnemyAttack bigSmash; // ID 1
    [SerializeField] private EnemyAttack jumpSmash; // ID 2
    [SerializeField] private EnemyAttack boulderToss; // ID 3

    [SerializeField] private float farAttackRange;

    private float repositionTimer;

    [SerializeField] private string sleepAnimation = "Sleep";
    [SerializeField] private string awakenAnimation = "Awaken";

    private void FixedUpdate()
    {
        // if (health.isEmpty() && state != EnemyState.dead)
        // {
        //     Die();
        // }
        // switch (state)
        // {
        //     case EnemyState.knockedback:
                
        //         break;
        //     case EnemyState.idle:
        //         animationHandler.changeAnimationState(sleepAnimation);

        //         if (lineOfSight.distanceFromTarget() < maxAttackRange)
        //         {
        //             onAggro();
        //             roamTimer = currentCooldownTimer;
        //         }

                

        //         if (roamTimer > 0)
        //         {
        //             idleTimer = idleCooldown;
        //             roamTimer -= Time.deltaTime;
        //             mv.Walk(roamDirection); // Roam in given direction
        //             if (mv.isGrounded())
        //             {
        //                 mv.Jump();
        //             }
        //         }
        //         else
        //         {   // After roaming, pause for a brief time before deciding next direction
        //             if (idleTimer > 0)
        //             {
        //                 mv.Stop();
        //                 idleTimer -= Time.deltaTime;
        //             }
        //             else
        //             {
        //                 roamTimer = roamCooldown; // Reset roaming time
        //                 roamDirection = Random.Range(-1, 1) >= 0 ? 1 : -1; // Change roaming direction
        //                 idleTimer = idleCooldown;
        //             }
        //         }
        //         mv.Stop();
        //         // Prevents movment during idle for now

        //         break;
        //     case EnemyState.aggro:
        //         handleMovementAnimations();

        //         // Chase player type-beat
        //         float distanceFromPlayer = Vector2.Distance(transform.position, target.transform.position);

        //         facePlayer(); // Face the player while aggro'd on him

        //         // Increment attack timer, even if goblin is far
        //         if (currentCooldownTimer > 0)
        //             currentCooldownTimer -= Time.deltaTime;

        //         if (distanceFromPlayer < maxAttackRange)
        //         {

        //             if (distanceFromPlayer > minAttackRange && mv.isGrounded()) // Sweet spot
        //             {
        //                 mv.Stop(); // Don't move and wait

        //                 // Check if ready to attack
        //                 if (currentCooldownTimer <= 0)
        //                 {
        //                     setUpSequenceOfAttacks(new List<EnemyAttack> { bigSmash });
        //                 }
        //             }
        //             else if (distanceFromPlayer < minAttackRange) // too close
        //             {
        //                 mv.walkBackwards(mv.getFacingDirection()); // Move away from player
        //             }
        //         }
        //         else // Only move towards the player
        //         {
        //             // Too far
        //             mv.Walk(mv.getFacingDirection()); // Move toward the player

        //             // Jump if reached a wall and is grounded
        //             if ( mv.isGrounded() && mv.onWall()) {
        //                 mv.Jump();
        //             }
                        
        //             if(currentCooldownTimer <= 0)
        //             {
        //                 if (distanceFromPlayer > farAttackRange)
        //                     setUpSequenceOfAttacks(new List<EnemyAttack> { jumpSmash });
        //                 else
        //                 {
        //                     int choice = Random.Range(0, 3); // Random number from 0 - 2
        //                     switch (choice)
        //                     {
        //                         case 0:
        //                             setUpSequenceOfAttacks(new List<EnemyAttack> { jumpSmash });
        //                             break;
        //                         case 1:
        //                             setUpSequenceOfAttacks(new List<EnemyAttack> { boulderToss });
        //                             break;
        //                         case 2:
        //                             setUpSequenceOfAttacks(new List<EnemyAttack> { bigSmash });
        //                             break;
        //                     }
        //                 }
        //             }
        //         }
        //         break;
        //     case EnemyState.repositioning:
        //         if (repositionTimer > 0)
        //         {
        //             mv.Stop();
        //             repositionTimer -= Time.deltaTime;
        //         }
        //         else
        //             state = EnemyState.aggro;
        //         break;
        //     case EnemyState.charging: // Enemy charges for attack
        //         if (delayTimer > 0)
        //         {

        //             mv.Stop();
        //             delayTimer -= Time.deltaTime;
        //         }
        //         else
        //         {   // After enemy finishes charging, set required values and do the attack
        //             attackTimer = currentAttack.attackDuration;

        //             if(currentAttack.ID == 2)
        //             {

        //                 mv.jumpReposition( (Vector2.Distance(transform.position, target.transform.position) - 4f) * mv.getFacingDirection());
        //             }

        //             state = EnemyState.attacking;
        //         }
        //         break;
        //     case EnemyState.attacking: // Enemy is in the action of attacking
        //         if (attackTimer > 0)
        //         {

        //             if(currentAttack.attackName == "Jump Smash")
        //             {
        //                 handleMovementAnimations();

        //                 if(attackTimer > 0.2f)
        //                     attackTimer -= Time.deltaTime;

        //                 if (mv.isGrounded() && attackTimer < 0.2f)
        //                     attackTimer = 0;
                        
        //             }
        //             else
        //                 attackTimer -= Time.deltaTime;
        //         }
        //         else
        //         { // After attack is finished, reset enemy values and set to aggro
        //             if (currentAttack.attackName == "Forward Smash")
        //             {
        //                 GameManager.instance.shakeCamera(0.5f, 0.5f);

        //                 Instantiate(bigShockwave, frontSpawnPoint.transform.position, frontSpawnPoint.transform.rotation);
        //             }
                        

        //             if(currentAttack.attackName == "Jump Smash")
        //             {
        //                 GameManager.instance.shakeCamera(0.5f, 0.5f);

        //                 Instantiate(smallShockwave, frontSpawnPoint.transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)));

        //                 Instantiate(smallShockwave, rearSpawnPoint.transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y - 180, 0)));
        //             }

        //             if(currentAttack.attackName == "Throw Boulder")
        //             {
        //                 aimBoulderAtPlayer();
        //                 Instantiate(boulder, boulderSpawnPoint.transform.position, boulderSpawnPoint.transform.rotation);
        //             }
                        
        //             recoveryTimer = currentAttack.attackRecovery; // Set reoovery time
        //             state = EnemyState.recovering; // Change enemy state
        //         }
        //         break;
        //     case EnemyState.recovering: // Enemy recovery time after attacking
        //         if (recoveryTimer > 0)
        //         {
        //             mv.Stop();
        //             recoveryTimer -= Time.deltaTime;
        //         }
        //         else
        //         {
        //             currentSequenceOfAttacks.RemoveAt(0);
        //             if (currentSequenceOfAttacks.Count > 0)
        //             {
        //                 setUpSequenceOfAttacks(currentSequenceOfAttacks);
        //                 state = EnemyState.charging;
        //             }
        //             else
        //             {
        //                 currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown); // Reset cooldown
        //                 state = EnemyState.aggro;
        //             }
        //         }
        //         break;
        //     case EnemyState.dead:
        //         // Do nothing so far

        //         break;
        // }
    }

    // protected override void setUpSequenceOfAttacks(List<EnemyAttack> enemyAttacks)
    // {
    //     base.setUpSequenceOfAttacks(enemyAttacks);
    //     switch (currentAttack.attackName) 
    //     {
    //         case "Forward Smash":
    //             //animator.SetTrigger("smash");
    //             break;
    //         case "Jump Smash":
    //             //animator.SetTrigger("jump smash");
    //             break;
    //         case "Throw Boulder":
    //             //animator.SetTrigger("throw boulder");
    //             break;
    //         default:
    //             throw new System.Exception("Mushroom Golem Tried to use a move it did not know lol");
    //     };
    //     facePlayer();
    //     animationHandler.changeAnimationState(currentAttack.attackName);
    // }

    private void aimBoulderAtPlayer()
    {
        Vector2 direction = target.position - boulderSpawnPoint.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        boulderSpawnPoint.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }

    // public override void onAggro()
    // {
    //     base.onAggro();

    //     // Start awaken animation
    //     animationHandler.changeAnimationState(awakenAnimation);
    //     repositionTimer = 1.5f;
    //     state = EnemyState.repositioning;
    // }
}

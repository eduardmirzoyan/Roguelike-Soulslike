using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Displacable))]
public class ShadowKnightAI : BossAI
{
    [Header("Shadow Knight Bow Values")]
    [SerializeField] private GameObject knightBow;
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int turnSpeed;

    [Header("Preset Shadow Knight attacks")]
    [SerializeField] private EnemyAttack overheadSwing; // ID 1
    [SerializeField] private EnemyAttack frontStab; // ID 2
    [SerializeField] private EnemyAttack shootBow; // ID 3
    [SerializeField] private EnemyAttack summonShdowFlame; // ID 4
    [SerializeField] private EnemyAttack DashStab; // ID 5
    [SerializeField] private float dashSpeed;
    [SerializeField] private bool readyToShoot;

    [SerializeField] private GameObject flame;
    [SerializeField] private Transform spawnpoint;

    [SerializeField] private GameObject intentionSparkle;
    [SerializeField] private Transform perilousIntentionPoint;

    [SerializeField] private float repositionTimer;
    
    
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
        //         handleMovementAnimations();

        //         if (lineOfSight.distanceFromTarget() < maxAttackRange) // Change this
        //         {
        //             onAggro();
        //             state = EnemyState.aggro;
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
        //                 roamDirection = UnityEngine.Random.Range(-1, 1) >= 0 ? 1 : -1; // Change roaming direction
        //                 idleTimer = idleCooldown;
        //             }
        //         }
        //         mv.Stop(); // Prevents movment during idle for now
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
        //                     // Don't do anything yet
        //                     int choice = 3; //UnityEngine.Random.Range(1, 6); //Random.Range(0, 3); // Gives value 1 - 4

        //                     switch (choice) 
        //                     {
        //                         case 1:
        //                             setUpSequenceOfAttacks(new List<EnemyAttack> { overheadSwing });
        //                             break;
        //                         case 2:
        //                             setUpSequenceOfAttacks(new List<EnemyAttack> { frontStab });
        //                             break;
        //                         case 3:
        //                             reposition(-1); // Reposition away from the player, then prepare to fire at player
        //                             break;
        //                         case 4:
        //                             reposition(1); // Reposition toward the player
        //                             break;
        //                         case 5:
        //                             setUpSequenceOfAttacks(new List<EnemyAttack> { overheadSwing, frontStab });
        //                             break;
        //                     }
        //                 }
        //             }
        //             else if (distanceFromPlayer < minAttackRange) // too close
        //             {
        //                 mv.walkBackwards(-mv.getFacingDirection()); // Move away from player
        //             }
        //         }
        //         else // Only move towards the player
        //         {
        //             // Too far
        //             mv.Walk(mv.getFacingDirection()); // Move toward the player

        //             // Jump if reached a wall and is grounded
        //             if (mv.isGrounded() && mv.onWall())
        //                 mv.Jump();

        //             if (currentCooldownTimer <= 0 && readyToShoot)
        //             {
        //                 int randomRangedAttack = UnityEngine.Random.Range(0, 2);

        //                 switch (randomRangedAttack)
        //                 {
        //                     case 0:
        //                         setUpSequenceOfAttacks(new List<EnemyAttack> { summonShdowFlame, summonShdowFlame, summonShdowFlame });
        //                         break;
        //                     case 1:
        //                         startBowAttack();
        //                         setUpSequenceOfAttacks(new List<EnemyAttack> { shootBow });
        //                         break;
        //                 }


        //                 readyToShoot = false;
        //             }
        //         }
        //         break;
        //     case EnemyState.repositioning:
        //         handleMovementAnimations();

        //         if (repositionTimer > 0)
        //             repositionTimer -= Time.deltaTime;
        //         else
        //         {
        //             if (mv.isGrounded())
        //             {
        //                 state = EnemyState.aggro;
        //             }
        //         }
        //         break;
        //     case EnemyState.charging: // Enemy charges for attack
        //         if (delayTimer > 0)
        //         {
        //             // While charging, don't move or do anything
        //             if (currentAttack.attackName == "Shoot Bow")
        //                 aimAtPlayer();

        //             mv.Stop();
        //             delayTimer -= Time.deltaTime;
        //         }
        //         else
        //         {   // After enemy finishes charging, set required values and do the attack
        //             attackTimer = currentAttack.attackDuration;

        //             if (currentAttack.attackName == "Shoot Bow")
        //                 fireArrow();

        //             if (currentAttack.attackName == "Cast Shadow Wave")
        //             {
        //                 Instantiate(flame, spawnpoint.transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)));
        //                 Instantiate(flame, spawnpoint.transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y - 180, 0)));
        //             }
        //             state = EnemyState.attacking;
        //         }
        //         break;
        //     case EnemyState.attacking: // Enemy is in the action of attacking
        //         if (attackTimer > 0)
        //         {
        //             mv.dash(dashSpeed, mv.getFacingDirection());

        //             // Reduced movespeed during attack in the direction of the attack
        //             attackTimer -= Time.deltaTime;
        //         }
        //         else
        //         { // After attack is finished, reset enemy values and set to aggro
        //             if (currentAttack.attackName == "Overhead Slash")
        //                 GameManager.instance.shakeCamera(0.5f, 0.5f);

        //             if (currentAttack.attackName == "Shoot Bow")
        //                 animationHandler.changeAnimationState(idleAnimation);

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
        //                 currentCooldownTimer = UnityEngine.Random.Range(minAttackCooldown, maxAttackCooldown); // Reset cooldown
        //                 state = EnemyState.aggro;
        //             }
        //         }
        //         break;
        //     case EnemyState.dead:
        //         // Do nothing so far
                
        //         break;
        // }
    }
    
    
    private void reposition(int direction) // reposition boss either toward the player if direction == 1, or away if direction == -1
    {
        // mv.jumpReposition(direction * mv.getFacingDirection() * maxAttackRange * 2);
        // if(direction == -1) // If the boss jumps away from the player, then prepare to fire
        // {
        //     readyToShoot = true;
        //     currentCooldownTimer = 0;
        // }
        // repositionTimer = 0.5f;
        // state = EnemyState.repositioning;
    }

    private void aimAtPlayer()
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
        Instantiate(arrowPrefab, firepoint.position, firepoint.rotation);
    }

    // protected override void setUpSequenceOfAttacks(List<EnemyAttack> enemyAttacks)
    // {
    //     base.setUpSequenceOfAttacks(enemyAttacks);
    //     switch (currentAttack.attackName)
    //     {
    //         case "Overhead Slash":
    //             dashSpeed = -10;
    //             break;
    //         case "PDash Stab":
    //             dashSpeed = 15;
    //             Instantiate(intentionSparkle, perilousIntentionPoint.position, Quaternion.identity);
    //             break;
    //         case "Shoot Bow":
    //             dashSpeed = 0;
    //             break;
    //         case "Cast Shadow Wave":
    //             dashSpeed = 0;
    //             Instantiate(intentionSparkle, spawnpoint.position, Quaternion.identity);
    //             break;
    //         default:
    //             throw new Exception("Shadow Knight Tried to use a move it did not know lol");
    //     }
    //     facePlayer();
    //     animationHandler.changeAnimationState(currentAttack.attackName);
    // }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable
public class BatAI : EnemyAI
{
    /*
    [SerializeField] protected SpriteRenderer eyeRender;
    [SerializeField] protected bool playerIsHeavyAttacking;
    protected override void Start()
    {
        base.Start();
    }
    protected override void FixedUpdate()
    {
        switch (state)
        {
            case EnemyState.stunned:
                if (stunTimer > 0)
                {
                    stunTimer -= Time.deltaTime;
                }
                else
                {
                    state = EnemyState.recovering;
                }
                break;
            case EnemyState.knockedback:
                if (Mathf.Abs(body.velocity.x) < 0.1f)
                {
                    state = EnemyState.aggro;
                }
                else
                    body.velocity = Vector2.Lerp(body.velocity, Vector2.zero, knockbackDecay);
                break;
            case EnemyState.idle:

                if (TargetInDistance())
                {
                    state = EnemyState.aggro;
                    roamTimer = currentCooldownTimer;
                }

                if (!roamEnabled) // If roaming is disabled, then end the idle state here
                {
                    batStop();
                    break;
                }

                // CHANGE EVERY ELSE IN THIS STATE
                if (roamTimer > 0)
                {
                    idleTimer = idleCooldown;
                    roamTimer -= Time.deltaTime;
                    //Walk(roamDirection * movespeed); // Roam in given direction
                    
                }
                else
                {   // After roaming, pause for a brief time before deciding next direction
                    if (idleTimer > 0)
                    {
                        Walk(0);
                        idleTimer -= Time.deltaTime;
                    }
                    else
                    {
                        roamTimer = roamCooldown; // Reset roaming time
                        roamDirection = Random.Range(-1, 1) >= 0 ? 1 : -1; // Change roaming direction
                        idleTimer = idleCooldown;
                    }
                }
                Walk(0); // Prevents movment during idle for now

                break;
            case EnemyState.aggro:
                // Chase player type-beat
                float distanceFromPlayer = Vector2.Distance(transform.position, target.transform.position);

                if (playerIsHeavyAttacking) // Player is heavy attacking
                {
                    if (distanceFromPlayer < maxAttackRange) // Move to edge of max attack range and wait until player finishes
                        body.velocity = -direction * dashSpeed;
                    else
                        body.velocity = Vector2.zero;
                } 
                else if (distanceFromPlayer > minAttackRange) 
                {
                    // Move toward the player
                    body.velocity = direction * movespeed;
                    if (currentCooldownTimer > 0)
                        currentCooldownTimer -= Time.deltaTime;
                }
                else // Enemy is too close
                {
                    body.velocity = Vector2.zero;
                    if (currentCooldownTimer > 0)
                        currentCooldownTimer -= Time.deltaTime;
                    else
                    {
                        delayTimer = attackDelay;
                        eyeRender.enabled = true;
                        state = EnemyState.charging;
                    }
                }
                break;
            case EnemyState.charging: // Enemy charges for attack
                if (delayTimer > 0)
                {
                    // While charging, don't move or do anything
                    batStop();
                    delayTimer -= Time.deltaTime;
                }
                else
                {   // After enemy finishes charging, set required values and do the attack
                    //attackDirection = bestRoute.x;
                    eyeRender.enabled = false;
                    attackTimer = attackDuration;
                    UpdatePath();
                    batAttack(direction); // Enables hitbox
                    state = EnemyState.attacking;
                }
                break;
            case EnemyState.attacking: // Enemy is in the action of attacking
                if (attackTimer > 0)
                {
                    // Reduced movespeed during attack in the direction of the attack
                    attackTimer -= Time.deltaTime;
                }
                else
                { // After attack is finished, reset enemy values and set to aggro
                    hitbox.disable();
                    hitbox.hasCollided = false;
                    recoveryTimer = recoveryDuration; // Set reoovery time
                    state = EnemyState.recovering; // Change enemy state
                }
                break;
            case EnemyState.recovering: // Enemy recovery time after attacking
                if (recoveryTimer > 0)
                {
                    batStop();
                    recoveryTimer -= Time.deltaTime;
                }
                else
                {
                    currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown); // Reset cooldown
                    state = EnemyState.aggro;
                }
                break;
            case EnemyState.dead:
                animator.SetBool("bat_dead", true);
                break;
        }
        base.FixedUpdate();
    }
    private void batStop()
    {
        body.velocity = Vector2.zero;
    }
    protected override void resetStatus()
    {
        currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown);
        delayTimer = attackDelay;
        attackTimer = attackDuration;
        recoveryTimer = recoveryDuration;
        eyeRender.enabled = false;
    }

    private void batAttack(Vector2 direction)
    {
        body.velocity = direction * dashSpeed;
        hitbox.enable();
    }

    protected override void Die()
    {
        base.Die();

        GameEvents.current.onPlayerIsLightAttack -= updatePlayerHeavyAttacking;
        //knockback(-getFacingDirection(), 5);
        state = EnemyState.dead;
    }

    private void updatePlayerHeavyAttacking(bool state)
    {
        playerIsHeavyAttacking = state;
    }

    protected override void onAggro()
    {
        GameEvents.current.onPlayerIsHeavyAttack += updatePlayerHeavyAttacking;
    }
    */
}

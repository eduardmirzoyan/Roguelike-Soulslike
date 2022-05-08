using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rapier : MeleeWeapon
{
    [Header("Rapier Settings")]
    [SerializeField] private float maxRange = 3f;
    [SerializeField] private ParticleSystem dashParticles;
    [SerializeField] private bool shouldCritDash = false;
    [SerializeField] private float critDashBufferTime = 1f;
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private Sprite activeIndicatorSprite;
    [SerializeField] private Sprite dashIndicatorSprite;

    // Fields
    private Vector3 currentPos;
    private Vector3 targetPos;
    private RaycastHit2D enemyHit;
    private Coroutine resetRoutine;

    private void Start() {
        // Subscribe to on roll
        GameEvents.instance.onRoll += enableDashCrit;
    }

    protected void FixedUpdate()
    {
        switch (state)
        {
            case WeaponState.Ready:
                // Check if enemy is in range
                if (shouldCritDash) {
                    // Raycast for enemy
                    enemyHit = Physics2D.Raycast(transform.position, Vector2.right * wielderMovement.getFacingDirection(), maxRange, 1 << LayerMask.NameToLayer("Enemies"));
                    if (enemyHit) {
                        // Update sprite
                        indicatorRenderer.sprite = dashIndicatorSprite;
                    }
                    else {
                        indicatorRenderer.sprite = activeIndicatorSprite;
                    }
                }
                
                break;
            case WeaponState.WindingUp:
                // Weapon is winding up the attack
                if (windupTimer > 0)
                {
                    wielderMovement.WalkNoTurn(InputBuffer.instance.moveDirection * windUpSpeedMultiplier);
                    windupTimer -= Time.deltaTime;
                }
                else {
                    // Reset attack requests
                    InputBuffer.instance.resetAttackRequests();

                    // If the weapon should crit dash, then do logic for crit dash
                    if (shouldCritDash) {               
                        // Calculate dashspeed here
                        currentPos = wielderMovement.transform.position;

                        // Spawn particles
                        dashParticles.Play();
                    }
                    else {
                        // Do regular attack
                        tempDashspeed = dashspeed;
                    }

                    state = WeaponState.Active;
                }
                break;
            case WeaponState.Active:
                // Weapon is capable of dealing damage, hitbox active
                if (activeTimer > 0)
                {   
                    if (shouldCritDash) {
                        // Dash Towards
                        currentPos = Vector3.Slerp(currentPos, targetPos, 1 - activeTimer / activeDuration);
                        wielderMovement.goTo(currentPos);
                    }
                    else {
                        // Normal dash
                        wielderMovement.dash(tempDashspeed, wielderMovement.getFacingDirection());
                        tempDashspeed = Mathf.Lerp(tempDashspeed, 0, 1 - activeTimer / activeDuration);
                    }

                    activeTimer -= Time.deltaTime;
                }
                else 
                {
                    if (shouldCritDash) {
                        // Set correct final pos
                        wielderMovement.goTo(targetPos);
                        // Reset combo
                        currentCombo = -1;
                        // Reset dash flag
                        shouldCritDash = false;
                    }
                    
                    // Increase combo
                    currentCombo += 1;
                    // Stop moving
                    wielderMovement.Walk(0);
                    // Change state
                    state = WeaponState.Recovering; 
                }
                break;
            case WeaponState.Recovering:
                // Weapon is recovering to ready state
                if (recoveryTimer > 0)
                    recoveryTimer -= Time.deltaTime;
                else {
                    // Reset Combo if you finish recovering
                    animationHandler.changeAnimationState(weaponIdleAnimation);
                    // Reset combo
                    currentCombo = 0;
                    // Change state
                    state = WeaponState.Ready;
                }
                break;
        }
    }

    private void enableDashCrit() {
        // Set should dash flag
        shouldCritDash = true;

        // Start reset timer
        if (resetRoutine != null) {
            // Reset timer
            StopCoroutine(resetRoutine);
        }
        resetRoutine = StartCoroutine(critDashResetTimer(critDashBufferTime));

        // Display indicator
        indicatorRenderer.sprite = activeIndicatorSprite;
    }

    public override void initiateAttack() {
        // Stop movement
        wielderMovement.Walk(0);

        // Check if critDash
        if (shouldCritDash) {
            // Disable timer if running
            if (resetRoutine != null) {
                // Reset timer
                StopCoroutine(resetRoutine);
            }
            // Remove indicator
            indicatorRenderer.sprite = null;

            // Check if enemy is in range
            enemyHit = Physics2D.Raycast(transform.position, Vector2.right * wielderMovement.getFacingDirection(), maxRange, 1 << LayerMask.NameToLayer("Enemies"));
            if (enemyHit) {
                // Get wielder width
                var wielderWidth = wielderMovement.GetComponent<Collider2D>().bounds.size.x;
                            
                // Store target location, which is the entity's bounds + wielder bounds, so the enemy ends up behind the enemy
                targetPos = enemyHit.point + new Vector2(wielderMovement.getFacingDirection() * (enemyHit.collider.bounds.size.x + wielderWidth), 0);
                
                // Set combo to combo 1 animation
                currentCombo = 1;
            }
            else {
                // Else disable crit dash
                shouldCritDash = false;
            }
        }
        
        animationHandler.changeAnimationState(weaponAttackAnimation + " " + currentCombo);
        windupTimer = windupDuration;
        activeTimer = activeDuration;
        recoveryTimer = recoveryDuration;

        state = WeaponState.WindingUp; // Begin attack process
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out Damageable damageable) && collider.gameObject != this.gameObject)
        {
            // Roll for miss
            int roll = Random.Range(0, 100);
            if(roll < (wielderStats.percentMissChance) * 100 )
            {
                PopUpTextManager.instance.createPopup("Miss", Color.gray, collider.transform.position);
                return;
            }

            int damage = (int) (owner.damage * (1 + wielderStats.damageDealtMultiplier));
            var damageColor = Color.white;

            // Check if dash crit
            if (shouldCritDash) {
                damage = (int) (damage * (1 + owner.critDamage));
                damageColor = Color.yellow;
            }

            Damage dmg = new Damage
            {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = transform,
                effects = weaponEffects,
                color = damageColor
            };
            damageable.takeDamage(dmg);
        }
    }

    private IEnumerator critDashResetTimer(float duration) {
        // Wait duration seconds before reseting critDash
        yield return new WaitForSeconds(duration);
        // Reset crit dashing
        shouldCritDash = false;
        indicatorRenderer.sprite = null;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        if (wielderMovement != null)
            Gizmos.DrawWireSphere(wielderMovement.transform.position, maxRange);
    }

}

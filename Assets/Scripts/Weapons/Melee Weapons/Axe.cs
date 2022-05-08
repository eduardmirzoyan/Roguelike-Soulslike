using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MeleeWeapon
{
    [SerializeField] private int consecutiveHitThreshold = 3;
    [SerializeField] private int consecutiveHitCounter = 0;
    [SerializeField] private float hitBufferTime = 0.5f;

    [SerializeField] private bool hasHitEntity = false;

    private void FixedUpdate() {
        switch (state)
        {
            case WeaponState.Ready:
                // Ready to use
                
                break;
            case WeaponState.WindingUp:
                // Weapon is winding up the attack
                if (windupTimer > 0)
                {
                    wielderMovement.WalkNoTurn(InputBuffer.instance.moveDirection * windUpSpeedMultiplier);
                    windupTimer -= Time.deltaTime;
                }
                else {
                    // Reset
                    hasHitEntity = false;

                    tempDashspeed = dashspeed;
                    state = WeaponState.Active; 
                }
                break;
            case WeaponState.Active:
                // Weapon is capable of dealing damage, hitbox active
                if (activeTimer > 0)
                {   
                    // Move while attacking
                    wielderMovement.dash(tempDashspeed, wielderMovement.getFacingDirection());
                    tempDashspeed = Mathf.Lerp(tempDashspeed, 0, 1 - activeTimer / activeDuration);

                    activeTimer -= Time.deltaTime;
                }
                else 
                {
                    // Reset requests
                    InputBuffer.instance.resetAttackRequests();

                    state = WeaponState.Recovering; 
                }
                break;
            case WeaponState.Recovering:
                // Weapon is recovering to ready state
                if (recoveryTimer > 0)
                    recoveryTimer -= Time.deltaTime;
                else {
                    // Reset counter
                    consecutiveHitCounter = 0;

                    // Reset Combo if you finish recovering
                    currentCombo = 0;
                    animationHandler.changeAnimationState(weaponIdleAnimation);
                    state = WeaponState.Ready;
                }
                break;
        }
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

            // Check that counter is above threshold, then make attacks crit
            if (consecutiveHitCounter > consecutiveHitThreshold) {
                damage = (int) (damage * (1 + owner.critDamage));
                damageColor = Color.yellow;
                // Trigger event
                GameEvents.instance.triggerOnCrit(this, damageable.transform);
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

            // Incrmenet
            if (!hasHitEntity) {
                consecutiveHitCounter++;
                hasHitEntity = true;
            }
        }
    }
}

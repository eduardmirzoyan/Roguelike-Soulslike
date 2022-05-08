using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MeleeWeapon
{
    protected void FixedUpdate()
    {
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
                    // Reset attack requests
                    if (transform.parent.TryGetComponent(out InputBuffer inputBuffer)) {
                        inputBuffer.resetAttackRequests();
                    }
                    
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

            // Check for backstab
            if (collider.TryGetComponent(out Movement mv)) {
                // Check to see if both the wielder and target are facing the same direction, if so, then crit
                if (mv.getFacingDirection() == wielderMovement.getFacingDirection()) {
                    damage = (int) (damage * (1 + owner.critDamage));
                    damageColor = Color.yellow;
                }
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

    public override bool canInitiate()
    {
        return state == WeaponState.Ready || state == WeaponState.Recovering;
    }
}

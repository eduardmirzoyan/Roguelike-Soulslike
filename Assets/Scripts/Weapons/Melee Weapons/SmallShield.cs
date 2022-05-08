using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallShield : MeleeWeapon
{
    [SerializeField] private float pushForce;

    private void FixedUpdate() {
        switch (state)
        {
            case WeaponState.Ready:
                // Ready to use
                if (cooldownTimer > 0) {
                    cooldownTimer -= Time.deltaTime;
                }
                
                break;
            case WeaponState.WindingUp:
                // Dash until times up or get near something?

                // Weapon is winding up the attack
                if (windupTimer > 0)
                {
                    windupTimer -= Time.deltaTime;
                }
                else {
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
                    currentCombo += 1;
                    wielderMovement.Walk(0);
                    state = WeaponState.Recovering; 
                }
                break;
            case WeaponState.Recovering:
                // Weapon is recovering to ready state
                if (recoveryTimer > 0)
                    recoveryTimer -= Time.deltaTime;
                else {
                    // Reset Combo if you finish recovering
                    currentCombo = 0;
                    cooldownTimer = cooldown; // Start cooldown
                    animationHandler.changeAnimationState(weaponIdleAnimation);
                    state = WeaponState.Ready;
                }
                break;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out Damageable damageable) &&  collider.gameObject != this.gameObject)
        {
            // Roll for miss
            int roll = Random.Range(0, 100);
            if(roll < (wielderStats.percentMissChance) * 100 )
            {
                PopUpTextManager.instance.createPopup("Miss", Color.gray, collider.transform.position);
                return;
            }

            var damage = (int) (owner.damage * (1 + wielderStats.damageDealtMultiplier));
            var adjustedPush = pushForce;
            var damageColor = Color.white;

            // // Roll for crit
            // roll = Random.Range(0, 100);
            // if(roll <= (wielderStats.percentCritChance + owner.critChance) * 100 )
            // {
            //     // Increase damage
            //     damage = (int) (damage * (1 + owner.critDamage));

            //     // Increase pushforce
            //     adjustedPush = (adjustedPush * (1 + owner.critDamage));

            //     // Change color
            //     damageColor = Color.yellow;
            // }

            Damage dmg = new Damage
            {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = transform,
                effects = weaponEffects,
                pushForce = adjustedPush,
                color = damageColor
            };
            damageable.takeDamage(dmg);

            // Stop dashing
            activeTimer = 0;
        }
    }
}

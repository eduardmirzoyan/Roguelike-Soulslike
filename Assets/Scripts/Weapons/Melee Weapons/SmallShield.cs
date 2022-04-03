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
                // Weapon is winding up the attack
                if (windupTimer > 0)
                {
                    wielderMovement.Walk(0);
                    windupTimer -= Time.deltaTime;
                }
                else {
                    state = WeaponState.Active; 
                }
                break;
            case WeaponState.Active:
                // Weapon is capable of dealing damage, hitbox active
                if (activeTimer > 0)
                {   
                    // Check for enemies hit
                    wielderMovement.Walk(wielderMovement.getFacingDirection() * attackMoveSpeed);

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


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Damageable damageable) &&  collision.gameObject != this.gameObject)
        {
            Damage dmg = new Damage
            {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = transform,
                effects = weaponEffects,
                pushForce = pushForce
            };
            damageable.takeDamage(dmg);
        }
    }
}

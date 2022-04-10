using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MeleeWeapon
{
    [SerializeField] private float bonusDamagePercentage;

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
                    wielderMovement.Walk(0);
                    windupTimer -= Time.deltaTime;
                }
                else {
                    // Spawn trail if possible
                    if(TryGetComponent(out WeaponSwingTrail weaponSwingTrail))
                        weaponSwingTrail.spawnTrail();

                    state = WeaponState.Active; 
                }
                break;
            case WeaponState.Active:
                // Weapon is capable of dealing damage, hitbox active
                if (activeTimer > 0)
                {   
                    // Check for enemies hit
                    // wielderMovement.Walk(wielderMovement.getFacingDirection() * attackMoveSpeed);

                    activeTimer -= Time.deltaTime;
                }
                else 
                {
                    if (transform.parent.TryGetComponent(out InputBuffer inputBuffer)) {
                        inputBuffer.resetAttackRequests();
                    }
                    if (currentCombo == 0) {
                        GameManager.instance.shakeCamera(0.15f, 0.15f);
                    }

                    currentCombo += 1;
                    // wielderMovement.Stop();
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
                    animationHandler.changeAnimationState(weaponIdleAnimation);
                    state = WeaponState.Ready;
                }
                break;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Damageable damageable) && collision.gameObject != this.gameObject)
        {
            int damage = (int) (owner.damage * (1 + wielderStats.damageDealtMultiplier));
            
            // If the hit target is effectable
            if (collision.TryGetComponent(out EffectableEntity effectableEntity)) {

                // If "mark" effect was found and sucessfully removed
                if (effectableEntity.removeEffect(ScriptableObject.CreateInstance<MarkEffect>())) {
                    // Increase damage
                    damage = (int)(damage * (1 + bonusDamagePercentage));
                }
            }

            // Check crit
            int rand = Random.Range(0, 100);
            if(rand < (wielderStats.percentCritChance + owner.critChance) * 100 )
            {
                print("crit!");
                GameManager.instance.CreatePopup("CRIT", transform.parent.position, Color.yellow);
                damage = (int) (damage * (1 + owner.critDamage));
            }

            Damage dmg = new Damage
            {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = transform,
                effects = weaponEffects
            };
            damageable.takeDamage(dmg);
        }
    }
}

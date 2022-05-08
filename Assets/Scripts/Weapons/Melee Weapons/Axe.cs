using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MeleeWeapon
{
    [SerializeField] private float aoeRadius = 1f;
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
                    if (transform.parent.TryGetComponent(out InputBuffer inputBuffer)) {
                        inputBuffer.resetAttackRequests();
                    }

                    // Screenshake for visual effect
                    if (currentCombo == 0) {
                        GameManager.instance.shakeCamera(0.15f, 0.15f);
                    }

                    // Increase combo
                    currentCombo += 1;
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

            // Check if any other enemies are in radius
            var hits = Physics2D.OverlapCircleAll(collider.transform.position, aoeRadius, 1 << LayerMask.NameToLayer("Enemies"));
            foreach (var hit in hits) {
                // If there is another enemy in radius, then crit
                if (hit != collider && hit.TryGetComponent(out Damageable damageable1)) {
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
        if (currentCombo > maxCombo) {
            return false;
        }

        return state == WeaponState.Ready || recoveryTimer <= recoveryDuration / 2;
    }

}

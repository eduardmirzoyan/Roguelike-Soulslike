using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MeleeWeapon
{
    [Header("Dash Values")]
    [SerializeField] private float dashspeed0;
    [SerializeField] private float dashspeed1;
    [SerializeField] private float dashspeed2;

    [Header("Combo Values")]
    [SerializeField] private float windupDuration1;
    [SerializeField] private float activeDuration1;
    [SerializeField] private float recoveryDuration1;
    [SerializeField] private float windupDuration2;
    [SerializeField] private float activeDuration2;
    [SerializeField] private float recoveryDuration2;

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
                    // Increase combo
                    // currentCombo += 1;
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

    public override bool canInitiate() {
        return state == WeaponState.Ready || state == WeaponState.Recovering;
    }

    public override void initiateAttack()
    {
        animationHandler.changeAnimationState(weaponAttackAnimation + " " + currentCombo);

        // Set timer's based on which combo you are one
        switch (currentCombo) {
            case 0:
                windupTimer = windupDuration;
                activeTimer = activeDuration;
                recoveryTimer = recoveryDuration;
                dashspeed = dashspeed0;
            break;
            case 1:
                windupTimer = windupDuration1;
                activeTimer = activeDuration1;
                recoveryTimer = recoveryDuration1;
                dashspeed = dashspeed1;
            break;
            case 2:
                windupTimer = windupDuration2;
                activeTimer = activeDuration2;
                recoveryTimer = recoveryDuration2;
                dashspeed = dashspeed2;
            break;
        }

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

            // Check for 3rd hit
            if (currentCombo == 2) {
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
        }
    }
}

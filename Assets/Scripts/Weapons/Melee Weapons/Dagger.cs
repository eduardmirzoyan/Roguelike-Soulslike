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
                    wielderMovement.Walk(0);
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

    public override bool canInitiate()
    {
        return state == WeaponState.Ready || state == WeaponState.Recovering;
    }
}

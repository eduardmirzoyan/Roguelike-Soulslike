using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
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
                    wielderMovement.Stop();
                    windupTimer -= Time.deltaTime;
                }
                else {
                    // Spawn trail if possible
                    var weaponTrail = GetComponent<WeaponSwingTrail>();
                    if(weaponTrail != null)
                        weaponTrail.spawnTrail();

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
                    wielderMovement.Stop();
                    GameEvents.current.triggerActionFinish(); // Trigger that the weapon has finished attacking
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
}

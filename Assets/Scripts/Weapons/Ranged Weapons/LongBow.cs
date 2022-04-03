using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongBow : RangedWeapon
{
    [Header("Settings")]
    [SerializeField] private string shootAnimation;
    [SerializeField] private float aimRotationSpeed;

    // Whether or not the bow was released from drawback
    private bool isReleased;

    private void FixedUpdate() {
        
        switch (state)
        {
            case WeaponState.Ready:
                // Ready to use
                
                break;
            case WeaponState.WindingUp: // Pulling back the bow is the windup time

                // You may aim the bow during windup
                var trans = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).z;
                if (Input.GetKey(KeyCode.UpArrow) && trans < 45) { // Max angle of -25
                    transform.Rotate(Vector3.forward * aimRotationSpeed * Time.deltaTime);
                }
                else if (Input.GetKey(KeyCode.DownArrow) && trans > -25) { // Min angle of -25
                    transform.Rotate(-Vector3.forward * aimRotationSpeed * Time.deltaTime);
                }

                // Prevent movement
                wielderMovement.Walk(0);
                
                // When the bow is released, switch to active state
                if (isReleased) {
                    // Release is reset for next interations
                    isReleased = false;

                    // Start shoot animation
                    animationHandler.changeAnimationState(shootAnimation); 

                    // Create arrow gameobject
                    var arrow = Instantiate(projectilePrefab, firepoint.position, firepoint.parent.rotation).GetComponent<Arrow>();

                    // Get the actual speed of the arrow
                    var scaledSpeed = (int) (projectileSpeed * cooldownTimer / cooldown);

                    // Initalize the arrow's values
                    if (arrow != null) {
                        arrow.initializeArrow(damage, projectieSizeMult, scaledSpeed * projectieSpeedMult, numberOfPierces, numberOfBounces, transform.parent.gameObject);
                    }
                       
                    state = WeaponState.Active; 
                }
                break;
            case WeaponState.Active: // Firing bow
                wielderMovement.Walk(0);

                if (activeTimer > 0) {   
                    activeTimer -= Time.deltaTime;
                }
                else {
                    state = WeaponState.Recovering; 
                }
                break;
            case WeaponState.Recovering: // Going back to idle
                wielderMovement.Walk(0);

                // Weapon is recovering to ready state
                if (recoveryTimer > 0) {
                    recoveryTimer -= Time.deltaTime;
                }
                else {
                    animationHandler.changeAnimationState(weaponIdleAnimation);
                    state = WeaponState.Ready;
                }
                break;
        }
    }

    public override void initiateAttack()
    {
        // Reset the rotation to its parent's
        transform.rotation = transform.parent.rotation;
                
        animationHandler.changeAnimationState(weaponAttackAnimation + " " + currentCombo);

        // Begin attack process
        state = WeaponState.WindingUp;
    }

    public override void releaseAttack(float time)
    {
        // Release the bow
        isReleased = true;

        // Saves the time for damage calculations
        cooldownTimer = time;
        if (time > cooldown) {
            cooldownTimer = cooldown;
        }

        // Set damage calculations
        damage = (int) (owner.damage * cooldownTimer / cooldown);
        activeTimer = activeDuration;
        recoveryTimer = recoveryDuration;

        // If you have stats, then increase damge
        if (TryGetComponent(out CombatStats stats)) {
            damage = (int) (damage * (1 + stats.damageDealtMultiplier));
        }
    }

    public override bool canInitiate() {
        return state == WeaponState.Ready;
    }

    public override bool canRelease()
    {
        return state == WeaponState.WindingUp && !isReleased;
    }
}

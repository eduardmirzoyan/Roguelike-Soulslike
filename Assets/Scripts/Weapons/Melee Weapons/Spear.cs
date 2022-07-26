using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spear : MeleeWeapon
{
    [Header("Debugging")]
    [SerializeField] private bool isReleased;
    [SerializeField] private float heldTime;
    [SerializeField] private float chargeTime;


    [Header("Settings")]
    [SerializeField] private float maxCharge;
    [SerializeField] private float aimRotationSpeed;
    [SerializeField] private int maxAimDegree = 45;
    [SerializeField] private int minAimDegree = -15;
    [SerializeField] private float projectileSpeed;
    

    [Header("Animation")]
    [SerializeField] private string missingIdleAnimation = "Idle Missing";
    [SerializeField] private string chargeThrowAnimation = "Spear Charge";
    [SerializeField] private string releaseThrowAnimation = "Spear Throw";

    [Header("Components")]
    [SerializeField] private GameObject spearProjectilePrefab;
    [SerializeField] private Transform firepoint;


    protected void FixedUpdate()
    {
        switch (state)
        {
            case WeaponState.Ready:
                // Ready to use if cooldown is over
                if (cooldownTimer > 0) {
                    cooldownTimer -= Time.deltaTime;
                    animationHandler.changeAnimationState(missingIdleAnimation);
                }
                else {
                    animationHandler.changeAnimationState(weaponIdleAnimation);
                }
                
                break;
            case WeaponState.WindingUp:
                // Allow slow horizontal movement
                wielderMovement.WalkNoTurn(InputBuffer.instance.moveDirection * windUpSpeedMultiplier);

                // If you held (not released) the key for more than the windup time of the normal attack then switch into throwing stance
                if (chargeTime > 0 || Time.time - heldTime > windupDuration) {

                    // You may aim the spear during charge up
                    var rotation = GetSignedEulerAngles(gameObject.transform.localEulerAngles);
                    if (Input.GetKey(KeyCode.UpArrow) && rotation.z < maxAimDegree) {
                        transform.Rotate(Vector3.forward * aimRotationSpeed * Time.deltaTime);
                    }
                    else if (Input.GetKey(KeyCode.DownArrow) && rotation.z > minAimDegree) {
                        transform.Rotate(-Vector3.forward * aimRotationSpeed * Time.deltaTime);
                    }

                    // Want to throw
                    animationHandler.changeAnimationState(chargeThrowAnimation);

                    // Force holder to change animations
                    wielderMovement.GetComponent<AnimationHandler>().changeAnimationState(chargeThrowAnimation);

                    if (isReleased) {
                        // Release is reset for next interations
                        isReleased = false;

                        // Start release animation
                        animationHandler.changeAnimationState(releaseThrowAnimation); 

                        // Force holder to change animations
                        wielderMovement.GetComponent<AnimationHandler>().changeAnimationState(releaseThrowAnimation);

                        // Spawn the spear projectile
                        var spear = Instantiate(spearProjectilePrefab, firepoint.position, 
                                firepoint.parent.rotation * Quaternion.Euler(Vector3.forward * 25)).GetComponent<SpearProjectile>();

                        // Get the actual speed of the arrow
                        var scaledSpeed = projectileSpeed * chargeTime / maxCharge;

                        // Calculate damage
                        var damage = (int) (owner.damage * chargeTime / maxCharge);

                        // If you have stats, then increase damge
                        damage = (int) (damage * (1 + wielderStats.damageDealtMultiplier));

                        bool isCrit = false;
                        // If max charged, then give crit
                        if (chargeTime >= maxCharge) {
                            isCrit = true;
                            damage = (int) (damage * (1 + owner.critDamage));
                        }

                        // Initalize the arrow's values
                        if (spear != null) {
                            spear.initializeSpear(damage, isCrit, weaponEffects, scaledSpeed, spriteRenderer.sprite, gameObject);
                        }
                        
                        // Start cooldown
                        cooldownTimer = cooldown;

                        // Reset charge time
                        chargeTime = 0;

                        // Set active time to 0
                        activeTimer = 0.33f;

                        // Set new recovery
                        recoveryTimer = 0;

                        // Reset rotation to parent's
                        transform.rotation = transform.parent.rotation;

                        // Push backwards
                        tempDashspeed = -dashspeed / 3;
                        
                        // Change states
                        state = WeaponState.Active; 
                    }

                }
                else { // Else do a normal melee attack

                    // Weapon is winding up the attack
                    if (windupTimer > 0)
                    {
                        windupTimer -= Time.deltaTime;
                    }
                    else {
                        tempDashspeed = dashspeed;
                        state = WeaponState.Active; 
                    }
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
                    wielderMovement.Walk(0);
                    // Reset requests
                    InputBuffer.instance.resetAttackRequests();

                    // Reset combo
                    currentCombo = 2;
                    
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
                    state = WeaponState.Ready;
                }
                break;
        }
    }

    public override void initiateAttack()
    {
        animationHandler.changeAnimationState(weaponAttackAnimation + " " + currentCombo);
        windupTimer = windupDuration;
        activeTimer = activeDuration;
        recoveryTimer = recoveryDuration;

        // Set released to false
        isReleased = false;

        // Set held time
        heldTime = Time.time;

        state = WeaponState.WindingUp; // Begin attack process
    }

    public override void releaseAttack(float time)
    {
        // Set released to true
        isReleased = true;

        // Store charge time
        if (Time.time - heldTime > windupDuration) {
            chargeTime = time - windupDuration;
            if (chargeTime > maxCharge)
                chargeTime = maxCharge;
        }
            
        // Reset time
        heldTime = float.MaxValue;
    }

    public void reduceCooldown() {
        if (cooldownTimer > 0) {
            cooldownTimer /= 2;
        }
    }

    public override bool canRelease()
    {
        return !isReleased && state == WeaponState.WindingUp;
    }

    public override void cancelAttack()
    {
        if (state != WeaponState.Ready) {
            animationHandler.changeAnimationState(weaponIdleAnimation);
            currentCombo = 0;
            state = WeaponState.Ready;
        }
    }

    private Vector3 GetSignedEulerAngles (Vector3 angles)
    {
        Vector3 signedAngles = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {
            signedAngles[i] = (angles[i] + 180f) % 360f - 180f;
        }
        return signedAngles;
    }
}

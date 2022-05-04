using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LongBow : RangedWeapon
{
    [Header("Components")]
    [SerializeField] private Inventory inventory;

    [Header("Settings")]
    [SerializeField] private string shootAnimation;
    [SerializeField] private float aimRotationSpeed;
    [SerializeField] private int maxAimDegree = 45;
    [SerializeField] private int minAimDegree = -25;

    // Whether or not the bow was released from drawback
    private bool isReleased;

    private void Start() {
        inventory = GetComponentInParent<Player>().GetComponentInChildren<Inventory>();
    }

    private void FixedUpdate() {
        
        switch (state)
        {
            case WeaponState.Ready:
                // Ready to use
                
                break;
            case WeaponState.WindingUp: // Pulling back the bow is the windup time

                // You may aim the bow during windup
                var rotation = GetSignedEulerAngles(gameObject.transform.localEulerAngles);
                if (Input.GetKey(KeyCode.UpArrow) && rotation.z < maxAimDegree) { // Max angle
                    transform.Rotate(Vector3.forward * aimRotationSpeed * Time.deltaTime);
                }
                else if (Input.GetKey(KeyCode.DownArrow) && rotation.z > minAimDegree) { // Min angle
                    transform.Rotate(-Vector3.forward * aimRotationSpeed * Time.deltaTime);
                }

                // Allow slow horizontal movement
                var horizontal = Input.GetAxis("Horizontal");
                if (horizontal > 0.5f || horizontal < -0.5f ) {
                    wielderMovement.dash(20, horizontal);
                }
                else {
                    wielderMovement.Walk(0);
                }
                
                // When the bow is released, switch to active state
                if (isReleased) {
                    // Release is reset for next interations
                    isReleased = false;

                    // Start shoot animation
                    animationHandler.changeAnimationState(shootAnimation); 

                    // Create arrow gameobject
                    var arrow = Instantiate(projectilePrefab, firepoint.position, firepoint.parent.rotation).GetComponent<Arrow>();

                    // Get the actual speed of the arrow
                    var scaledSpeed = projectileSpeed * cooldownTimer / cooldown;

                    // Calculate damage
                    var damage = (int) (owner.damage * cooldownTimer / cooldown);

                    // If you have stats, then increase damge
                    damage = (int) (damage * (1 + wielderStats.damageDealtMultiplier));

                    // Initalize the arrow's values
                    if (arrow != null) {
                        arrow.initializeArrow(damage, projectileSizeMult, scaledSpeed * projectileSpeedMult, numberOfPierces, numberOfBounces, transform.parent.gameObject);
                    }
                       
                    state = WeaponState.Active; 
                }
                break;
            case WeaponState.Active: // Firing bow
                // Prevent movement
                wielderMovement.Walk(0);

                if (activeTimer > 0) {   
                    activeTimer -= Time.deltaTime;
                }
                else {
                    state = WeaponState.Recovering; 
                }
                break;
            case WeaponState.Recovering: // Going back to idle
                // Prevent movement
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
        // Remove 1 arrow from inventory
        inventory.removeItemOfType(ItemType.Ammo);

        // Reset the rotation to its parent's
        transform.rotation = transform.parent.rotation;
                
        animationHandler.changeAnimationState(weaponAttackAnimation + " " + currentCombo);

        // Begin attack process
        state = WeaponState.WindingUp;
    }

    public override void releaseAttack(float time)
    {
        // Saves the time for damage calculations
        cooldownTimer = time;
        if (time > cooldown) {
            cooldownTimer = cooldown;
        }

        // Set damage calculations
        activeTimer = activeDuration;
        recoveryTimer = recoveryDuration;

        // Release the bow
        isReleased = true;
    }

    // User must have arrows in inventory
    public override bool canInitiate() {
        return state == WeaponState.Ready && inventory.getItemOfType(ItemType.Ammo) != null;
    }

    public override bool canRelease()
    {
        return state == WeaponState.WindingUp && !isReleased;
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

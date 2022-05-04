using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rapier : MeleeWeapon
{
    [SerializeField] private float maxRange = 3f;
    [SerializeField] private ParticleSystem dashParticles;

    private Vector2 currentPos;
    private Vector2 targetPos;

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

                    // Calculate dashspeed here
                    currentPos = wielderMovement.transform.position;
                    // Raycast in facing direction to see if there is an enemey
                    var hit = Physics2D.Raycast(transform.position, Vector2.right * wielderMovement.getFacingDirection(), maxRange, 1 << LayerMask.NameToLayer("Enemies"));                    
                    if (hit) {
                        var wielderWidth = wielderMovement.GetComponent<Collider2D>().bounds.size.x;
                        // Target location is the entity's bounds + wielder bounds, so the enemy ends up behind the enemy
                        targetPos = hit.point + new Vector2(wielderMovement.getFacingDirection() * (hit.collider.bounds.size.x + wielderWidth), 0);
                        // Get distance between enemy and user
                        // var distance = Vector2.Distance(wielderPosition, targetPos);
                        
                        // tempDashspeed = distance * dashFactor / activeDuration;
                        print("enemy in range! dashspeed: " + targetPos);
                        Debug.DrawLine(currentPos, targetPos, Color.red, 5f);
                        dashParticles.Play();
                    }
                    else {
                        targetPos = currentPos;
                        print("no enemy in range");
                        // tempDashspeed = 0;
                    }

                    state = WeaponState.Active;
                }
                break;
            case WeaponState.Active:
                // Weapon is capable of dealing damage, hitbox active
                if (activeTimer > 0)
                {   
                    // Move entity
                    currentPos = Vector2.Lerp(currentPos, targetPos, 1 - activeTimer / activeDuration);
                    wielderMovement.goTo(currentPos);

                    // Move while attacking
                    //wielderMovement.dash(tempDashspeed, wielderMovement.getFacingDirection());
                    //tempDashspeed = Mathf.Lerp(tempDashspeed, 0, 1 - activeTimer / activeDuration);

                    activeTimer -= Time.deltaTime;
                }
                else 
                {
                    // Set correct final pos
                    wielderMovement.goTo(targetPos);

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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        if (wielderMovement != null)
            Gizmos.DrawWireSphere(wielderMovement.transform.position, maxRange);
    }

}

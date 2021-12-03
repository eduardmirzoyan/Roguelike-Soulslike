using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BashingShield : MonoBehaviour
{
    [SerializeField] private Damage bashDamage;

    [SerializeField] private GameObject sparkle; // Visual effect for reflecting

    public bool enabledProjectileReflection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (damageable != null && collision.tag != transform.parent.gameObject.tag)
        {
            damageable.takeDamage(bashDamage);
        }

        // If projectile reflection is enabled
        if (enabledProjectileReflection)
        {
            // And a projectile has been collided with
            var projectile = collision.GetComponent<Projectile>();
            if (projectile != null)
            {
                /*// If the projectile has a collider, change it to target enemies now
                var collider = collision.GetComponent<NewCollider>();
                if (collider != null)
                {
                    //Debug.Log(LayerMask.NameToLayer("Enemies"));
                    collider.setNewLayer(128); // 128 is the layer associated with enemies in binary
                    collider.enableHit();
                }*/

                Debug.Log("reversed!");
                // Reverse the direction of the projectile
                projectile.reverseVelocity();
                projectile.setCreator(gameObject);
                // CHANGE THIS!

                /*// And reset the player immunity frames
                var playerDamageable = GetComponentInParent<Damageable>();

                // Transfer knockback into player
                if (playerDamageable != null)
                {
                    playerDamageable.resetImmunityFrames();
                }*/
            }
        }
    }
}

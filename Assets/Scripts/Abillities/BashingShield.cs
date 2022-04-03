using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BashingShield : MonoBehaviour
{
    [SerializeField] private GameObject sparkle; // Visual effect for reflecting

    private Damage bashDamage;
    public bool enabledProjectileReflection;
    public bool enabledStun;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // var damageable = collision.GetComponent<Damageable>();
        // var displace = collision.GetComponent<Displacable>();
        // if (damageable != null && collision.tag != transform.parent.gameObject.tag)
        // {
        //     damageable.takeDamage(bashDamage);
        //     if(displace != null)
        //     {
        //         if (!enabledStun)
        //             displace.triggerKnockback(15, 0.15f, transform.position);
        //         else
        //             displace.triggerStun(1f);
        //     }
        // }

        // If projectile reflection is enabled
        if (enabledProjectileReflection)
        {
            // And a projectile has been collided with
            var projectile = collision.GetComponent<Projectile>();
            if (projectile != null)
            {
                Debug.Log("reversed!");
                // Reverse the direction of the projectile
                // projectile.reverseVelocity();
                projectile.setCreator(transform.parent.gameObject);
                enabledProjectileReflection = false;
            }
        }
    }

    public void setDamage(Damage damage) => bashDamage = damage;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class Arrow : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Projectile projectile;

    [Header("Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float timeTilDestroy = 1f;
    
    private void Awake() {
        projectile = GetComponent<Projectile>();
    }

    public void initializeArrow(int damage, float size, float speed, int pierces, int bounces, GameObject owner) {
        // Set damage
        this.damage = damage;

        // Set projectile
        projectile.initializeProjectile(size, speed, pierces, bounces, owner);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If arrow hits something dmaageableo other than the creator
        if (collision.TryGetComponent(out Damageable damageable) && collision.gameObject != projectile.creator) {
           
            // Deal damage
            Damage dmg = new Damage {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = projectile.creator.transform
            };
            damageable.takeDamage(dmg);

            // If the projectile cannot pierce, make it stuck in enenmy
            if (!projectile.pierce()) {
                 // Disable collider as to not trigger on other entities
                GetComponent<Collider2D>().enabled = false;

                // Make the arrow 'stick' into it's target
                transform.parent = collision.transform;
            
                // Freeze projectile
                projectile.freezePosition();

                // Timed destroy
                Destroy(gameObject, timeTilDestroy);
            }
        }

        // If arrow hit's the ground, then freeze it and destroy it in 1 second
        if (collision.tag == "Ground") {
            // If the projectile cannot bounce
            if (!projectile.bounce()) {
                // Freeze the projectile
                projectile.freezePosition();

                // Timed destroy
                Destroy(gameObject, timeTilDestroy);
            }
        }
    }
}

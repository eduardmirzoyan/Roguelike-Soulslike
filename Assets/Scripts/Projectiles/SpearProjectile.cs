using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class SpearProjectile : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Projectile projectile;
    [SerializeField] private ParticleSystem trail;

    [Header("Settings")]
    [SerializeField] private int damage;
    [SerializeField] private bool isDead;
    
    private void Awake() {
        projectile = GetComponent<Projectile>();
        trail = GetComponentInChildren<ParticleSystem>();
    }

    public void initializeSpear(int damage, float speed, GameObject owner) {
        // Set damage
        this.damage = damage;

        // Set projectile
        projectile.initializeProjectile(1, speed, 0, 0, owner);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If arrow hits something dmaageableo other than the creator
        if (!isDead && collision.TryGetComponent(out Damageable damageable) && collision.gameObject != projectile.creator) {
           
            // Deal damage
            Damage dmg = new Damage {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = projectile.creator.transform
            };
            damageable.takeDamage(dmg);

            // If the projectile cannot pierce, make it stuck in enenmy
            if (!projectile.pierce()) {
                // Once it hits an enemy make it dead
                isDead = true;

                // Make the arrow 'stick' into it's target
                transform.parent = collision.transform;
            
                // Freeze projectile
                projectile.freezePosition();
            }
        }

        // If the creator picked up this spear while it is on the ground, then reduce spear cooldown
        if (collision.gameObject == projectile.creator && isDead) {
            // Remove parent
            trail.transform.parent = null;

            // Destroy gameobject
            Destroy(gameObject);

            // Damage the enemy inside the spear if possible
            if (transform.parent != null && transform.parent.TryGetComponent(out Damageable damageable1)) {
                // Deal half-damage
                Damage dmg = new Damage {
                    damageAmount = damage,
                    source = DamageSource.fromPlayer,
                    origin = projectile.creator.transform
                };
                damageable1.takeDamage(dmg);
            }

            // Reduce the cooldown, if spear is equipped
            var spear = projectile.creator.GetComponentInChildren<Spear>();
            if (spear != null) {
                spear.reduceCooldown();
            }
        }

        // If arrow hit's the ground, then freeze it and destroy it in 1 second
        if (collision.tag == "Ground") {
            // If the projectile cannot bounce
            if (!projectile.bounce()) {
                // Freeze the projectile
                projectile.freezePosition();
            }
            
            // Once it touches the ground, it is dead
            isDead = true;
        }
    }

    private void OnDestroy() {
        // Make copy of it
        if (transform.parent != null) {
            projectile.setVelocity(0);
            projectile.unFreezePosition();
            var obj = Instantiate(gameObject, transform.position, transform.rotation);
        }
            
    }
}

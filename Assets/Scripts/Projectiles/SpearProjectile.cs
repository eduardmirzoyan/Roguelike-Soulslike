using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class SpearProjectile : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Projectile projectile;
    [SerializeField] private ParticleSystem trail;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Settings")]
    [SerializeField] private int damage;
    [SerializeField] private bool isDead;
    [SerializeField] private List<BaseEffect> spearEffects;

    private GameObject wielder;

    private bool isCrit;
    
    private void Awake() {
        projectile = GetComponent<Projectile>();
        trail = GetComponentInChildren<ParticleSystem>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void initializeSpear(int damage, bool isCrit, List<BaseEffect> ownerEffects, float speed, Sprite sprite, GameObject owner) {
        // Set damage
        this.damage = damage;

        // Toggle crit
        this.isCrit = isCrit;

        // Set sprite
        spriteRenderer.sprite = sprite;

        // Set effects
        spearEffects = ownerEffects;

        // Set projectile
        projectile.initializeProjectile(1, speed, 0, 0, owner);

        // Get the spear's wielder
        wielder = owner.transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If arrow hits something dmaageableo other than the creator
        if (!isDead && collision.TryGetComponent(out Damageable damageable) && collision.gameObject != wielder) {
           
           if (isCrit) {
               // Trigger event
                GameEvents.instance.triggerOnCrit(projectile.creator.GetComponent<Weapon>(), damageable.transform);
           }

            // Deal half-damage
            Damage dmg = new Damage {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = projectile.creator.transform,
                effects = spearEffects,
                color = isCrit ? Color.yellow : Color.white
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
        if (collision.gameObject == wielder && isDead) {
            // Remove parent
            trail.transform.parent = null;

            // Destroy gameobject
            Destroy(gameObject);

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

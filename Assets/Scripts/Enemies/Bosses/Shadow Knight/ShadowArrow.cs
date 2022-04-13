using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Projectile))]
public class ShadowArrow : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Projectile projectile;

    [Header("Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float timeTilDestroy;

    protected void Awake()
    {
        projectile = GetComponent<Projectile>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If arrow hits something dmaageableo other than the creator
        if (collision.TryGetComponent(out Damageable damageable) && collision.gameObject != projectile.creator) {
           
            // Deal damage
            Damage dmg = new Damage {
                damageAmount = damage,
                source = DamageSource.fromEnemy,
                origin = projectile.creator.transform,
                color = Color.white
            };
            damageable.takeDamage(dmg);
        }

        // If arrow hit's the ground, then freeze it and destroy it in 1 second
        if (collision.tag == "Ground") {
            // Disable collider as to not trigger on other entities
            GetComponent<Collider2D>().enabled = false;

            // Freeze the projectile
            projectile.freezePosition();

            // Timed destroy
            Destroy(gameObject, timeTilDestroy);
        }
    }
}

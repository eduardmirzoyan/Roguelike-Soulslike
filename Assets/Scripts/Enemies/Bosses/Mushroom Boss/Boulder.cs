using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Projectile projectile;
    [SerializeField] private int boulderDamage;
    [SerializeField] private float timeTilDestroy;
    [SerializeField] private float turnspeed;

    protected void Awake()
    {
        projectile = GetComponent<Projectile>();
    }

    private void FixedUpdate() {
        transform.Rotate(new Vector3(0, 0, turnspeed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If arrow hits something dmaageableo other than the creator
        if (collision.TryGetComponent(out Damageable damageable) && collision.gameObject != projectile.creator) {
           
            // Deal damage
            Damage dmg = new Damage {
                damageAmount = boulderDamage,
                source = DamageSource.fromEnemy,
                origin = projectile.creator.transform,
                color = Color.red
            };
            damageable.takeDamage(dmg);
        }

        // If arrow hit's the ground, then freeze it and destroy it in 1 second
        if (collision.tag == "Ground") {
            // Reduce the velocity
            GetComponent<Rigidbody2D>().velocity /= 6;

            // Disable collider as to not trigger on other entities
            GetComponent<Collider2D>().enabled = false;

            // Timed destroy
            Destroy(gameObject, timeTilDestroy);
        }
    }
}

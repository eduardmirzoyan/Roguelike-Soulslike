using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class Fireball : MonoBehaviour
{
    [SerializeField] protected Damage fireballDamage;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (damageable != null && collision.tag != GetComponent<Projectile>().creator.tag)
        {
            damageable.takeDamage(fireballDamage);
            Destroy(gameObject);
        }

        if (collision.tag == "Ground")
            Destroy(gameObject);
    }
}

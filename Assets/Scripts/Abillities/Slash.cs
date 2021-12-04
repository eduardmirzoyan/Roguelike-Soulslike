using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class Slash : MonoBehaviour
{
    [SerializeField] private Damage slashDamage;
    [SerializeField] public int numberOfPeirces;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (collision.gameObject != this.gameObject && damageable != null && collision.tag == "Enemy")
        {
            damageable.takeDamage(slashDamage);

            if (numberOfPeirces <= 0)
                Destroy(gameObject);
            numberOfPeirces--;
        }

        if (collision.tag == "Ground")
            Destroy(gameObject);
    }
}
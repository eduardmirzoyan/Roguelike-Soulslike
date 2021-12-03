using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    [SerializeField] private Damage boulderDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (collision.gameObject != this.gameObject && damageable != null && collision.tag != "Enemy")
        {
            damageable.takeDamage(boulderDamage);
        }

        if (collision.tag == "Ground")
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false; // Disable collider so it doesn't damage anymore
        }
    }
}

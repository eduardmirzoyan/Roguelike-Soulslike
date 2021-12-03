using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Projectile))]
public class ShadowArrow : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Damage arrowDamage;

    // Start is called before the first frame update
    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (collision.gameObject != this.gameObject && damageable != null && collision.tag != "Enemy")
        {
            damageable.takeDamage(arrowDamage);
        }

        if (collision.tag == "Ground")
        {
            body.isKinematic = true;
            body.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false; // Disable collider so it doesn't damage anymore
        }
    }
}

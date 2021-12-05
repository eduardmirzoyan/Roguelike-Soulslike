using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class MagicBolt : MonoBehaviour
{
    [SerializeField] private Damage boltDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (damageable != null && collision.tag != GetComponent<Projectile>().creator.tag)
        {
            damageable.takeDamage(boltDamage);
            Destroy(gameObject);
        }

        if (collision.tag == "Ground")
            Destroy(gameObject);
    }
}

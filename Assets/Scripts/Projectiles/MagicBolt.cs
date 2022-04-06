using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class MagicBolt : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private int damage;
    [SerializeField] private Transform target;

    private void Start() {
        projectile = GetComponent<Projectile>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (collision != null && damageable != null && target != null && collision.transform == target)
        {
            Damage dmg = new Damage {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = projectile.creator.transform
            };
            damageable.takeDamage(dmg);

            // Destroy this bolt
            Destroy(gameObject);
        }

        if (collision.tag == "Ground")
            Destroy(gameObject);
    }

    public void setTarget(Transform transform) {
        target = transform;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class EnemyWeapon : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyAI wielder;

    [Header("Settings")]
    [SerializeField] private int damage = 5;
    [SerializeField] private float force = 0;
    [SerializeField] private bool hitAll;

    private void Start() {
        wielder = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (hitAll || other.transform == wielder.getTarget()) {
            if (other.TryGetComponent(out Damageable damageable)) {
                Damage dmg = new Damage {
                    damageAmount = damage,
                    source = DamageSource.fromEnemy,
                    origin = transform.parent,
                    pushForce = force
                };
                damageable.takeDamage(dmg);
            }
        }
    }

    public void setDamage(int dmg) {
        damage = dmg;
    }

    public void setPushForce(int fce) {
        force = fce;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private EnemyAI wielder;
    [SerializeField] private Damage dmg;

    private void Start() {
        wielder = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform == wielder.getTarget()) {
            if (other.TryGetComponent(out Damageable damageable)) {
                damageable.takeDamage(dmg);
            }
        }
    }

}

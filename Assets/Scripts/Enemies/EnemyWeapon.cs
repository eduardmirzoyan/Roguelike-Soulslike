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

    private void OnTriggerEnter2D(Collider2D collider) {
        if (hitAll || collider.transform == wielder.getTarget()) {
            if (collider.TryGetComponent(out Damageable damageable)) {
                // Roll for miss
                if (wielder.TryGetComponent(out Stats wielderStats)) {
                    int rand = Random.Range(0, 100);
                    if(rand < (wielderStats.percentMissChance) * 100 )
                    {
                        PopUpTextManager.instance.createPopup("Miss", Color.gray, collider.transform.position);
                        return;
                    }
                }

                Damage dmg = new Damage {
                    damageAmount = damage,
                    source = DamageSource.fromEnemy,
                    origin = transform.parent,
                    pushForce = force,
                    color = Color.red
                };
                damageable.takeDamage(dmg);
            }
        }
    }

    public int getDamage() {
        return damage;
    }

    public void setDamage(int dmg) {
        damage = dmg;
    }

    public void setPushForce(int fce) {
        force = fce;
    }
}

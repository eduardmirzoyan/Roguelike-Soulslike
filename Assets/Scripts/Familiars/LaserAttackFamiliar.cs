using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttackFamiliar : Familiar
{
    [Header("Close Ranger Attacker Settings")]
    [SerializeField] private float attackRadius;
    [SerializeField] private LineOfSight lineOfSight;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform target;

    [Header("Firing Settings")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private ParticleSystem chargingParticles;
    [SerializeField] private int laserDamage;
    [SerializeField] private float fireRate;
    [SerializeField] private float chargeRate;
    private float fireTimer;
    private float chargeTimer;
    private bool isCharging;

    protected override void Start()
    {
        base.Start();
        lineOfSight = GetComponent<LineOfSight>();
        chargingParticles = GetComponentInChildren<ParticleSystem>();

        fireTimer = fireRate;
        GameEvents.instance.onHit += setTarget;
    }

    protected void FixedUpdate()
    {
        if (target != null)
        {
            // If target goes out of range, then dip
            if (Vector3.Distance(transform.position, target.position) > attackRadius) {
                target = null;
                return;
            }

            // Aim attack
            aimAtTarget();

            if (!isCharging) {
                if (fireTimer > 0) {
                    fireTimer -= Time.deltaTime;
                }
                else if (chargeTimer <= 0) // Only if target is in range
                {
                    // Play particles
                    chargingParticles.Play();

                    // Start charging
                    chargeTimer = chargeRate;
                    isCharging = true;
                }
            }
            else {
                if (chargeTimer > 0) {
                    chargeTimer -= Time.deltaTime;
                }
                else {
                    // Fire beam at target
                    var beam = Instantiate(laserPrefab, transform.position, Quaternion.identity).GetComponent<Laserbeam>();
                    beam.fireAt(target);

                    if (target.TryGetComponent(out Damageable damageable)) {
                        // Deal damage
                        Damage dmg = new Damage {
                            damageAmount = laserDamage,
                            source = DamageSource.fromPlayer,
                            origin = owner.transform
                        };
                        damageable.takeDamage(dmg);
                    }

                    // Reset rotation
                    transform.rotation = owner.transform.rotation;
                    firePoint.rotation = owner.transform.rotation;

                    // Reset attack rate
                    isCharging = false;
                    fireTimer = fireRate;
                    isCharging = false;
                }
            }
        }
    }

    private void setTarget(GameObject attacker, GameObject hit, int damage) {
        // If the owner hit something, then set it as this familar's target
        if (attacker.gameObject == owner.gameObject && damage > 0) {
            // Set target
            target = hit.transform;

            // Reset fireTime
            fireTimer = fireRate;
        }
    }

    private void aimAtTarget()
    {
        Vector2 direction = target.position - firePoint.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(Vector3.forward * angle);
        // Set it's own rotation towards target
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }

    // Draws radius of familair
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}

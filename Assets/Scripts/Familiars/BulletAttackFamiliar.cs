using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttackFamiliar : Familiar
{ 
    [Header("Close Ranger Attacker Settings")]
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform target;

    [Header("Firing Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate;
    private float fireTimer;

    protected override void Start()
    {
        base.Start();
        fireTimer = fireRate;
        GameEvents.instance.onHit += setTarget;
    }

    protected void FixedUpdate()
    {
        if (target != null)
        {
            if (fireTimer > 0)
                fireTimer -= Time.deltaTime;
            else if (Vector3.Distance(transform.position, target.position) < attackRadius) // Only if target is in range
            {
                aimAtTarget();
                var bolt = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
                bolt.GetComponent<Projectile>().setCreator(owner.gameObject);
                bolt.GetComponent<MagicBolt>().setTarget(target);

                // Reset attack rate
                fireTimer = fireRate;
            }
            
            // If target is out of range, then attack it
            if (Vector3.Distance(transform.position, target.position) > attackRadius) {
                target = null;
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
        Vector2 direction = target.position - firePoint.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }

    // Draws radius of familair
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}

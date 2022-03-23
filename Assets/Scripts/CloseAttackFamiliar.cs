using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAttackFamiliar : Familiar
{
    [Header("Close Ranger Attacker Settings")]
    [SerializeField] private float attackRadius;
    [SerializeField] private float minAttackRadius;
    [SerializeField] private LayerMask targetLayer;
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
    }

    protected override void Update()
    {
        base.Update();
        var hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, targetLayer);
        if(hits != null)
        {
            target = GetClosestEnemyOutsideInnerRadius(hits);
        }
        else
        {
            target = null;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (target != null)
        {
            if (fireTimer > 0)
                fireTimer -= Time.deltaTime;
            else
            {
                aimAtTarget();
                var bolt = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
                bolt.GetComponent<Projectile>().setCreator(owner.gameObject);
                fireTimer = fireRate;
            }
        }
    }

    private Transform GetClosestEnemyOutsideInnerRadius(Collider2D[] enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Collider2D coll in enemies)
        {
            float dist = Vector3.Distance(coll.transform.position, currentPos);
            if (dist >= minAttackRadius && dist < minDist)
            {
                tMin = coll.transform;
                minDist = dist;
            }
        }
        return tMin;
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
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.DrawWireSphere(transform.position, minAttackRadius);
    }
}

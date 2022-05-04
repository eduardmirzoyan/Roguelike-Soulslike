using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Transform creator;
    [SerializeField] private int damage;
    [SerializeField] private Transform target;
    [SerializeField] private float timeTilHoming = 1f;
    [SerializeField] private float travelSpeed = 10f;
    [SerializeField] private float rotateSpeed = 200f;

    private float homingTimer = 0f;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        if (homingTimer > 0) {
            homingTimer -= Time.deltaTime;
        }
        else {
            // Face target
            Vector2 direction = (Vector2)target.position - body.position;

            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.right).z;

            body.angularVelocity = -rotateAmount * rotateSpeed;

            body.velocity = transform.right * travelSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (homingTimer < 0 && collision != null && damageable != null && target != null && collision.transform == target)
        {
            Damage dmg = new Damage {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = creator,
                color = Color.white
            };
            damageable.takeDamage(dmg);

            // Destroy this bolt
            Destroy(gameObject);
        }
    }

    public void intialize(int damage, Transform newTarget, Transform creator) {
        // Intialize values
        this.damage = damage;
        this.target = newTarget;
        this.creator = creator;

        // Start timer
        homingTimer = timeTilHoming;

        // Start with random direction velocity
        body.velocity =  new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * travelSpeed;

        // Set angle based on velocity direction
        float angle = Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows components to fly through the air
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] public GameObject creator;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private int numberOfBounces = 0;
    [SerializeField] private int numberOfPierces = 0;


    [Header("Velocity Gain?")]
    [SerializeField] private bool gainVelocityOverTime;
    [SerializeField] private float velocityGainRate;
    [SerializeField] private float percentVelocityGain;

    [Header("Gravity?")]
    [SerializeField] private bool enableGravity;

    private float gainTimer;

    // Start is called before the first frame update
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.velocity = transform.right * projectileSpeed;
        body.isKinematic = !enableGravity;
    }

    private void FixedUpdate()
    {
        if (gainVelocityOverTime)
        {
            if (gainTimer > 0)
                gainTimer -= Time.deltaTime;
            else
            {
                body.velocity *= percentVelocityGain;
                gainTimer = velocityGainRate;
            }
        }
    }

    public void initializeProjectile(float size, float speed, int pierces, int bounces, GameObject owner) {
        // Change size
        transform.localScale *= size;

        // Set speed
        projectileSpeed = speed;

        // Set pierces
        numberOfPierces = pierces;

        // Set bounces
        numberOfBounces = bounces;

        // Set owner
        creator = owner;
    }

    public bool bounce() {
        if (numberOfBounces > 0) {

            var hit = Physics2D.Raycast(transform.position, body.velocity.normalized, 1f, 1 << LayerMask.NameToLayer("Ground"));
            if (hit)
            {
                var newDirection = Vector3.Reflect(body.velocity.normalized, hit.normal);
                body.velocity = body.velocity.magnitude * newDirection;
            }

            numberOfBounces--;
            return true;
        }
        return false;
    }

    public bool pierce() {
        if (numberOfPierces > 0) {
            numberOfPierces--;
            return true;
        }
        return false;
    }

    public void setVelocity(int speed) {
        projectileSpeed = speed;
    }

    public float getVelocity() {
        return projectileSpeed;
    }

    public void freezePosition() {
        enableGravity = false;
        body.isKinematic = true;
        body.velocity = Vector2.zero;
    }

    private void Update() {
        if (enableGravity) {
            turn();
        }
    }

    // Reverse the velocity of the projectile
    public void reverseVelocity()
    {
        body.velocity = -body.velocity.normalized * projectileSpeed;
        transform.Rotate(0f, 180f, 0f);
    }

    private void turn() {
        float angle = Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void setCreator(GameObject gameObject) => creator = gameObject;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows components to fly through the air
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private int projectileSpeed;
    [SerializeField] private Rigidbody2D body;

    [SerializeField] private bool gainVelocityOverTime;
    [SerializeField] private float velocityGainRate;
    [SerializeField] private float percentVelocityGain;

    [SerializeField] public GameObject creator { get; private set; }

    private float gainTimer;

    // Start is called before the first frame update
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.velocity = transform.right * projectileSpeed;
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

    // Reverse the velocity of the projectile
    public void reverseVelocity()
    {
        body.velocity = -body.velocity.normalized * projectileSpeed;
        transform.Rotate(0f, 180f, 0f);
    }

    public void setCreator(GameObject gameObject) => creator = gameObject;
}

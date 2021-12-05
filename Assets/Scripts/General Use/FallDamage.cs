using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class FallDamage : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Movement mv;
    [SerializeField] private float fallHeight;

    [Header("Adjustable Settings")]
    [SerializeField] public bool enabled;
    [SerializeField] private float maxDropDistance;
    [SerializeField] private Damage fallDamage;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        mv = GetComponent<Movement>();
    }

    private void FixedUpdate()
    {
        if (enabled)
        {
            if (!mv.isGrounded())
            {
                if (body.velocity.y > 0)
                    fallHeight = transform.position.y;
            }
            else
            {
                if (fallHeight - transform.position.y >= maxDropDistance)
                {
                    // Deal damage, make this scale or something
                    var damageable = GetComponent<Damageable>();
                    if (damageable != null)
                        damageable.takeDamage(fallDamage);
                }
                fallHeight = transform.position.y;
            }
        }
    }


}

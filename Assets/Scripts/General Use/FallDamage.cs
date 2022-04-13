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
    [SerializeField] public bool enableFallDamage;
    [SerializeField] private float maxDropDistance;
    [SerializeField] private int fallDamage;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        mv = GetComponent<Movement>();
    }

    private void FixedUpdate()
    {
        if (enableFallDamage)
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
                    // Deal damage
                    if (TryGetComponent(out Damageable damageable)) {
                        // Take damage based on how much you feel
                        Damage dmg = new Damage {
                            damageAmount = (int) (fallDamage * (fallHeight / maxDropDistance)),
                            source = DamageSource.fromEnvironment,
                            origin = transform,
                            color = Color.red
                        };
                        damageable.takeDamage(dmg);
                    }
                        
                }
                fallHeight = transform.position.y;
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDropDistance);
    }

}

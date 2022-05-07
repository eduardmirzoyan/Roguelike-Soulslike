using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStun : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Displacable displacable;
    [SerializeField] private Health health;

    [Header("Settings")]
    [SerializeField] private float damageRequiredRatio = 0.25f;
    [SerializeField] private float bufferDuration = 0.5f;
    [SerializeField] private float pushforce = 400f;
    [SerializeField] private float pushDuration = 0.25f;

    private int damageBuildUp;
    private float bufferTimer;

    // Start is called before the first frame update
    private void Awake()
    {
        displacable = GetComponent<Displacable>();
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (bufferTimer > 0) {
            bufferTimer -= Time.deltaTime;
        }
        else {
            // Reset damage buildup
            damageBuildUp = 0;
            bufferTimer = 0;
        }
    }

    public void increment(int amount, Vector2 origin) {
       
        // Add build up
        damageBuildUp += amount;
        // Reset buffer
        bufferTimer = bufferDuration;

        // If you pass threshold, then trigger knockback
        if (damageBuildUp >= health.getMaxHP() * damageRequiredRatio) {

            // Trigger hitstun, ONLY IF the entity is not already displaced
            if (!displacable.isDisplaced()) {
                var enemy = GetComponent<EnemyAI>();

                // If its an enemy, make sure it is get knocked back during its attack
                if (enemy == null || (enemy != null && !enemy.isAttacking())) {
                    displacable.triggerKnockback(pushforce, pushDuration, origin);
                }
            }
            
            // Reset values
            damageBuildUp = 0;
            bufferTimer = 0;
        }
    }
}

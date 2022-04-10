using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Health health;

    [Header("Settings")]
    [SerializeField] private float immunityDuration = 0.5f;
    [SerializeField] private bool isInvincible;
    [SerializeField] private float immunityTimer = 0f;
    [SerializeField] private Color defaultColor;

    [SerializeField] private GameObject tempDamageEffect;

    private void Start() {
        health = GetComponent<Health>();
    }

    private void FixedUpdate()
    {
        if (immunityTimer > 0)
            immunityTimer -= Time.deltaTime;
    }

    public void takeDamage(Damage damage)
    {
        if (health == null || health.isEmpty()) // If entity does not have health, then dip
            return;

        // If entity is invicible or is still in I-frames, skip damage calculation
        if (isInvincible || immunityTimer > 0)
            return;

        // Check dodge and block chance
        if(TryGetComponent(out CombatStats stats))
        {
            // RNG test
            int rand = Random.Range(0, 100);
            if(rand <= stats.percentDodgeChance * 100)
            {
                // Dodge ATTACK!
                return;
            }
        }

        // Reduce damage if possible
        int correctedDamage = reduceDamageBasedOnStats(damage.damageAmount);

        // Reduce hp
        health.reduceHealth(correctedDamage);

        // Visual feedback
        if (damage.color.a == 0) // If the damage does not have a color, then use default color
            GameManager.instance.CreatePopup(correctedDamage.ToString(), transform.position, defaultColor);
        else
            GameManager.instance.CreatePopup(correctedDamage.ToString(), transform.position, damage.color);

        // Damage particles if possible
        if (TryGetComponent(out DamageParticles damageParticles))
            damageParticles.spawnDamageParticles();

        // Flash if applicible
        if (TryGetComponent(out DamageFlash damageFlash))
            damageFlash.Flash();

        // Add any direct Status effets
        if (TryGetComponent(out EffectableEntity effectable) && damage.effects != null)
        {
            // Add every effect in the damage
            foreach (BaseEffect effect in damage.effects)
            {
                effectable.addEffect(effect.InitializeEffect(gameObject));
            }
        }
        
        // Check for knockback
        if (TryGetComponent(out Displacable displacable)) {
            if (damage.pushForce > 0) {
                displacable.triggerKnockback(damage.pushForce, 0.33f, damage.origin.position);
            }
        }

        if (tempDamageEffect != null) {
            Instantiate(tempDamageEffect, transform.position, Quaternion.identity);
        }

        // Convert the origin to the wielder if the origin is a weapon
        if (damage.origin.TryGetComponent<Weapon>(out Weapon weapon)) {
            damage.origin = damage.origin.parent;
        }

        // If the entity hit is an enemy
        if (TryGetComponent(out EnemyAI enemy))
        {   
            // Signal that the enemy was attacked
            enemy.isAttacked(damage.origin);
        }

        // Trigger onhit event
        GameEvents.instance.triggerOnHit(damage.origin.gameObject, gameObject, correctedDamage);

        immunityTimer = immunityDuration;
    }

    // Reduce damage based on stats if possible
    private int reduceDamageBasedOnStats(int damage) 
    {
        if (TryGetComponent(out CombatStats stats)) 
        {
            int reducedStandardDamage = Mathf.RoundToInt(damage * (1 - stats.damageTakenMultiplier)) - stats.defense / 4;
            return Mathf.Max(reducedStandardDamage, 1); // Min reduced damage to take is 1
        }
        return damage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Stats))]
public class Damageable : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Health health;
    [SerializeField] private Stats stats;

    [Header("Settings")]
    [SerializeField] private float immunityDuration = 0.5f;
    [SerializeField] private bool isInvincible;
    [SerializeField] private float immunityTimer = 0f;

    private void Start() {
        health = GetComponent<Health>();
        stats = GetComponent<Stats>();
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

        // Roll to see if damage is dodged
        int roll = Random.Range(0, 100);
        if(roll < stats.percentDodgeChance * 100)
        {
            PopUpTextManager.instance.createPopup("Dodged", Color.cyan, transform.position);
            // Dodge ATTACK!
            return;
        }

        // Reduce damage if possible
        int correctedDamage = reduceDamageBasedOnStats(damage.damageAmount);

        // Reduce hp
        health.reduceHealth(correctedDamage);

        // Damage popup
        PopUpTextManager.instance.createPopup(correctedDamage.ToString(), damage.color, transform.position);

        // Convert the origin to the wielder if the origin is a weapon
        if (damage.origin.TryGetComponent<Weapon>(out Weapon weapon)) {
            // Trigger weapon hit
            GameEvents.instance.triggerOnWeaponHit(weapon, gameObject);
            
            damage.origin = damage.origin.parent;
        }

        // Damage particles if possible
        if (TryGetComponent(out DamageParticles damageParticles))
            damageParticles.spawnDamageParticles();

        // Flash if applicible
        if (TryGetComponent(out DamageFlash damageFlash))
            damageFlash.Flash();
        
        // Increment hitstun if possible
        if (TryGetComponent(out HitStun hitStun)) {
            hitStun.increment(correctedDamage, damage.origin.position);
        }

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

        // If the entity hit is an enemy
        if (TryGetComponent(out EnemyAI enemy))
        {   
            // Signal that the enemy was attacked
            enemy.isAttacked(damage.origin);
        }

        // Trigger onhit event
        GameEvents.instance.triggerOnHit(damage.origin.gameObject, gameObject, correctedDamage);

        // Reset I-frames
        immunityTimer = immunityDuration;
    }

    // Reduce damage based on stats if possible
    private int reduceDamageBasedOnStats(int damage) 
    {   
        // Reduce damage by defense
        int reducedDamage = Mathf.RoundToInt(damage * (1 - stats.defense / 100f));

        // Reduce damage by multiplier
        reducedDamage = Mathf.RoundToInt(reducedDamage * (1 - stats.damageTakenMultiplier));
        
        // Minimum damage to take is 1
        return Mathf.Max(reducedDamage, 1);
    }
}

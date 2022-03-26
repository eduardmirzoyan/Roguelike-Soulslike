using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] protected float immunityDuration = 0.5f;
    [SerializeField] protected bool isInvincible;
    [SerializeField] private float immunityTimer = 0f;
    [SerializeField] private Color defaultColor;

    private void FixedUpdate()
    {
        if (immunityTimer > 0)
            immunityTimer -= Time.deltaTime;
    }

    public void takeDamage(Damage damage)
    {
        // Check for health
        var health = GetComponent<Health>();
        if (health == null || health.isEmpty()) // If entity does not have health, then dip
            return;

        if (damage.triggersIFrames)
        {
            // Check frames

            // If entity is invicible or is still in I-frames, skip damage calculation
            if (isInvincible || immunityTimer > 0)
                return;
        }

        // Calculate damage if attack is not avoided...

        // Reduce damage if possible
        int correctedDamage = reduceDamageBasedOnStats(damage.damageAmount);

        if (damage.isTrue)
        {
            // Reset damage calculation
            correctedDamage = damage.damageAmount;
        }

        if (damage.isAvoidable)
        {
            // Check dodge and block chance

            var stats = GetComponent<CombatStats>();
            if(stats != null)
            {
                int rand = Random.Range(0, 100);
                if(rand <= stats.percentDodgeChance)
                {
                    // Dodge ATTACK!
                    GameManager.instance.CreatePopup("DODGED", transform.position, Color.cyan);
                    return;
                }
            }

            var shield = GetComponentInChildren<Shield>();
            // If you hit a shield then block the damage if conditions are met
            if (shield != null && shield.isActive) // If the damageable has a shield and is enabled and the attack is not perilous...
            {
                // Need to fix this?
                if (shield.checkIfShieldShouldBlock(damage.origin.position))
                {
                    shield.blockDamage(damage);
                    immunityTimer = immunityDuration;
                    return;
                }
            }
        }

        // Universal Calculations

        // Reduce hp
        health.reduceHealth(correctedDamage);

        // Visual feedback
        if (damage.color.a == 0) // If the damage does not have a color, then use default color
            GameManager.instance.CreatePopup(damage.damageAmount.ToString(), transform.position, defaultColor);
        else
            GameManager.instance.CreatePopup(damage.damageAmount.ToString(), transform.position, damage.color);

        // Reduce poise if applicible
        // var poise = GetComponent<Poise>();
        // if (poise != null)
        //     poise.damagePoise(5, damage.origin.position);

        // Damage particles if possible
        var damageParticles = GetComponent<DamageParticles>();
        if (damageParticles != null)
            damageParticles.spawnParticles();

        // Flash if applicible
        var damageFlash = GetComponent<DamageFlash>();
        if (damageFlash != null)
            damageFlash.Flash();

        // Add any direct Status effets
        var effectable = GetComponent<EffectableEntity>();
        if (effectable != null && damage.effects != null)
        {
            // Add every effect in the damage
            foreach (BaseEffect effect in damage.effects)
            {
                effectable.addEffect(effect.InitializeEffect(gameObject));
            }
        }

        // Add any buildUps
        var buildUp = GetComponent<EffectBuildupHandler>();
        if (buildUp != null && damage.buildupEffects != null)
        {
            // Add every effect in the damage
            foreach (BuildupEffect buildUpEffect in damage.buildupEffects)
            {
                buildUp.addEffectBuildUp(buildUpEffect); // Need to rework build up handling...
            }
        }

        // If the entity hit is an enemy
        var enemy = GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // Signal that the enemy was attacked
            enemy.isAttacked(damage.origin);

            // Trigger onhit event
            GameEvents.current.triggerOnHit();

            // If enemy is damage'd while idle from the player, aggro the enemy
            if (enemy.isIdle() && damage.source == DamageSource.fromPlayer)
                enemy.onAggro();
        }

        if (damage.triggersIFrames)
        {
            // Reset I-frames
            immunityTimer = immunityDuration;
        }
    }

    // Reduce damage based on stats if possible
    private int reduceDamageBasedOnStats(int damage)
    {
        var stats = GetComponent<CombatStats>();
        if (stats != null)
        {
            int reducedStandardDamage = Mathf.RoundToInt(damage * (1 - stats.damageTakenMultiplier)) - stats.defense / 4;
            return Mathf.Max(reducedStandardDamage, 1); // Min reduced damage to take is 1
        }
        else
            return damage;
    }

    public void resetImmunityFrames() => immunityTimer = immunityDuration;
}

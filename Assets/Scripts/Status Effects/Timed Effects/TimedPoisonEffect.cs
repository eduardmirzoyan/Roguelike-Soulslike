using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPoisonEffect : TimedEffect
{
    private Health health;
    private Rigidbody2D rigidbody2D;
    private ParticleSystem poisonParticles;

    public TimedPoisonEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        health = parent.GetComponent<Health>();
        rigidbody2D = parent.GetComponent<Rigidbody2D>();

    }

    public override void End()
    {
        // Does nothing on end
    }

    protected override void ApplyEffect()
    {
        PoisonEffect poisonEffect = (PoisonEffect)Effect;
        // Create fire particles
        poisonParticles = GameObject.Instantiate(poisonEffect.poisonedParticles, health.transform).GetComponent<ParticleSystem>();
        // Set duration
        var main = poisonParticles.main;
        main.duration = Effect.Duration;
        poisonParticles.Play();
    }

    protected override void onTick()
    {
        PoisonEffect poisonEffect = (PoisonEffect)Effect;
        if (health != null) {
            // If the entitiy is moving, increase the tickrate
            if (rigidbody2D.velocity.magnitude < 1f) {
                poisonEffect.tickRate = 1f;
            }
            else {
                poisonEffect.tickRate = 0.5f;
            }

            // Reduce hp
            health.reduceHealth(poisonEffect.tickDamage);

            // Create popup
            PopUpTextManager.instance.createWeakPopup(poisonEffect.tickDamage + "", poisonEffect.damageColor, health.transform.position);
        }
    }
}

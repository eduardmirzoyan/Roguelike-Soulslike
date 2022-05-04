using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBurnEffect : TimedEffect
{
    private Health health;
    private Rigidbody2D rigidbody2D;
    private ParticleSystem fireParticles;

    public TimedBurnEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
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
        BurnEffect burnEffect = (BurnEffect)Effect;
        // Create fire particles
        fireParticles = GameObject.Instantiate(burnEffect.onFireParticles, health.transform).GetComponent<ParticleSystem>();
        // Set duration
        var main = fireParticles.main;
        main.duration = Effect.Duration;
        fireParticles.Play();
    }

    protected override void onTick()
    {
        BurnEffect burnEffect = (BurnEffect)Effect;
        
        if (health != null) {
            // If the entitiy is standing still, increase the tickrate
            if (rigidbody2D.velocity.magnitude < 1f) {
                burnEffect.tickRate = 0.5f;
            }
            else {
                burnEffect.tickRate = 1f;
            }

            health.reduceHealth(burnEffect.tickDamage);

            // Create popup
            PopUpTextManager.instance.createWeakPopup(burnEffect.tickDamage + "", burnEffect.damageColor, health.transform.position);
        }
    }
}

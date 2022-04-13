using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBurnEffect : TimedEffect
{
    private Health health;
    private Rigidbody2D rigidbody2D;

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
        // Does nothing on apply
        BurnEffect burnEffect = (BurnEffect)Effect;

        // Create fire particles
        // var ps = GameObject.Instantiate(burnEffect.onFireParticles, health.transform).GetComponent<ParticleSystem>();
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
            PopUpTextManager.instance.createShortPopup(burnEffect.tickDamage + "", burnEffect.damageColor, health.transform.position);
        }
    }
}

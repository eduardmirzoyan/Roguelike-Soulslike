using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBleedEffect : TimedEffect
{
    private Health health;
    private ParticleSystem bleedParticles;

    public TimedBleedEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        health = parent.GetComponent<Health>();
    }

    public override void End()
    {
        // Does nothing on end
    }

    protected override void ApplyEffect()
    {
        if (bleedParticles == null) {
            BleedEffect bleedEffect = (BleedEffect)Effect;
            // Create fire particles
            bleedParticles = GameObject.Instantiate(bleedEffect.bleedingParticles, health.transform).GetComponent<ParticleSystem>();
            // Set duration
            var main = bleedParticles.main;
            main.duration = Effect.Duration;
            bleedParticles.Play();
        }
    }

    protected override void onTick()
    {
        BleedEffect bleedEffect = (BleedEffect)Effect;

        if (health != null) {
            health.reduceHealth(bleedEffect.tickDamage * stacks);
            PopUpTextManager.instance.createWeakPopup(bleedEffect.tickDamage * stacks + "", bleedEffect.damageColor, health.transform.position);
        }
    }
}

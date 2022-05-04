using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpeedEffect : TimedEffect
{
    private readonly Stats stats;
    private ParticleSystem speedParticles;

    public TimedSpeedEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        stats = parent.GetComponent<Stats>();
    }

    public override void End()
    {
        SpeedEffect speedEffect = (SpeedEffect)Effect;
        if (stats != null)
        {
            stats.movespeedMultiplier -= speedEffect.percentSpeedBoost;
        }
    }

    protected override void ApplyEffect()
    {
        SpeedEffect speedEffect = (SpeedEffect)Effect;
        if (stats != null)
        {
            stats.movespeedMultiplier += speedEffect.percentSpeedBoost;

            // Add speed particles
            speedParticles = GameObject.Instantiate(speedEffect.speedParticles, stats.transform).GetComponent<ParticleSystem>();
            // Set duration
            var main = speedParticles.main;
            main.duration = Effect.Duration;
            speedParticles.Play();
        }
    }

    protected override void onTick()
    {
        // Do nothing
    }
}

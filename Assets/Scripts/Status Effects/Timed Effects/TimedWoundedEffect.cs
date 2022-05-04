using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedWoundedEffect : TimedEffect
{
    private Stats stats;
    private EffectableEntity effectableEntity;

    public TimedWoundedEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
       stats = parent.GetComponent<Stats>();
       effectableEntity = parent.GetComponent<EffectableEntity>();
    }

    protected override void ApplyEffect()
    {
        GameEvents.instance.onAddStatusEffect += increaseDuration;

        WoundedEffect woundedEffect = (WoundedEffect)Effect;
        stats.damageDealtMultiplier -= woundedEffect.percentReducedDamage;
    }

    protected override void onTick()
    {
        // Do nothing
    }

    public override void End()
    {
        GameEvents.instance.onAddStatusEffect -= increaseDuration;

        WoundedEffect woundedEffect = (WoundedEffect)Effect;
        stats.damageDealtMultiplier += woundedEffect.percentReducedDamage;
    }

    private void increaseDuration(TimedEffect timedEffect, EffectableEntity effectable) {
        if (effectableEntity == effectable && (timedEffect is TimedBleedEffect || timedEffect is TimedPoisonEffect)) {
            // Add time
            Debug.Log("Increased time!");
            WoundedEffect woundedEffect = (WoundedEffect)Effect;
            timedEffect.Effect.Duration += woundedEffect.effectTimeIncreased;
        }
    }
}

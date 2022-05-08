using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedMarkEffect : TimedEffect
{

    private Stats stats;

    public TimedMarkEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        stats = parent.GetComponent<Stats>();
    }

    public override void End()
    {
        if (stats != null) {
            var effect = (MarkEffect)Effect;
            stats.damageTakenMultiplier += effect.extraDamageTakenPercent;
        }
            
    }

    protected override void ApplyEffect()
    {
        if (stats != null) {
            var effect = (MarkEffect)Effect;
            stats.damageTakenMultiplier -= effect.extraDamageTakenPercent;
        }
    }

    protected override void onTick()
    {
        // Does nothing on tick
    }
}

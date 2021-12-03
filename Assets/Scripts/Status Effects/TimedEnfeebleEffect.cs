using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEnfeebleEffect : TimedEffect
{
    private readonly CombatStats stats;

    public TimedEnfeebleEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        stats = parent.GetComponent<CombatStats>();
    }

    public override void End()
    {
        EnfeebleEffect enfeebleEffect = (EnfeebleEffect)Effect;
        if (stats != null)
        {
            stats.damageDealtMultiplier += enfeebleEffect.damageDealtMultiplier;
        }
    }

    protected override void ApplyEffect()
    {
        EnfeebleEffect enfeebleEffect = (EnfeebleEffect)Effect;
        if(stats != null)
        {
            stats.damageDealtMultiplier -= enfeebleEffect.damageDealtMultiplier;
        }
    }

    protected override void onTick()
    {
        // Do nothing
    }
}

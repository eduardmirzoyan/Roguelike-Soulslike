using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpeedEffect : TimedEffect
{
    private readonly CombatStats stats;

    public TimedSpeedEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        stats = parent.GetComponent<CombatStats>();
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
        }
    }

    protected override void onTick()
    {
        // Do nothing
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEmpowerEffect : TimedEffect
{
    private readonly Stats stats;

    public TimedEmpowerEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        stats = parent.GetComponent<Stats>();
    }

    public override void End()
    {
        EmpowerEffect empowerEffect = (EmpowerEffect)Effect;
        if (stats != null)
        {
            stats.damageDealtMultiplier -= empowerEffect.damageDealtMultiplier;
        }
    }

    protected override void ApplyEffect()
    {
        EmpowerEffect empowerEffect = (EmpowerEffect)Effect;
        if (stats != null)
        {
            stats.damageDealtMultiplier += empowerEffect.damageDealtMultiplier;
        }
    }

    protected override void onTick()
    {
        // Do nothing
    }
}

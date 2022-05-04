using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedFortifiedEffect : TimedEffect
{
    private Stats stats;
    private int totalArmorGiven;

    public TimedFortifiedEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        stats = parent.GetComponent<Stats>();
    }

    public override void End()
    {
        if (stats != null) {
            stats.defense -= totalArmorGiven;
            totalArmorGiven = 0;
        }
    }

    protected override void ApplyEffect()
    {
        if (stats != null) {
            FortifiedEffect fortified = (FortifiedEffect)Effect;
            stats.defense += fortified.bonusArmor;
            totalArmorGiven += fortified.bonusArmor;
        }
    }

    protected override void onTick()
    {
        // Does nothing on tick
    }
}

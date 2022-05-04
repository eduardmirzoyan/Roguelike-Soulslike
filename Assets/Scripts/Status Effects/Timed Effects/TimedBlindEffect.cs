using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBlindEffect : TimedEffect
{
    private Stats stats;

    public TimedBlindEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        stats = parent.GetComponent<Stats>();
    }

    public override void End()
    {
        if (stats != null)
            stats.percentMissChance -= 1;
    }

    protected override void ApplyEffect()
    {
        if (stats != null)
            stats.percentMissChance += 1;
    }

    protected override void onTick()
    {
        // Does nothing on tick
    }

}

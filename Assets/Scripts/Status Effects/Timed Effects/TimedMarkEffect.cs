using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedMarkEffect : TimedEffect
{

    public TimedMarkEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        // Does nothing
    }

    public override void End()
    {
        // Does nothing on end
    }

    protected override void ApplyEffect()
    {
        // Does nothing on apply
    }

    protected override void onTick()
    {
        // Does nothing on tick
    }
}

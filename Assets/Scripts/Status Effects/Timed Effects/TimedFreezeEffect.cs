using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedFreezeEffect : TimedEffect
{

    public TimedFreezeEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
       
    }

    public override void End()
    {
        // Does nothing on end
    }

    protected override void ApplyEffect()
    {
        
    }

    protected override void onTick()
    {
        // Do nothing per tick
    }
}

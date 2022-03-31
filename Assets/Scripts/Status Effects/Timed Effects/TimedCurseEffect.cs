using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCurseEffect : TimedEffect
{
    private Health health;

    public TimedCurseEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        health = parent.GetComponent<Health>(); 
    }

    public override void End()
    {
        // Does nothing on end
    }

    protected override void ApplyEffect()
    {
        
        if (EffectStacks + 1 >= ((CurseEffect)Effect).triggerAmount) {
            EffectStacks = 0;
            // Instantly set health to 0
            if (health != null) {
                health.reduceHealth(health.getMaxHP());
            }
        }           
    }

    protected override void onTick()
    {
        // Do nothing on tick
    }
}

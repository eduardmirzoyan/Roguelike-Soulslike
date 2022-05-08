using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedFreezeEffect : TimedEffect
{
    private Displacable displacable;

    public TimedFreezeEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        displacable = parent.GetComponent<Displacable>();
    }

    public override void End()
    {
        // Does nothing on end
    }

    protected override void ApplyEffect()
    {
        var freezeEffect = (FreezeEffect)Effect;
        // Check if trigger amount is reached
        if (stacks + 1 >= freezeEffect.triggerAmount) {
            // If so, then stun!
            if (displacable != null) {
                // Stun!
                displacable.triggerStun(freezeEffect.freezeDuration);

                // Set duration to 0
                IsFinished = true;
            }
        }   
    }

    protected override void onTick()
    {
        // Do nothing per tick
    }
}

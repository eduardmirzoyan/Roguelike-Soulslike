using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedStunEffect : TimedEffect
{
    private Displacable displacable;

    public TimedStunEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        displacable = parent.GetComponent<Displacable>();
    }

    protected override void ApplyEffect()
    {
        // Spawn stunned icon
        GameManager.instance.stunAnimation(displacable.transform, Effect.Duration);

        if(displacable != null)
        {
            displacable.triggerStun(Effect.Duration);
        }
        else
        {
            Duration = 0;
        }
    }

    protected override void onTick()
    {
        // Nothing per tick
    }

    public override void End()
    {
        // Do nothing
    }
}

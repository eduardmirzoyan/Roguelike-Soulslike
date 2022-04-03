using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedStunEffect : TimedEffect
{
    [SerializeField] private Displacable displacable;

    public TimedStunEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
       displacable = parent.GetComponent<Displacable>();
    }

    protected override void ApplyEffect()
    {
        if (displacable != null) {
            Debug.Log(Effect.Duration);
            displacable.triggerStun(Effect.Duration);
        }
    }

    protected override void onTick()
    {
        // Nothing per tick
        if (displacable != null) { 
            if (displacable.getStunDuration() <= 0) {
                IsFinished = true;
            }
        }
    }

    public override void End()
    {
        // Do nothing
    }
}

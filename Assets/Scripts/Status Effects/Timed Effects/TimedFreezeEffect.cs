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
        if(displacable != null)
        {
            Debug.Log("Frozen");
            displacable.triggerStun(2f);
        }
    }

    protected override void onTick()
    {
        // Do nothing per tick
    }
}

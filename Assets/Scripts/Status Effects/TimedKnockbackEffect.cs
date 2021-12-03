using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedKnockbackEffect : TimedEffect
{
    private Displacable displacable;
    private Vector3 origin;
    private float pushForce;

    public TimedKnockbackEffect(BaseEffect effect, GameObject parent, Vector3 origin, float pushForce) : base(effect, parent)
    {
        this.pushForce = pushForce;
        this.origin = origin;
        displacable = parent.GetComponent<Displacable>();
    }

    protected override void ApplyEffect()
    {
        pushForce *= (EffectStacks + 1);

        if (displacable != null)
        {
            //displacable.triggerKnockbackKK(pushForce, origin);
        }
        else
        {
            Duration = 0;
        }
    }

    protected override void onTick()
    {
        // Keep reseting duration until the entity is no longer displacing
        /*Duration = 1f;
        if (displacable.knockbackSpeed == 0)
            Duration = 0;*/
    }

    public override void End()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedKnockbackEffect : TimedEffect
{
    private Displacable displacable;
    private float pushForce;
    private Vector3 origin;

    public TimedKnockbackEffect(BaseEffect effect, GameObject parent, Vector3 origin, float pushForce) : base(effect, parent)
    {
        this.pushForce = pushForce;
        this.origin = origin;
        displacable = parent.GetComponent<Displacable>();
    }

    protected override void ApplyEffect()
    {
        if (displacable != null) {
            displacable.triggerKnockback(pushForce, ((KnockbackEffect)Effect).Duration, origin);
        }
    }

    protected override void onTick()
    {
        // Nothing
    }

    public override void End()
    {
        // Nothing
    }
}

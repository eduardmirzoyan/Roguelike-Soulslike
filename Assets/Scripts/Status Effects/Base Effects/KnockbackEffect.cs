using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class KnockbackEffect : BaseEffect
{
    public Vector3 origin;
    public float pushForce;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedKnockbackEffect(this, parent, origin, pushForce);
    }

    public TimedEffect InitializeEffect(GameObject parent, Vector3 origin, float pushForce)
    {
        return new TimedKnockbackEffect(this, parent, origin, pushForce);
    }
}

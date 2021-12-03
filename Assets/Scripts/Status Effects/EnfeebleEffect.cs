using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnfeebleEffect : BaseEffect
{
    public float damageDealtMultiplier;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedEnfeebleEffect(this, parent);
    }
}

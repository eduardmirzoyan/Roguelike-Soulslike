using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StunEffect : BaseEffect
{
    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedStunEffect(this, parent);
    }
}

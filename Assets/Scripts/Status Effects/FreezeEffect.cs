using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FreezeEffect : BaseEffect
{
    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedFreezeEffect(this, parent);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CurseEffect : BaseEffect
{
    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedCurseEffect(this, parent);
    }
}

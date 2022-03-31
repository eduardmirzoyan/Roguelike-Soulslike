using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Freeze")]
public class FreezeEffect : BaseEffect
{
    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedFreezeEffect(this, parent);
    }
}

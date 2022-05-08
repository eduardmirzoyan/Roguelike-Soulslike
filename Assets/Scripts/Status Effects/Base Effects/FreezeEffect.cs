using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Freeze")]
public class FreezeEffect : BaseEffect
{
    public int triggerAmount = 3;
    public float freezeDuration = 1f;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedFreezeEffect(this, parent);
    }
}

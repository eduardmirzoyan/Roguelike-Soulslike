using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Wounded")]
public class WoundedEffect : BaseEffect
{
    public float percentReducedDamage = 0.15f;
    public float effectTimeIncreased = 1f;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedWoundedEffect(this, parent);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BurnEffect : BaseEffect
{
    public int tickDamage;
    public Color damageColor;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedBurnEffect(this, parent);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BleedEffect : BaseEffect
{
    public int tickDamage;
    public Color damageColor;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedBleedEffect(this, parent);
    }
}
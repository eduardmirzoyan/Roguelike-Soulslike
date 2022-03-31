using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Poison")]
public class PoisonEffect : BaseEffect
{
    public int tickDamage;
    public Color damageColor;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedPoisonEffect(this, parent);
    }
}

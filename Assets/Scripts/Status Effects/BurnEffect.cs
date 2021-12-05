using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Burn")]
public class BurnEffect : BaseEffect
{
    public int tickDamage;
    public Color damageColor;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedBurnEffect(this, parent);
    }
}

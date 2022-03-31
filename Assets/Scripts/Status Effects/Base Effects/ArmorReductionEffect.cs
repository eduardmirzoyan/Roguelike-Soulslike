using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Armor Reduction")]
public class ArmorReductionEffect : BaseEffect
{
    public float percentArmorReduction;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedArmorReductionEffect(this, parent);
    }
}

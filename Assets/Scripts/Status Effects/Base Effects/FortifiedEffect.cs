using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Fortified")]
public class FortifiedEffect : BaseEffect
{
    public int bonusArmor = 5;
    
    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedFortifiedEffect(this, parent);
    }
}

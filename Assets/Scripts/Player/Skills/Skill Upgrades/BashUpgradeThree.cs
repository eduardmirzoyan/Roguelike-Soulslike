using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BashUpgradeThree : Upgrade
{
    // Enable projectile reflection for the shield bash
    public override void upgradeDuringActive(GameObject parent, Ability ability)
    {
        var bashShield = parent.GetComponentInChildren<BashingShield>();
        if(bashShield != null)
            bashShield.enabledProjectileReflection = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BashUpgradeOne : Upgrade
{
    // Change the damage of the shield from a knockback to a stun
    public override void upgradeBeforeChargeUp(GameObject parent, Ability ability)
    {
        // Clears all effects
        ((ShieldBashAbility)ability).damage.effects.Clear();
        var stun = CreateInstance<StunEffect>();

        // Adds a stun
        stun.Duration = 1f;
        ((ShieldBashAbility)ability).damage.effects.Add(stun);
    }
}

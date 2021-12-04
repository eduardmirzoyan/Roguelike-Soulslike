using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Upgrades slash to cause bleeding
[CreateAssetMenu]
public class SlashUpgradeTwo : Upgrade
{
    [SerializeField] private BaseEffect bleedEffect;

    // Give the sword a bleed effect
    public override void upgradeAfterChargeUp(GameObject parent, Ability ability)
    {
        ((SlashAbility)ability).damage.effects.Add(bleedEffect);
    }
}

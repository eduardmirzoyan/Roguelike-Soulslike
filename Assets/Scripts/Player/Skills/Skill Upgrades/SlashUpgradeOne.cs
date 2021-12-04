using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SlashUpgradeOne : Upgrade
{
    [SerializeField] private int numberOfPeircesToAdd;
    // Use a light attack to send the slash wave, but still need to change the timer!!!
    public override void upgradeBeforeChargeUp(GameObject parent, Ability ability)
    {
        ((SlashAbility)ability).increaseNumberPerices();
    }
}

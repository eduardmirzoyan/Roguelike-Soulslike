using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIX THIS!
[CreateAssetMenu]
public class SlashUpgradeOne : Upgrade
{
    // Use a light attack to send the slash wave, but still need to change the timer!!!
    public override void upgradeBeforeChargeUp(GameObject parent, Ability ability)
    {
        //parent.GetComponent<Animator>().SetTrigger("light attack");
        //parent.GetComponentInChildren<Weapon>().GetComponent<Animator>().lightAttackAnimation();
        ability.chargeUpTime = parent.GetComponentInChildren<Weapon>().lightActiveTime + parent.GetComponentInChildren<Weapon>().lightWindupTime;
    }
}

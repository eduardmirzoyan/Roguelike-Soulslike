using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockUpgradeTwo : Upgrade
{
    private float orginalCooldown;
    [SerializeField] private bool hasPerfectBlocked;

    public override void upgradeInstaniation(GameObject parent, Ability ability)
    {
        //GameEvents.current.onPerfectBlock += resetCooldownOnPerfectBlock;
        orginalCooldown = ability.cooldownTime;
    }

    public override void upgradeAfterChargeUp(GameObject parent, Ability ability)
    {
        hasPerfectBlocked = false;
        ability.cooldownTime = orginalCooldown;
    }

    private void resetCooldownOnPerfectBlock()
    {
        hasPerfectBlocked = true;
    }

    public override void upgradeAfterActive(GameObject parent, Ability ability)
    {
        if (hasPerfectBlocked)
        {
            GameManager.instance.CreatePopup("COOLDOWN RESET", parent.transform.position);
            ability.cooldownTime = 0;
        }
    }

    public override void upgradeUninstaniate(GameObject parent, Ability ability)
    {
        //GameEvents.current.onPerfectBlock -= resetCooldownOnPerfectBlock;
    }
}

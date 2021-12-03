using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Upgrade reduces the cooldown of slash ability on hit
[CreateAssetMenu]
public class SlashUpgradeThree : Upgrade
{
    private GameObject parent;

    // Cause ability cooldown to decrease if you hit an enemy
    public override void upgradeInstaniation(GameObject parent, Ability ability)
    {
        this.parent = parent;
        GameEvents.current.onHit += reduceCooldownOnHit;
    }

    private void reduceCooldownOnHit()
    {
        parent.GetComponent<CombatHandler>().getSignatureAbilityHolder().reduceAbilityCooldown(1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Give yourself a damage reduction buff when using attack, CHANGE THIS TO ONHIT ONYL AT SOME POINT IN TIME
[CreateAssetMenu]
public class BashUpgradeTwo : Upgrade
{
    [SerializeField] private BaseEffect selfDefenseBuff;

    public override void upgradeBeforeChargeUp(GameObject parent, Ability ability)
    {
        parent.GetComponent<EffectableEntity>().addEffect(selfDefenseBuff.InitializeEffect(parent));
    }

}

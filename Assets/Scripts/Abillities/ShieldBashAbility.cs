using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShieldBashAbility : Ability
{
    [SerializeField] private GameObject shieldBashPrefab;

    [SerializeField] public Damage damage;

    public override void perfromBeforeChargeUp(GameObject parent)
    {
        base.perfromBeforeChargeUp(parent);

        var shield = Instantiate(shieldBashPrefab, parent.transform.position, Quaternion.identity, parent.transform);
        //shield.GetComponent<DamageDealer>().setDamage(damage);
    }

    // Move forward during shield bash
    public override void performDuringActive(GameObject parent)
    {
        var mover = parent.GetComponent<Movement>();
        if(mover != null)
        {
            mover.dash(7, mover.getFacingDirection());
        }

        base.performDuringActive(parent);
    }
}

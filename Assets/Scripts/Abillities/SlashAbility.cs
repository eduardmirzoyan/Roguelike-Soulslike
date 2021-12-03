using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlashAbility : Ability
{
    [SerializeField] public Damage damage;
    [SerializeField] private GameObject slashProjectilePrefab;

    public override void perfromBeforeChargeUp(GameObject parent)
    {
        //parent.GetComponent<Animator>().SetTrigger("heavy attack");
        //parent.GetComponentInChildren<Weapon>().heavyAttackAnimation();
        chargeUpTime = parent.GetComponentInChildren<Weapon>().heavyActiveTime + parent.GetComponentInChildren<Weapon>().heavyWindupTime;

        base.perfromBeforeChargeUp(parent);
    }

    public override void performAfterChargeUp(GameObject parent)
    {
        var slash = Instantiate(slashProjectilePrefab, parent.transform.position, parent.transform.rotation);
        //slash.GetComponent<DamageDealer>().setDamage(damage);

        base.performAfterChargeUp(parent);
    }
}

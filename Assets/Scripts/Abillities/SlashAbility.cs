using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlashAbility : Ability
{
    [SerializeField] public Damage damage;
    [SerializeField] private GameObject slashProjectilePrefab;
    private int numberOfPeirces = 0;
    private Weapon weapon;

    public override void perfromBeforeChargeUp(GameObject parent)
    {
        weapon = parent.GetComponentInChildren<Weapon>();
        weapon.GetComponent<Animator>().Play(weapon.weaponSpecialAttackAnimation);
        parent.GetComponent<AnimationHandler>().changeAnimationState(weapon.weaponSpecialAttackAnimation);
        chargeUpTime = weapon.heavyActiveTime + weapon.heavyWindupTime;

        base.perfromBeforeChargeUp(parent);
    }

    public override void performAfterChargeUp(GameObject parent)
    {
        var slash = Instantiate(slashProjectilePrefab, parent.transform.position, parent.transform.rotation);
        slash.GetComponent<Slash>().numberOfPeirces = numberOfPeirces;
        base.performAfterChargeUp(parent);
    }

    public override void performAfterActive(GameObject parent)
    {
        weapon.GetComponent<Animator>().Play(weapon.weaponIdleAnimation);
        base.performAfterActive(parent);
    }

    public void increaseNumberPerices() => numberOfPeirces = 1;
}

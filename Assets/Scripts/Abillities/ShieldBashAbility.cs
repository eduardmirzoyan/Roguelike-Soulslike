using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShieldBashAbility : Ability
{
    [SerializeField] private GameObject shieldBashPrefab;
    [SerializeField] public BashingShield bashingShield;

    [SerializeField] private string shieldBashAnimation;
    [SerializeField] public Damage shieldBashDamage;

    public override void perfromBeforeChargeUp(GameObject parent)
    {
        var shield = Instantiate(shieldBashPrefab, parent.transform.position, Quaternion.identity, parent.transform);
        bashingShield = shield.GetComponent<BashingShield>();
        bashingShield.setDamage(shieldBashDamage);

        parent.GetComponent<AnimationHandler>().changeAnimationState(shieldBashAnimation); // Play animation

        base.perfromBeforeChargeUp(parent);
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

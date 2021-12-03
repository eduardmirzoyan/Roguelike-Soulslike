using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockUpgradeOne : Upgrade
{
    [SerializeField] private int blockingMoveSpeed;
    public override void upgradeDuringActive(GameObject parent, Ability ability)
    {
        var mover = parent.GetComponent<Movement>();
        var movedirction = parent.GetComponent<InputBuffer>().moveDirection;
        mover.WalkAtSpeed(movedirction, blockingMoveSpeed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Curse")]
public class CurseEffect : BaseEffect
{
    public int triggerAmount = 5;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedCurseEffect(this, parent);
    }
}

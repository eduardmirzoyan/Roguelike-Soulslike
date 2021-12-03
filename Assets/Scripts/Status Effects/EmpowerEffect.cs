using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EmpowerEffect : BaseEffect
{
    public float damageDealtMultiplier;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedEmpowerEffect(this, parent);
    }
}

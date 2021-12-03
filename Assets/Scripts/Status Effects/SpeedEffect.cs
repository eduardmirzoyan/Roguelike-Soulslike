using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpeedEffect : BaseEffect
{
    public float percentSpeedBoost;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedSpeedEffect(this, parent);
    }
}

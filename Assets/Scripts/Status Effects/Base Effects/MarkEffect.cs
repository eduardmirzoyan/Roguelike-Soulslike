using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Mark")]
public class MarkEffect : BaseEffect
{
    public float extraDamageTakenPercent = 0.15f;
    
    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedMarkEffect(this, parent);
    }
}

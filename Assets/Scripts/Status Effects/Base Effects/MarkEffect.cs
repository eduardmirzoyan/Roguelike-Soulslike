using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Mark")]
public class MarkEffect : BaseEffect
{
    // Fields?
    
    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedMarkEffect(this, parent);
    }
}

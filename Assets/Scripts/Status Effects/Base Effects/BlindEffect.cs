using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Blind")]
public class BlindEffect : BaseEffect
{
    [SerializeField] private float blindRadius;
    
    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedBlindEffect(this, parent);
    }
}

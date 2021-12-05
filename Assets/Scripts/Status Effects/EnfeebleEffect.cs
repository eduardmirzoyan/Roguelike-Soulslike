using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Enfeeble")]
public class EnfeebleEffect : BaseEffect
{
    public float damageDealtMultiplier;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedEnfeebleEffect(this, parent);
    }
}

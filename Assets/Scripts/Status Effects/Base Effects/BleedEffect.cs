using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Bleed")]
public class BleedEffect : BaseEffect
{
    public int tickDamage;
    public Color damageColor;
    public GameObject bleedingParticles;

    public override TimedEffect InitializeEffect(GameObject parent)
    {
        return new TimedBleedEffect(this, parent);
    }
}

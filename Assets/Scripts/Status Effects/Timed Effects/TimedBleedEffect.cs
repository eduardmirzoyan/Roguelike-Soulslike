using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBleedEffect : TimedEffect
{
    private Health health;

    public TimedBleedEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        health = parent.GetComponent<Health>();
    }

    public override void End()
    {
        // Does nothing on end
    }

    protected override void ApplyEffect()
    {
        // Does nothing on apply
    }

    protected override void onTick()
    {
        BleedEffect bleedEffect = (BleedEffect)Effect;

        if (health != null) {
            health.reduceHealth(bleedEffect.tickDamage * EffectStacks);
            GameManager.instance.CreatePopup(bleedEffect.tickDamage * EffectStacks + "", health.transform.position, bleedEffect.damageColor);
        }
    }
}

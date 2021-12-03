using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBurnEffect : TimedEffect
{
    private readonly Damageable entity;

    public TimedBurnEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        entity = parent.GetComponent<Damageable>();
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
        BurnEffect bleedEffect = (BurnEffect)Effect;

        if (entity != null)
        {
            Damage dmg = new Damage
            {
                damageAmount = bleedEffect.tickDamage * EffectStacks, // Deal damage equal to stacks
                source = DamageSource.fromSelf,
                isTrue = true,
                isAvoidable = false,
                triggersIFrames = false,
                color = bleedEffect.damageColor
            };
            entity.takeDamage(dmg);
        }
    }
}

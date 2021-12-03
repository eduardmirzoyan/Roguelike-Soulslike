using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBleedEffect : TimedEffect
{
    private readonly Damageable entity;

    public TimedBleedEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
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
        BleedEffect bleedEffect = (BleedEffect)Effect;

        if(entity != null)
        {
            Damage dmg = new Damage
            {
                damageAmount = bleedEffect.tickDamage * EffectStacks, // Deal damage equal to stacks
                isTrue = true,
                isAvoidable = false,
                triggersIFrames = false,
                source = DamageSource.fromSelf,
                color = bleedEffect.damageColor
            };
            entity.takeDamage(dmg);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPoisonEffect : TimedEffect
{
    private readonly Damageable entity;

    public TimedPoisonEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
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
        PoisonEffect poisonEffect = (PoisonEffect)Effect;

        if (entity != null)
        {
            Damage dmg = new Damage
            {
                damageAmount = poisonEffect.tickDamage * EffectStacks, // Deal damage equal to stacks
                source = DamageSource.fromSelf,
                isTrue = true,
                isAvoidable = false,
                triggersIFrames = false,
                color = poisonEffect.damageColor
            };
            entity.takeDamage(dmg);
        }
    }
}

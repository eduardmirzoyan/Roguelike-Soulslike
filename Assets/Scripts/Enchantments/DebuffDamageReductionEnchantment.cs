using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/DamageReductionDebuff")]
public class DebuffDamageReductionEnchantment : Enchantment
{
    private EffectableEntity effectableEntity;
    private CombatStats stats;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        stats = entity.GetComponent<CombatStats>();
        effectableEntity = entity.GetComponent<EffectableEntity>();
    }

    public override void onTick()
    {
        if (effectableEntity != null && stats != null)
        {
            stats.damageTakenMultiplier = effectableEntity.getNumberOfEffects() * 0.1f;
        }
    }

    public override void unintialize()
    {
        stats.damageTakenMultiplier = 0;
        stats = null;
        effectableEntity = null;
        base.unintialize();
    }
}

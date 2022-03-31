using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedArmorReductionEffect : TimedEffect
{
    private CombatStats stats;
    private int stolenArmorAmount;

    public TimedArmorReductionEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        stats = parent.GetComponent<CombatStats>();
        stolenArmorAmount = 0;
    }

    public override void End()
    {
        // Give back all the taken armor
        if (stats != null) {
            stats.defense += stolenArmorAmount;
        }
    }

    protected override void ApplyEffect()
    {
        ArmorReductionEffect armorReductionEffect = (ArmorReductionEffect)Effect;
        // Take 10% of remaining armor per stack
        if (stats != null) {
            // First give back all stolen armor too see the true amount
            stats.defense += stolenArmorAmount;

            // Then calculate the new percentage to take
            int amountToTake = (int)(stats.defense * armorReductionEffect.percentArmorReduction * (EffectStacks + 1));
            
            // Cache that amount
            stolenArmorAmount = amountToTake;

            // Then reduce the defence
            stats.defense -= amountToTake;
        }
    }

    protected override void onTick()
    {
        // Do nothing
    }
}

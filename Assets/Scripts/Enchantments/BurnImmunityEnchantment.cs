using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/BurnImmunity")]
public class BurnImmunityEnchantment : Enchantment
{
    private CombatStats stats;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        stats = entity.GetComponent<CombatStats>();
        if (stats != null)
            stats.percentFireResistance = 100;
    }

    public override void unintialize()
    {
        if (stats != null)
            stats.percentFireResistance = 0;
        stats = null;
        base.unintialize();
    }
}

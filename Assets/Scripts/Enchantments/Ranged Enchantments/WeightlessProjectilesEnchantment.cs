using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Ranged Enchantment/Weightless Projectiles")]
public class WeightlessProjectilesEnchantment : RangedEnchantment
{
    private RangedWeapon rangedWeapon;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        rangedWeapon = entity.GetComponent<RangedWeapon>();
        if (rangedWeapon != null) {
            Debug.Log("Does nothing so far.");
        }
    }

    public override void unintialize()
    {
        if (rangedWeapon != null) {
            Debug.Log("Still does nothing so far.");
        }
        rangedWeapon = null;
        base.unintialize();
    }
}

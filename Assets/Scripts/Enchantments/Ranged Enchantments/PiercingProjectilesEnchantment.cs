using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Ranged Enchantment/Piercing Projectiles")]
public class PiercingProjectilesEnchantment : RangedEnchantment
{
    [SerializeField] private int numberOfPierces;
    private RangedWeapon rangedWeapon;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        rangedWeapon = entity.GetComponent<RangedWeapon>();
        if (rangedWeapon != null) {
            rangedWeapon.addPierces(numberOfPierces);
        }
    }

    public override void unintialize()
    {
        if (rangedWeapon != null) {
            rangedWeapon.addPierces(-numberOfPierces);
        }
        rangedWeapon = null;
        base.unintialize();
    }
}

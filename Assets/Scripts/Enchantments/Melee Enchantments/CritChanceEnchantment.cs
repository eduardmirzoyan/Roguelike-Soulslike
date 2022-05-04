using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Increased Crit Chance")]
public class CritChanceEnchantment : MeleeEchantment
{
    [SerializeField] private float bonusAmount = 0.15f;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        // Get the weapon
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.getOwner().critChance += bonusAmount;
    }

    public override void unintialize()
    {
        meleeWeapon.getOwner().critChance -= bonusAmount;
        meleeWeapon = null;
        base.unintialize();
    }
}

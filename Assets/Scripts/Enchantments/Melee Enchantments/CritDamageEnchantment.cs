using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Increased Crit Damage")]
public class CritDamageEnchantment : MeleeEchantment
{
    [SerializeField] private float bonusAmount = 0.5f;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        // Get the weapon
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.getOwner().critDamage += bonusAmount;
    }

    public override void unintialize()
    {
        meleeWeapon.getOwner().critDamage -= bonusAmount;
        meleeWeapon = null;
        base.unintialize();
    }
}

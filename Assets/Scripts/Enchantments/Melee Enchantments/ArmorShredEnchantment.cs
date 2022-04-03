using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Reduce Armor Melee")]
public class ArmorShredEnchantment : MeleeEchantment
{
    [SerializeField] private ArmorReductionEffect armorReductionEffect;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.addEffect(armorReductionEffect);
    }

    public override void unintialize()
    {
        meleeWeapon.addEffect(armorReductionEffect);
        meleeWeapon = null;
        base.unintialize();
    }
}

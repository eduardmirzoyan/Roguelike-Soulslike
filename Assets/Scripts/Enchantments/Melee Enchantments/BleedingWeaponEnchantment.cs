using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Bleeding Melee")]
public class BleedingWeaponEnchantment : MeleeEchantment
{
    [SerializeField] private BleedEffect bleedEffect;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.addEffect(bleedEffect);
    }

    public override void unintialize()
    {
        meleeWeapon.addEffect(bleedEffect);
        meleeWeapon = null;
        base.unintialize();
    }
}

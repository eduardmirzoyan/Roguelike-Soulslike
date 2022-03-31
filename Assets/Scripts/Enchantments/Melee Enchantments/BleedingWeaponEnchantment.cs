using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Bleeding Melee")]
public class BleedingWeaponEnchantment : MeleeEchantment
{
    [SerializeField] private BleedEffect bleedEffect;
    private Weapon weapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        weapon = weaponGameObject.GetComponentInChildren<Weapon>();
        weapon.addEffect(bleedEffect);
    }

    public override void unintialize()
    {
        weapon.addEffect(bleedEffect);
        weapon = null;
        base.unintialize();
    }
}

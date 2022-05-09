using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Reduced Stamina Cost")]
public class ReducedStaminaCostEnchantment : MeleeEchantment
{
    [SerializeField] private float reducedPercent = 0.5f;
    private MeleeWeapon meleeWeapon;
    private WeaponItem weaponItem;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        weaponItem = meleeWeapon.getOwner();
        weaponItem.staminaCostMultiplier -= reducedPercent;
    }

    public override void unintialize()
    {
        weaponItem.staminaCostMultiplier += reducedPercent;
        weaponItem = null;
        meleeWeapon = null;
        base.unintialize();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Posioned Melee")]
public class PoisonedWeaponEnchantment : MeleeEchantment
{
    [SerializeField] private PoisonEffect poisonEffect;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.addEffect(poisonEffect);
    }

    public override void unintialize()
    {
        meleeWeapon.addEffect(poisonEffect);
        meleeWeapon = null;
        base.unintialize();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Burning Melee")]
public class BurningMeleeEnchantment : MeleeEchantment
{
    [SerializeField] private BurnEffect burnEffect;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.addEffect(burnEffect);
    }

    public override void unintialize()
    {
        meleeWeapon.addEffect(burnEffect);
        meleeWeapon = null;
        base.unintialize();
    }
}

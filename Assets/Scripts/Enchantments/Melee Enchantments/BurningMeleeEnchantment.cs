using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Burning Melee")]
public class BurningMeleeEnchantment : MeleeEchantment
{
    [SerializeField] private BurnEffect burnEffect;
    private Weapon weapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        weapon = weaponGameObject.GetComponentInChildren<Weapon>();
        weapon.addEffect(burnEffect);
    }

    public override void unintialize()
    {
        weapon.addEffect(burnEffect);
        weapon = null;
        base.unintialize();
    }
}

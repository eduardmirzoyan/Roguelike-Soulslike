using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Posioned Melee")]
public class PoisonedWeaponEnchantment : MeleeEchantment
{
    [SerializeField] private PoisonEffect poisonEffect;
    private Weapon weapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        weapon = weaponGameObject.GetComponentInChildren<Weapon>();
        weapon.addEffect(poisonEffect);
    }

    public override void unintialize()
    {
        weapon.addEffect(poisonEffect);
        weapon = null;
        base.unintialize();
    }
}

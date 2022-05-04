using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Wound on Healthy Hit")]
public class WoundOnHealthyEnchantment : MeleeEchantment
{
    [SerializeField] private float damageBonus;
    [SerializeField] private float minPercentRatio;
    [SerializeField] private WoundedEffect woundedEffect;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        // TODO: THIS
    }

    public override void unintialize()
    {

        // TODO: THIS

        base.unintialize();
    }

    public void applyWound(Weapon weapon, GameObject hitEntity) {
        if (weapon == meleeWeapon && hitEntity.TryGetComponent(out Health health)) {
            // If HP is above min ratio, then apply bonus damage and wound
            if (health.getMaxHP() * minPercentRatio >= health.getHP()) {
                // TODO: THIS
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Ranged Enchantment/Large Projectiles")]
public class LargerProjectileEnchantment : RangedEnchantment
{
    [SerializeField] private float sizeMultiplier;
    private RangedWeapon rangedWeapon;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        rangedWeapon = entity.GetComponent<RangedWeapon>();
        if (rangedWeapon != null) {
            rangedWeapon.increaseProjectileSize(sizeMultiplier);
        }
    }

    public override void unintialize()
    {
        if (rangedWeapon != null) {
            rangedWeapon.increaseProjectileSize(-sizeMultiplier);
        }
        rangedWeapon = null;
        base.unintialize();
    }
}

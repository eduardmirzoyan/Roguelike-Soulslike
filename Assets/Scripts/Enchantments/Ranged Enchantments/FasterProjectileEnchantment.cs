using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Ranged Enchantment/Faster Projectiles")]
public class FasterProjectileEnchantment : RangedEnchantment
{
    [SerializeField] private float speedMultiplier;
    private RangedWeapon rangedWeapon;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        rangedWeapon = entity.GetComponent<RangedWeapon>();
        if (rangedWeapon != null) {
            rangedWeapon.increaseProjectileSpeed(speedMultiplier);
        }
    }

    public override void unintialize()
    {
        if (rangedWeapon != null) {
            rangedWeapon.increaseProjectileSpeed(-speedMultiplier);
        }
        rangedWeapon = null;
        base.unintialize();
    }
}

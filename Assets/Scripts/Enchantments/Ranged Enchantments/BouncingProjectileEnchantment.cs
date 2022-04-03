using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Ranged Enchantment/Bouncy Projectiles")]
public class BouncingProjectileEnchantment : RangedEnchantment
{
    [SerializeField] private int numberOfBounces;
    private RangedWeapon rangedWeapon;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        rangedWeapon = entity.GetComponent<RangedWeapon>();
        if (rangedWeapon != null) {
            rangedWeapon.addBounces(numberOfBounces);
        }
    }

    public override void unintialize()
    {
        if (rangedWeapon != null) {
            rangedWeapon.addBounces(-numberOfBounces);
        }
        rangedWeapon = null;
        base.unintialize();
    }
}

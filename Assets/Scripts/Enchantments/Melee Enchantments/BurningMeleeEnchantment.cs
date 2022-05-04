using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Burning Melee")]
public class BurningMeleeEnchantment : MeleeEchantment
{
    [SerializeField] private BurnEffect burnEffect;
    [SerializeField] private float weaponDamageRatio = 0.2f;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();

        // Calculate tickdamage based on weapon damage
        int tickDamage = (int) (meleeWeapon.getOwner().damage * weaponDamageRatio);
        // Min tick damage is 1
        tickDamage = Mathf.Max(tickDamage, 1);

        burnEffect.tickDamage = tickDamage;
        meleeWeapon.addEffect(burnEffect);
    }

    public override void unintialize()
    {
        meleeWeapon.addEffect(burnEffect);
        meleeWeapon = null;
        base.unintialize();
    }
}

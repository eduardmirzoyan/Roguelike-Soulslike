using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Posioned Melee")]
public class PoisonedWeaponEnchantment : MeleeEchantment
{
    [SerializeField] private PoisonEffect poisonEffect;
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

        poisonEffect.tickDamage = tickDamage;
        meleeWeapon.addEffect(poisonEffect);
    }

    public override void unintialize()
    {
        meleeWeapon.addEffect(poisonEffect);
        meleeWeapon = null;
        base.unintialize();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Consume Bleed Melee")]
public class ConsumeBleedEnchantment : MeleeEchantment
{
    [SerializeField] private float damagePercent;
    [SerializeField] private BleedEffect timedBleedEffect;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        GameEvents.instance.onWeaponHit += attemptToConsumeBleed;
    }

    public override void unintialize()
    {
        GameEvents.instance.onWeaponHit -= attemptToConsumeBleed;
        meleeWeapon = null;
        base.unintialize();
    }

    private void attemptToConsumeBleed(Weapon weapon, GameObject hitEntity) {
        if (weapon == meleeWeapon && hitEntity.TryGetComponent(out EffectableEntity effectableEntity) && hitEntity.TryGetComponent(out Health health)) {
            // Try to remove the bleed effect
            var tBleedEffect = effectableEntity.removeEffect(timedBleedEffect);
            if (tBleedEffect != null) {
                // Calculate damage
                int damage = (int) (meleeWeapon.getOwner().damage * damagePercent * tBleedEffect.getStacks());
                health.reduceHealth(damage);
                PopUpTextManager.instance.createPopup(damage + "", timedBleedEffect.damageColor, hitEntity.transform.position, 1.5f);
            }
        }
    }
}

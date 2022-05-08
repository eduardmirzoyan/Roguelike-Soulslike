using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Wound on Crit")]
public class WoundOnCritEnchantment : MeleeEchantment
{
    [SerializeField] private WoundedEffect woundedEffect;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        GameEvents.instance.onCrit += attemptToWound;
    }

    public override void unintialize()
    {
        GameEvents.instance.onCrit -= attemptToWound;
        meleeWeapon = null;
        base.unintialize();
    }

    public void attemptToWound(Weapon weapon, Transform hitEntity) {
        if (weapon == meleeWeapon && hitEntity.TryGetComponent(out EffectableEntity effectableEntity)) {
            effectableEntity.addEffect(woundedEffect.InitializeEffect(hitEntity.gameObject));
        }
    }
}

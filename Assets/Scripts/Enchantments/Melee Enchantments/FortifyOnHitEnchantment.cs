using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Fortifying Melee")]
public class FortifyOnHitEnchantment : MeleeEchantment
{
    [SerializeField] private FortifiedEffect fortifyEffect;
    private EffectableEntity effectableEntity;
    private MeleeWeapon meleeWeapon;

    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        effectableEntity = weaponGameObject.GetComponentInParent<EffectableEntity>();
        GameEvents.instance.onWeaponHit += attemptToGiveFortified;
    }

    public override void unintialize()
    {
        GameEvents.instance.onWeaponHit -= attemptToGiveFortified;
        effectableEntity = null;
        meleeWeapon = null;
        base.unintialize();
    }

    private void attemptToGiveFortified(Weapon weapon, GameObject hitEntity) {
        if (weapon == meleeWeapon && effectableEntity != null) {
            effectableEntity.addEffect(fortifyEffect.InitializeEffect(effectableEntity.gameObject));
        }
    }
}

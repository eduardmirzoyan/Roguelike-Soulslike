using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Speed On Hit Melee")]
public class SpeedOnHitEnchantment : MeleeEchantment
{
    [SerializeField] private SpeedEffect speedEffect;
    private EffectableEntity effectableEntity;
    private MeleeWeapon meleeWeapon;

    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        effectableEntity = weaponGameObject.GetComponentInParent<EffectableEntity>();
        GameEvents.instance.onWeaponHit += attemptToGiveMovespeed;
    }

    public override void unintialize()
    {
        GameEvents.instance.onWeaponHit -= attemptToGiveMovespeed;
        effectableEntity = null;
        meleeWeapon = null;
        base.unintialize();
    }

    private void attemptToGiveMovespeed(Weapon weapon, GameObject hitEntity) {
        if (weapon == meleeWeapon && effectableEntity != null) {
            effectableEntity.addEffect(speedEffect.InitializeEffect(effectableEntity.gameObject));
        }
    }
}

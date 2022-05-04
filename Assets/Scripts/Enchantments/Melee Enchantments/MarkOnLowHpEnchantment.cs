using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Mark on Low Hp Melee")]
public class MarkOnLowHpEnchantment : MeleeEchantment
{
    [SerializeField] private MarkEffect markEffect;
    [SerializeField] private float healthThresholdPercent = 0.33f;
    private MeleeWeapon meleeWeapon;

    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        GameEvents.instance.onWeaponHit += attemptToMark;
    }

    public override void unintialize()
    {
        GameEvents.instance.onWeaponHit -= attemptToMark;
        meleeWeapon = null;
        base.unintialize();
    }

    private void attemptToMark(Weapon weapon, GameObject hitEntity) {
        if (weapon == meleeWeapon && hitEntity.TryGetComponent(out Health health) && hitEntity.TryGetComponent(out EffectableEntity effectableEntity)) {
            // If entity health is under ratio, then you can mark
            if (health.getHP() <= health.getMaxHP() * healthThresholdPercent) {
                effectableEntity.addEffect(markEffect.InitializeEffect(effectableEntity.gameObject));
            }
        }
    }
}

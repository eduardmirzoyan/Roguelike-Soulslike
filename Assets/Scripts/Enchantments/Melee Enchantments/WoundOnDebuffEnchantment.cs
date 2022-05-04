using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Wound on Debuff")]
public class WoundOnDebuffEnchantment : MeleeEchantment
{
    [SerializeField] private int minDebuffCount = 1;
    [SerializeField] private WoundedEffect woundedEffect;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        GameEvents.instance.onWeaponHit += attemptToWound;
    }

    public override void unintialize()
    {
        GameEvents.instance.onWeaponHit -= attemptToWound;
        meleeWeapon = null;
        base.unintialize();
    }

    public void attemptToWound(Weapon weapon, GameObject hitEntity) {
        if (weapon == meleeWeapon && hitEntity.TryGetComponent(out EffectableEntity effectableEntity)) {
            // If amount of effects passed the min requirement, then apply wound
            int offset = 0;
            if (effectableEntity.containsEffect(woundedEffect)) {
                Debug.Log("contains wound!");
                offset = 1;
            }

            if (effectableEntity.getActiveEffectCount() + offset >= minDebuffCount) {
                // Add effect
                effectableEntity.addEffect(woundedEffect.InitializeEffect(hitEntity.gameObject));
            }
        }
            
    }
}

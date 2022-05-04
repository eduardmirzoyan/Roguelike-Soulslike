using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Blind Melee")]
public class BlindOnHitEnchantment : MeleeEchantment
{
    [SerializeField] private BlindEffect blindEffect;

    [SerializeField] private float chance;

    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();

        // Subscribe
        GameEvents.instance.onWeaponHit += applyBlind;
    }

    public override void unintialize()
    {
        // Unsubscribe
        GameEvents.instance.onWeaponHit -= applyBlind;
        meleeWeapon = null;
        base.unintialize();
    }

    private void applyBlind(Weapon weapon, GameObject hitEntity) {
        int roll = Random.Range(0, 100);
        // Roll to see if blind is applied
        if(roll <= chance * 100 )
        {
            // Apply blind
            if (hitEntity.TryGetComponent(out EffectableEntity effectableEntity)) {
                effectableEntity.addEffect(blindEffect.InitializeEffect(hitEntity));
            }
        }
    }
}

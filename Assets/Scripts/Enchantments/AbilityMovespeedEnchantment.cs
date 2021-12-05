using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/AbilitySpeedboost")]
public class AbilityMovespeedEnchantment : Enchantment
{
    [SerializeField] private BaseEffect speedEffect;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        GameEvents.current.onPlayerUseSignature += speedBoostOnSpecialAbility;
    }

    public override void unintialize()
    {
        GameEvents.current.onPlayerUseSignature -= speedBoostOnSpecialAbility;
        base.unintialize();
    }

    private void speedBoostOnSpecialAbility()
    {
        entity.GetComponent<EffectableEntity>().addEffect(speedEffect.InitializeEffect(entity));
    }
}

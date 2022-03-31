using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Faster Roll")]
public class FasterRollingEnchantment : Enchantment
{
    [SerializeField] private float ratio = 1.5f;
    private Rolling rolling;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        rolling = entity.GetComponent<Rolling>();
        if (rolling != null) {
            rolling.setRollDuration(rolling.getRollDuration() / ratio);
            rolling.setRollSpeed(rolling.getRollSpeed() * ratio);
        }
    }

    public override void unintialize()
    {
        if (rolling != null) {
            rolling.setRollDuration(rolling.getRollDuration() * ratio);
            rolling.setRollSpeed(rolling.getRollSpeed() / ratio);
            rolling = null;
        }
        
        base.unintialize();
    }
}

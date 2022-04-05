using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Faster Roll")]
public class FasterRollingEnchantment : Enchantment
{
    [SerializeField] private float ratio = 1.5f;
    private RollingHandler rollingHandler;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        rollingHandler = entity.GetComponent<RollingHandler>();
        if (rollingHandler != null) {
            rollingHandler.setRollDuration(rollingHandler.getRollDuration() / ratio);
            rollingHandler.setRollSpeed(rollingHandler.getRollSpeed() * ratio);
        }
    }

    public override void unintialize()
    {
        if (rollingHandler != null) {
            rollingHandler.setRollDuration(rollingHandler.getRollDuration() * ratio);
            rollingHandler.setRollSpeed(rollingHandler.getRollSpeed() / ratio);
            rollingHandler = null;
        }
        
        base.unintialize();
    }
}

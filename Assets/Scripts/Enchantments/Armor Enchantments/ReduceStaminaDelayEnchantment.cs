using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Reduce Stamina Delay")]
public class ReduceStaminaDelayEnchantment : Enchantment
{
    [SerializeField] private float reducePercent = 0.5f;
    private Stamina stamina;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        stamina = entity.GetComponent<Stamina>();
        if (stamina != null) {
            stamina.changeDelay(reducePercent);
        }
    }

    public override void unintialize()
    {
        if (stamina != null) {
            stamina.changeDelay(1 / reducePercent);
        }
        stamina = null;
        base.unintialize();
    }
}

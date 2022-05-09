using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Reduce Stamina Duration")]
public class ReduceStaminaDurationEnchantment : Enchantment
{
    [SerializeField] private float reducePercent = 0.5f;
    private Stamina stamina;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        stamina = entity.GetComponent<Stamina>();
        if (stamina != null) {
            stamina.changeDuration(reducePercent);
        }
    }

    public override void unintialize()
    {
        if (stamina != null) {
            stamina.changeDuration(1 / reducePercent);
        }
        stamina = null;
        base.unintialize();
    }
}

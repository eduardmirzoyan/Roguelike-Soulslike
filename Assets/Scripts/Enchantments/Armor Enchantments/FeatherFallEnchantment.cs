using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Changes the gravity scale of entity
[CreateAssetMenu(menuName = "Enchantments/Featherfall")]
public class FeatherFallEnchantment : Enchantment
{
    private ComplexMovement cmv;
    private FallDamage fallDamage;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        cmv = entity.GetComponent<ComplexMovement>();
        fallDamage = entity.GetComponent<FallDamage>();

        if (cmv != null)
        {
            cmv.setFallMult(cmv.getFallMult() / 5);
            if (fallDamage != null)
                fallDamage.enableFallDamage = false;
        }
    }

    public override void unintialize()
    {
        cmv = entity.GetComponent<ComplexMovement>();
        if (cmv != null)
        {
            cmv.setFallMult(cmv.getFallMult() * 5);
            if (fallDamage != null)
                fallDamage.enableFallDamage = true;
        }
        base.unintialize();
    }
}

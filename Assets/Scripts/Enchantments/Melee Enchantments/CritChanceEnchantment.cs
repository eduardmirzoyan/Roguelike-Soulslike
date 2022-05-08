using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Increased Crit Chance")]
public class CritChanceEnchantment : MeleeEchantment
{
    [SerializeField] private float bonusAmount = 0.15f;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        Debug.Log("DOES NOTHING");
    }

    public override void unintialize()
    {
        Debug.Log("STILL DOES NOTHING");
        base.unintialize();
    }
}

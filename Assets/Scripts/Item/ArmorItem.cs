using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EquipmentSlot { Helmet, Chestpiece, Cape, Gloves, Boots }

public enum ArmorType { Light, Medium, Heavy }

[CreateAssetMenu]
public class ArmorItem : Item
{
    public EquipmentSlot equipSlot;
    public ArmorType armorType;

    public Enchantment enchantment;

    public int defenseValue;
    public int bonusStamina;

    public int poisonResistance;
    public int burnResistance;
    public int freezeResistance;
    public int curseResistance;
    public int dodgeChance;

    public float damageTakenMultiplier;

    public void Awake()
    {
        type = ItemType.Armor;
        isStackable = false;
        count = 1;
    }
}

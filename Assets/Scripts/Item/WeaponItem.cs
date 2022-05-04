using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    Axe,
    SmallShield,
    LongBow,
    Spear,
    Dagger
}

[CreateAssetMenu]
public class WeaponItem : Item
{
    public int damage;
    public float critChance = 0f;
    public float critDamage = 1f;
    public WeaponType weaponType;
    public bool twoHanded;
    public List<Enchantment> enchantments;
    public int enchantmentSlots = 3;

    public void Awake()
    {
        type = ItemType.Weapon;
        isStackable = false;
        count = 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    Axe,
    SmallShield,
    LongBow
}

[CreateAssetMenu]
public class WeaponItem : Item
{
    public int damage;
    public WeaponType weaponType;
    public bool twoHanded;
    public Enchantment enchantment;

    public void Awake()
    {
        type = ItemType.Weapon;
        isStackable = false;
        count = 1;
    }
}

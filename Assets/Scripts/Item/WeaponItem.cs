using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    Axe,
    Spear
}

[CreateAssetMenu]
public class WeaponItem : Item
{
    public int damage;
    public WeaponType weaponType;
    public bool twoHanded;
    public MeleeEchantment enchantment;

    public void Awake()
    {
        type = ItemType.Weapon;
        isStackable = false;
        count = 1;
    }
}

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
    public int lightDamage;
    public int heavyDamage;

    public WeaponType weaponType;

    public void Awake()
    {
        type = ItemType.Weapon;
        isStackable = false;
        count = 1;
    }
}

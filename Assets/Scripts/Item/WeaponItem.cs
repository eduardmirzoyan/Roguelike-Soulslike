using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    GreatAxe,
    SmallShield,
    LongBow,
    Spear,
    Dagger,
    Rapier,
    Axe
}

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItem : Item
{
    public int damage;
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

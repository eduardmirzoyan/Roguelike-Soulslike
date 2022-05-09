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

public enum WeaponSize { Small, Medium, Large };

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItem : Item
{
    public int damage;
    public float critDamage = 1f;
    public float staminaCostMultiplier = 1f;
    public WeaponType weaponType;
    public WeaponSize weaponSize;
    public bool twoHanded;
    public List<Enchantment> enchantments;
    public int enchantmentSlots = 3;

    public void Awake()
    {
        type = ItemType.Weapon;
        isStackable = false;
        count = 1;
    }

    public int staminaCost() {
        return (int) (((int) weaponSize + 2) * 5 * staminaCostMultiplier);
    }

    public int rawStaminaCost() {
        return ((int) weaponSize + 2) * 5;
    }
}

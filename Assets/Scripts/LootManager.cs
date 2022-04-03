using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager instance; // Accessible by every class at any point

    [SerializeField] private List<string> prefixes;
    [SerializeField] private List<Sprite> swordSprites;
    [SerializeField] private List<Sprite> axeSprites;
    [SerializeField] private List<Sprite> shieldSprites;
    [SerializeField] private List<MeleeEchantment> meleeEchantments;

    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private GameObject smallShieldPrefab;

    private void Awake()
    {
        // If another lootmanager exists, destroy it
        if(LootManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; // Sets this to itself
        
        DontDestroyOnLoad(gameObject);
    }

    public Item getRandomItem()
    {
        // First randomly choose between a weapon or armor
        var itemType = (ItemType)Random.Range(0, 2); // 0 - weapon, 1 - armor
        itemType = 0; // Hard code 0 for now

        // Then randomly choose weapon type or armor type (light, med, heavy)
        switch (itemType)
        {
            case ItemType.Weapon:
                return generateRandomWeapon();
            case ItemType.Armor:
                var armorSlot = (EquipmentSlot)Random.Range(0, 5);
                var armorType = (ArmorType)Random.Range(0, 3);

                // Set the sprite

                // Then set the level of the gear based on floor level

                // Create the SO item
                ArmorItem armorItem = new ArmorItem();
                armorItem.armorType = armorType;
                armorItem.equipSlot = armorSlot;
                armorItem.name = "Random Armor";
                armorItem.defenseValue = Random.Range(1, 10);
                armorItem.bonusStamina = Random.Range(0, 3) * 5;
                // armorItem.sprite = icon2;

                return armorItem;

                //break;
        }

        // If no appropriate item was created, then return null
        return null;

        // Then decide the core stats of gear based on gear level

        // Finally randomize # of sub stats
    }

    private WeaponItem generateRandomWeapon() {
        var weaponType = (WeaponType)Random.Range(0, 4); // 0-3
        switch(weaponType) {
            case WeaponType.Sword:
                return generateRandomSword();
            case WeaponType.Axe:
                return generateRandomAxe();
            case WeaponType.SmallShield:
                return generateRandomSmallShield();
            case WeaponType.LongBow:
                return generateRandomSword();
        }
        return null;
    }

    private WeaponItem generateRandomSword() {

        // Create the scriptable object
        WeaponItem newSword = ScriptableObject.CreateInstance<WeaponItem>();
        newSword.weaponType = WeaponType.Sword;
        newSword.twoHanded = false;
        newSword.prefab = swordPrefab;

        // Randomize damage stats
        newSword.damage = Random.Range(1, 8); // 1-7

        // Randomize enchantment
        newSword.enchantment = meleeEchantments[Random.Range(0, meleeEchantments.Count)];

        // Randomize sprite
        newSword.sprite = swordSprites[Random.Range(0, swordSprites.Count)];

        // Randomize name
        newSword.name = prefixes[Random.Range(0, prefixes.Count)] + " Sword";

        return newSword;
    }

    private WeaponItem generateRandomAxe() {

        // Create the scriptable object
        WeaponItem newAxe = ScriptableObject.CreateInstance<WeaponItem>();
        newAxe.weaponType = WeaponType.Axe;
        newAxe.twoHanded = true;
        newAxe.prefab = axePrefab;

        // Randomize damage stats
        newAxe.damage = Random.Range(5, 13); // 5-12

        // Randomize enchantment
        newAxe.enchantment = meleeEchantments[Random.Range(0, meleeEchantments.Count)];

        // Randomize sprite
        newAxe.sprite = shieldSprites[Random.Range(0, shieldSprites.Count)];

        // Randomize name
        newAxe.name = prefixes[Random.Range(0, prefixes.Count)] + " Axe";

        return newAxe;
    }

    private WeaponItem generateRandomSmallShield() {

        // Create the scriptable object
        WeaponItem newSmallShield = ScriptableObject.CreateInstance<WeaponItem>();
        newSmallShield.weaponType = WeaponType.SmallShield;
        newSmallShield.twoHanded = false;
        newSmallShield.prefab = smallShieldPrefab;

        // Randomize damage stats
        newSmallShield.damage = Random.Range(1, 2); // 1

        // Randomize enchantment
        newSmallShield.enchantment = meleeEchantments[Random.Range(0, meleeEchantments.Count)];

        // Randomize sprite
        newSmallShield.sprite = axeSprites[Random.Range(0, axeSprites.Count)];

        // Randomize name
        newSmallShield.name = prefixes[Random.Range(0, prefixes.Count)] + " Shield";

        return newSmallShield;
    }

    
}

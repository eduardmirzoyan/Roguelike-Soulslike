using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager instance; // Accessible by every class at any point

    [Header("Melee Weapons")]
    [SerializeField] private List<string> prefixes;
    [SerializeField] private List<Sprite> swordSprites;
    [SerializeField] private List<Sprite> axeSprites;
    [SerializeField] private List<Sprite> shieldSprites;
    [SerializeField] private List<Sprite> spearSprites;
    [SerializeField] private List<MeleeEchantment> meleeEchantments;

    [Header("Ranged Weapons")]
    [SerializeField] private List<Sprite> longBowSprites;
    [SerializeField] private List<RangedEnchantment> rangedEnchantments;

    [Header("Weapon Prefabs")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private GameObject smallShieldPrefab;
    [SerializeField] private GameObject longBowPrefab;
    [SerializeField] private GameObject spearPrefab;

    [Header("Armor")]
    [SerializeField] private List<Sprite> helmetSprites;
    [SerializeField] private List<Sprite> chestplateSprites;
    [SerializeField] private List<Sprite> gloveSprites;
    [SerializeField] private List<Sprite> bootsSprites;
    [SerializeField] private List<Sprite> capeSprites;
    [SerializeField] private List<Enchantment> armorEnchantments;

    [Header("Loot tables")]
    [SerializeField] private LootTable consumableLootTable;

    [Header("Weapon Descriptions")]
    [TextArea(10, 15)]
    public string swordDescription;

    [TextArea(10, 15)]
    public string axeDescription;

    [TextArea(10, 15)]
    public string smallShieldDescription;

    [TextArea(10, 15)]
    public string longBowDescription;
    [TextArea(10, 15)]
    public string spearDescription;


    [Header("Debugging")]
    [SerializeField] private bool alwaysEnchant;

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

        // Then randomly choose weapon type or armor type
        switch (itemType)
        {
            case ItemType.Weapon:
                return generateRandomWeapon();
            case ItemType.Armor:
                return generateRandomArmorPiece();
            default:
                return null;
        }
    }

    private WeaponItem generateRandomWeapon() {
        var weaponType = (WeaponType)Random.Range(0, 5); // 0 - 4
        switch(weaponType) {
            case WeaponType.Sword:
                return generateRandomSword();
            case WeaponType.Axe:
                return generateRandomAxe();
            case WeaponType.SmallShield:
                return generateRandomSmallShield();
            case WeaponType.LongBow:
                return generateRandomLongBow();
            case WeaponType.Spear:
                return generateRandomSpear();
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

        // Set crit chance
        newSword.critChance = 0.1f; // 1-7

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newSword.enchantment = meleeEchantments[Random.Range(0, meleeEchantments.Count)];
        }
        else {
            newSword.enchantment = null;
        }
        
        // Randomize sprite
        newSword.sprite = swordSprites[Random.Range(0, swordSprites.Count)];

        // Randomize name
        newSword.name = prefixes[Random.Range(0, prefixes.Count)] + " Sword";

        // Set description
        newSword.description = swordDescription;

        return newSword;
    }

    private WeaponItem generateRandomAxe() {
        // Create the scriptable object
        WeaponItem newAxe = ScriptableObject.CreateInstance<WeaponItem>();
        newAxe.weaponType = WeaponType.Axe;
        newAxe.twoHanded = false;
        newAxe.prefab = axePrefab;

        // Randomize damage stats
        newAxe.damage = Random.Range(5, 13); // 5-12

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newAxe.enchantment = meleeEchantments[Random.Range(0, meleeEchantments.Count)];
        }
        else {
            newAxe.enchantment = null;
        }

        // Randomize sprite
        newAxe.sprite = axeSprites[Random.Range(0, axeSprites.Count)];

        // Randomize name
        newAxe.name = prefixes[Random.Range(0, prefixes.Count)] + " Axe";

        // Set description
        newAxe.description = axeDescription;

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

        // Set crit chance
        newSmallShield.critChance = 0.1f; // 1-7

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newSmallShield.enchantment = meleeEchantments[Random.Range(0, meleeEchantments.Count)];
        }
        else {
            newSmallShield.enchantment = null;
        }

        // Randomize sprite
        newSmallShield.sprite = shieldSprites[Random.Range(0, shieldSprites.Count)];

        // Randomize name
        newSmallShield.name = prefixes[Random.Range(0, prefixes.Count)] + " Shield";

        // Set description
        newSmallShield.description = smallShieldDescription;

        return newSmallShield;
    }

    private WeaponItem generateRandomSpear() {

        // Create the scriptable object
        WeaponItem newSpear = ScriptableObject.CreateInstance<WeaponItem>();
        newSpear.weaponType = WeaponType.Spear;
        newSpear.twoHanded = false;
        newSpear.prefab = spearPrefab;

        // Randomize damage stats
        newSpear.damage = Random.Range(4, 5);

        // Set crit chance
        newSpear.critChance = 0.1f;

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newSpear.enchantment = meleeEchantments[Random.Range(0, meleeEchantments.Count)];
        }
        else {
            newSpear.enchantment = null;
        }

        // Randomize sprite
        newSpear.sprite = spearSprites[Random.Range(0, spearSprites.Count)];

        // Randomize name
        newSpear.name = prefixes[Random.Range(0, prefixes.Count)] + " Spear";

        // Set description
        newSpear.description = spearDescription;

        return newSpear;
    }

    private WeaponItem generateRandomLongBow() {

        // Create the scriptable object
        WeaponItem newLongBow = ScriptableObject.CreateInstance<WeaponItem>();
        newLongBow.weaponType = WeaponType.LongBow;
        newLongBow.twoHanded = false;
        newLongBow.prefab = longBowPrefab;

        // Randomize damage stats
        newLongBow.damage = Random.Range(6, 8);

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newLongBow.enchantment = rangedEnchantments[Random.Range(0, rangedEnchantments.Count)];
        }
        else {
            newLongBow.enchantment = null;
        }

        // Randomize sprite
        newLongBow.sprite = longBowSprites[Random.Range(0, longBowSprites.Count)];

        // Randomize name
        newLongBow.name = prefixes[Random.Range(0, prefixes.Count)] + " Long Bow";

        // Set description
        newLongBow.description = longBowDescription;

        return newLongBow;
    }

    private ArmorItem generateRandomArmorPiece() {
        
        var armorType = (EquipmentSlot)Random.Range(0, 5); // 0 - 4
        switch(armorType) {
            case EquipmentSlot.Helmet:
                return generateRandomHelmet();
            case EquipmentSlot.Chestpiece:
                return generateRandomChestplate();
            case EquipmentSlot.Gloves:
                return generateRandomGlove();
            case EquipmentSlot.Boots:
                return generateRandomBoots();
            case EquipmentSlot.Cape:
                return generateRandomCape();
            default:
                return null;
        }
    }

    private ArmorItem generateRandomHelmet() {
        // Create the scriptable object
        ArmorItem newArmor = ScriptableObject.CreateInstance<ArmorItem>();
        newArmor.equipSlot = EquipmentSlot.Helmet;
        newArmor.armorType = (ArmorType)Random.Range(0, 3); // 0 - 2

        // Randomize armor stats based on armor type
        newArmor.defenseValue = Random.Range(1, 5) * (1 + (int)newArmor.armorType);

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newArmor.enchantment = armorEnchantments[Random.Range(0, armorEnchantments.Count)];
        }
        else {
            newArmor.enchantment = null;
        }

        // Randomize sprite
        newArmor.sprite = helmetSprites[Random.Range(0, helmetSprites.Count)];

        // Randomize name
        newArmor.name = prefixes[Random.Range(0, prefixes.Count)] + " Helmet";

        return newArmor;
    }

    private ArmorItem generateRandomChestplate() {
        // Create the scriptable object
        ArmorItem newArmor = ScriptableObject.CreateInstance<ArmorItem>();
        newArmor.equipSlot = EquipmentSlot.Chestpiece;
        newArmor.armorType = (ArmorType)Random.Range(0, 3); // 0 - 2

        // Randomize armor stats based on armor type
        newArmor.defenseValue = Random.Range(1, 5) * (1 + (int)newArmor.armorType);

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newArmor.enchantment = armorEnchantments[Random.Range(0, armorEnchantments.Count)];
        }
        else {
            newArmor.enchantment = null;
        }

        // Randomize sprite
        newArmor.sprite = chestplateSprites[Random.Range(0, chestplateSprites.Count)];

        // Randomize name
        newArmor.name = prefixes[Random.Range(0, prefixes.Count)] + " Chestplate";
        
        return newArmor;
    }

    private ArmorItem generateRandomGlove() {
        // Create the scriptable object
        ArmorItem newArmor = ScriptableObject.CreateInstance<ArmorItem>();
        newArmor.equipSlot = EquipmentSlot.Gloves;
        newArmor.armorType = (ArmorType)Random.Range(0, 3); // 0 - 2

        // Randomize armor stats based on armor type
        newArmor.defenseValue = Random.Range(1, 5) * (1 + (int)newArmor.armorType);

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newArmor.enchantment = armorEnchantments[Random.Range(0, armorEnchantments.Count)];
        }
        else {
            newArmor.enchantment = null;
        }

        // Randomize sprite
        newArmor.sprite = gloveSprites[Random.Range(0, gloveSprites.Count)];

        // Randomize name
        newArmor.name = prefixes[Random.Range(0, prefixes.Count)] + " Gloves";
        
        return newArmor;
    }

    private ArmorItem generateRandomBoots() {
        // Create the scriptable object
        ArmorItem newArmor = ScriptableObject.CreateInstance<ArmorItem>();
        newArmor.equipSlot = EquipmentSlot.Boots;
        newArmor.armorType = (ArmorType)Random.Range(0, 3); // 0 - 2

        // Randomize armor stats based on armor type
        newArmor.defenseValue = Random.Range(1, 5) * (1 + (int)newArmor.armorType);

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newArmor.enchantment = armorEnchantments[Random.Range(0, armorEnchantments.Count)];
        }
        else {
            newArmor.enchantment = null;
        }

        // Randomize sprite
        newArmor.sprite = bootsSprites[Random.Range(0, bootsSprites.Count)];

        // Randomize name
        newArmor.name = prefixes[Random.Range(0, prefixes.Count)] + " Boots";
        
        return newArmor;
    }

    private ArmorItem generateRandomCape() {
        // Create the scriptable object
        ArmorItem newArmor = ScriptableObject.CreateInstance<ArmorItem>();
        newArmor.equipSlot = EquipmentSlot.Cape;
        newArmor.armorType = (ArmorType)Random.Range(0, 3); // 0 - 2

        // Randomize armor stats based on armor type
        newArmor.defenseValue = Random.Range(1, 5) * (1 + (int)newArmor.armorType);

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newArmor.enchantment = armorEnchantments[Random.Range(0, armorEnchantments.Count)];
        }
        else {
            newArmor.enchantment = null;
        }

        // Randomize sprite
        newArmor.sprite = capeSprites[Random.Range(0, capeSprites.Count)];

        // Randomize name
        newArmor.name = prefixes[Random.Range(0, prefixes.Count)] + " Cape";
        
        return newArmor;
    }

    public Item getConsumableDrop()
    {
        return consumableLootTable.getDrop();
    }
}

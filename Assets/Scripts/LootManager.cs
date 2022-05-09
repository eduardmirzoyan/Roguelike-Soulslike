using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager instance; // Accessible by every class at any point

    [Header("General Settings")]
    [SerializeField] private int maxSlots = 4;
    [SerializeField] private int minSlots = 2;

    [Header("Melee Weapons")]
    [SerializeField] private List<string> prefixes;
    [SerializeField] private List<Sprite> swordSprites;
    [SerializeField] private List<Sprite> greatAxeSprites;
    [SerializeField] private List<Sprite> shieldSprites;
    [SerializeField] private List<Sprite> spearSprites;
    [SerializeField] private List<Sprite> daggerSprites;
    [SerializeField] private List<Sprite> axeSprites;
    [SerializeField] private List<Sprite> rapierSprites;
    [SerializeField] private List<MeleeEchantment> meleeEchantments;

    [Header("Ranged Weapons")]
    [SerializeField] private List<Sprite> longBowSprites;
    [SerializeField] private List<RangedEnchantment> rangedEnchantments;

    [Header("Weapon Prefabs")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private GameObject greatAxePrefab;
    [SerializeField] private GameObject smallShieldPrefab;
    [SerializeField] private GameObject longBowPrefab;
    [SerializeField] private GameObject spearPrefab;
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private GameObject rapierPrefab;

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
    [TextArea(5, 10)]
    public string swordDescription;

    [TextArea(5, 10)]
    public string greatAxeDescription;

    [TextArea(5, 10)]
    public string smallShieldDescription;

    [TextArea(5, 10)]
    public string longBowDescription;

    [TextArea(5, 10)]
    public string spearDescription;

    [TextArea(5, 10)]
    public string daggerDescription;

    [TextArea(5, 10)]
    public string axeDescription;

    [TextArea(5, 10)]
    public string rapierDescription;


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
        var roll = Random.Range(0, 10); // 0 - 9

        if (roll > 3) {
            return generateRandomWeapon();
        }
        return generateRandomArmorPiece();
    }

    private WeaponItem generateRandomWeapon() {
        var roll = Random.Range(0, 7); // 0 - 6
        switch(roll) {
            case 0:
                return generateRandomSword();
            case 1:
                return generateRandomGreatAxe();
            case 2:
                return generateRandomLongBow();
            case 3:
                return generateRandomSpear();
            case 4:
                return generateRandomDagger();
            case 5:
                return generateRandomAxe();
            case 6:
                return generateRandomRapier();
        }
        print("Error while generating loot");
        return null;
    }

    private WeaponItem generateRandomSword() {

        // Create the scriptable object
        WeaponItem newSword = ScriptableObject.CreateInstance<WeaponItem>();
        newSword.weaponType = WeaponType.Sword;
        newSword.twoHanded = false;
        newSword.prefab = swordPrefab;

        // Randomize damage stats
        newSword.damage = Random.Range(3, 8); // 3-7

        // Set weapon size
        newSword.weaponSize = WeaponSize.Medium;

        // Randomly choose up to maxSltos
        int roll = Random.Range(minSlots, maxSlots + 1);
        newSword.enchantmentSlots = roll;

        // Randomly choose between 0 and amount of slots
        roll = Random.Range(0, minSlots + 1); 
        if (roll == 0 && alwaysEnchant)
            roll = 1;
        newSword.enchantments = getRandomMeleeEnchantments(roll);
        
        // Randomize sprite
        newSword.sprite = swordSprites[Random.Range(0, swordSprites.Count)];

        // Randomize name
        newSword.name = prefixes[Random.Range(0, prefixes.Count)] + " Sword";

        // Set description
        newSword.description = swordDescription;

        return newSword;
    }

    private WeaponItem generateRandomGreatAxe() {
        // Create the scriptable object
        WeaponItem newAxe = ScriptableObject.CreateInstance<WeaponItem>();
        newAxe.weaponType = WeaponType.GreatAxe;
        newAxe.twoHanded = false;
        newAxe.prefab = greatAxePrefab;

        // Randomize damage stats
        newAxe.damage = Random.Range(5, 13); // 5-12

        // Set weapon size
        newAxe.weaponSize = WeaponSize.Large;

        // Randomly choose up to maxSltos
        int roll = Random.Range(minSlots, maxSlots + 1);
        newAxe.enchantmentSlots = roll;

        // Randomly choose between 0 and amount of slots
        roll = Random.Range(0, minSlots + 1); 
        if (roll == 0 && alwaysEnchant)
            roll = 1;
        newAxe.enchantments = getRandomMeleeEnchantments(roll);

        // Randomize sprite
        newAxe.sprite = greatAxeSprites[Random.Range(0, greatAxeSprites.Count)];

        // Randomize name
        newAxe.name = prefixes[Random.Range(0, prefixes.Count)] + " Great Axe";

        // Set description
        newAxe.description = greatAxeDescription;

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

        // Randomly choose up to maxSltos
        int roll = Random.Range(minSlots, maxSlots + 1);
        newSmallShield.enchantmentSlots = roll;

        // Randomly choose between 0 and amount of slots
        roll = Random.Range(0, minSlots + 1); 
        if (roll == 0 && alwaysEnchant)
            roll = 1;
        newSmallShield.enchantments = getRandomMeleeEnchantments(roll);

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

        // Set weapon size
        newSpear.weaponSize = WeaponSize.Medium;

        // Randomly choose up to maxSltos
        int roll = Random.Range(minSlots, maxSlots + 1);
        newSpear.enchantmentSlots = roll;

        // Randomly choose between 0 and amount of slots
        roll = Random.Range(0, minSlots + 1); 
        if (roll == 0 && alwaysEnchant)
            roll = 1;
        newSpear.enchantments = getRandomMeleeEnchantments(roll);

        // Randomize sprite
        newSpear.sprite = spearSprites[Random.Range(0, spearSprites.Count)];

        // Randomize name
        newSpear.name = prefixes[Random.Range(0, prefixes.Count)] + " Spear";

        // Set description
        newSpear.description = spearDescription;

        return newSpear;
    }

    private WeaponItem generateRandomDagger() {

        // Create the scriptable object
        WeaponItem newDagger = ScriptableObject.CreateInstance<WeaponItem>();
        newDagger.weaponType = WeaponType.Dagger;
        newDagger.twoHanded = false;
        newDagger.prefab = daggerPrefab;

        // Randomize damage stats
        newDagger.damage = Random.Range(1, 4);

        // Set weapon size
        newDagger.weaponSize = WeaponSize.Small;

        // Randomly choose up to maxSltos
        int roll = Random.Range(minSlots, maxSlots + 1);
        newDagger.enchantmentSlots = roll;

        // Randomly choose between 0 and amount of slots
        roll = Random.Range(0, minSlots + 1); 
        if (roll == 0 && alwaysEnchant)
            roll = 1;
        newDagger.enchantments = getRandomMeleeEnchantments(roll);
        
        // Randomize sprite
        newDagger.sprite = daggerSprites[Random.Range(0, daggerSprites.Count)];

        // Randomize name
        newDagger.name = prefixes[Random.Range(0, prefixes.Count)] + " Dagger";

        // Set description
        newDagger.description = daggerDescription;

        return newDagger;
    }

    private WeaponItem generateRandomAxe() {

        // Create the scriptable object
        WeaponItem newAxe = ScriptableObject.CreateInstance<WeaponItem>();
        newAxe.weaponType = WeaponType.Axe;
        newAxe.twoHanded = false;
        newAxe.prefab = axePrefab;

        // Randomize damage stats
        newAxe.damage = Random.Range(1, 4);

        // Set weapon size
        newAxe.weaponSize = WeaponSize.Small;

        // Randomly choose up to maxSlots
        int roll = Random.Range(minSlots, maxSlots + 1);
        newAxe.enchantmentSlots = roll;

        // Randomly choose between 0 and amount of slots
        roll = Random.Range(0, minSlots + 1); 
        if (roll == 0 && alwaysEnchant)
            roll = 1;
        newAxe.enchantments = getRandomMeleeEnchantments(roll);
        
        // Randomize sprite
        newAxe.sprite = axeSprites[Random.Range(0, axeSprites.Count)];

        // Randomize name
        newAxe.name = prefixes[Random.Range(0, prefixes.Count)] + " Axe";

        // Set description
        newAxe.description = axeDescription;

        return newAxe;
    }

    private WeaponItem generateRandomRapier() {

        // Create the scriptable object
        WeaponItem newRapier = ScriptableObject.CreateInstance<WeaponItem>();
        newRapier.weaponType = WeaponType.Rapier;
        newRapier.twoHanded = false;
        newRapier.prefab = rapierPrefab;

        // Randomize damage stats
        newRapier.damage = Random.Range(3, 7);

        // Set weapon size
        newRapier.weaponSize = WeaponSize.Medium;

        // Randomly choose up to maxSlots
        int roll = Random.Range(minSlots, maxSlots + 1);
        newRapier.enchantmentSlots = roll;

        // Randomly choose between 0 and amount of slots
        roll = Random.Range(0, minSlots + 1); 
        if (roll == 0 && alwaysEnchant)
            roll = 1;
        newRapier.enchantments = getRandomMeleeEnchantments(roll);
        
        // Randomize sprite
        newRapier.sprite = rapierSprites[Random.Range(0, rapierSprites.Count)];

        // Randomize name
        newRapier.name = prefixes[Random.Range(0, prefixes.Count)] + " Rapier";

        // Set description
        newRapier.description = rapierDescription;

        return newRapier;
    }

    private WeaponItem generateRandomLongBow() {

        // Create the scriptable object
        WeaponItem newLongBow = ScriptableObject.CreateInstance<WeaponItem>();
        newLongBow.weaponType = WeaponType.LongBow;
        newLongBow.twoHanded = false;
        newLongBow.prefab = longBowPrefab;

        // Randomize damage stats
        newLongBow.damage = Random.Range(6, 8);

        // Set weapon size
        newLongBow.weaponSize = WeaponSize.Medium;

        // Randomize enchantment
        if (Random.Range(0, 2) == 0 || alwaysEnchant) {
            newLongBow.enchantments = new List<Enchantment>();
            newLongBow.enchantments.Add(rangedEnchantments[Random.Range(0, rangedEnchantments.Count)]);
        }
        else {
            newLongBow.enchantments = null;
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

        // Set stamina bonus
        newArmor.bonusStamina = 10;

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

        // Set stamina bonus
        newArmor.bonusStamina = 10;

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

        // Set stamina bonus
        newArmor.bonusStamina = 10;

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

        // Set stamina bonus
        newArmor.bonusStamina = 10;

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

        // Set stamina bonus
        newArmor.bonusStamina = 10;

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

    private List<Enchantment> getRandomMeleeEnchantments(int count) {
        if (count <= 0)
            return null;

        List<Enchantment> enchantments = new List<Enchantment>();
        List<MeleeEchantment> possibleChoices = new List<MeleeEchantment>();
        // Copy list
        possibleChoices.AddRange(meleeEchantments);

        for (int i = 0; i < count; i++)
        {
            // If you have no choices left, then leave
            if (possibleChoices.Count <= 0)
                break;
            
            // Randomly choose from pool
            var randomChoice = possibleChoices[Random.Range(0, possibleChoices.Count)];
            // If enchantment was already chosen, then remove it from the pool and retry next iteration
            if (enchantments.Contains(randomChoice)) {
                // Remove enchantment from pool
                possibleChoices.Remove(randomChoice);
                // Move back counter
                i--;
            }
            else { // Add enchantment
                enchantments.Add(randomChoice);
            }
        }

        return enchantments;
    }
}

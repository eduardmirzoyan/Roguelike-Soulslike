using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;
    [SerializeField] private Sprite[] swordSprites;
    [SerializeField] private Sprite[] axeSprites;
    [SerializeField] private Sprite[] helmetSprites;
    [SerializeField] private Sprite[] chestplateSprites;
    [SerializeField] private Sprite[] gloveSprites;
    [SerializeField] private Sprite[] bootsSprites;
    [SerializeField] private Sprite[] capeSprites;

    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private GameObject axePrefab;

    // Singleton logic
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this; // Sets this to itself

        DontDestroyOnLoad(gameObject);
    }

    public Item randomizeItem()
    {
        // First randomly choose between a weapon or armor
        var itemType = (ItemType)Random.Range(1, 3); // Gives (1, weapon) or (2, armor)

        // Then randomly choose weapon type or armor type (light, med, heavy)
        switch (itemType)
        {
            case ItemType.Weapon:
                // Create the SO item
                WeaponItem weaponItem = new WeaponItem();

                // Generate random weapontype
                var weaponType = (WeaponType)Random.Range(0, 2);        
                weaponItem.weaponType = weaponType;
                
                // Set random sprite
                switch (weaponItem.weaponType)
                {
                    case WeaponType.Sword:
                        weaponItem.prefab = swordPrefab;
                        weaponItem.sprite = swordSprites[Random.Range(0, swordSprites.Length)];
                        weaponItem.name = "Randomized Sword";
                        weaponItem.damage = 4;
                        break;

                    case WeaponType.GreatAxe:
                        weaponItem.prefab = axePrefab;
                        weaponItem.sprite = axeSprites[Random.Range(0, axeSprites.Length)];
                        weaponItem.name = "Randomized Axe";
                        weaponItem.damage = 9;
                        break;
                }

                return weaponItem;
            //break;
            case ItemType.Armor:
                var armorSlot = (EquipmentSlot)Random.Range(0, 5);
                var armorType = (ArmorType)Random.Range(0, 3);

                // Then set the level of the gear based on floor level

                // Create the SO item
                ArmorItem armorItem = new ArmorItem();
                armorItem.armorType = armorType;
                armorItem.equipSlot = armorSlot;

                // Based on armor type, give core stats TO BE CHANGED
                switch (armorItem.armorType)
                {
                    case ArmorType.Light:
                        armorItem.defenseValue = Random.Range(1, 3);
                        armorItem.bonusStamina = 10;
                        break;
                    case ArmorType.Medium:
                        armorItem.defenseValue = Random.Range(4, 6);
                        armorItem.bonusStamina = 5;
                        break;
                    case ArmorType.Heavy:
                        armorItem.defenseValue = Random.Range(7, 9);
                        armorItem.bonusStamina = 0;
                        break;
                }

                // Based on equip slot, give sprite and name
                switch (armorItem.equipSlot)
                {
                    case EquipmentSlot.Helmet:
                        armorItem.name = "Randomized Helmet";
                        armorItem.sprite = helmetSprites[Random.Range(0, helmetSprites.Length)];
                        break;
                    case EquipmentSlot.Chestpiece:
                        armorItem.name = "Randomized Chestplate";
                        armorItem.sprite = chestplateSprites[Random.Range(0, chestplateSprites.Length)];
                        break;
                    case EquipmentSlot.Gloves:
                        armorItem.name = "Randomized Gloves";
                        armorItem.sprite = gloveSprites[Random.Range(0, gloveSprites.Length)];
                        break;
                    case EquipmentSlot.Boots:
                        armorItem.name = "Randomized Boots";
                        armorItem.sprite = bootsSprites[Random.Range(0, bootsSprites.Length)];
                        break;
                    case EquipmentSlot.Cape:
                        armorItem.name = "Randomized Cape";
                        armorItem.sprite = capeSprites[Random.Range(0, capeSprites.Length)];
                        break;
                }

                return armorItem;
        }

        // If no appropriate item was created, then return null
        return null;

        // Then decide the core stats of gear based on gear level

        // Finally randomize # of sub stats
    }
}

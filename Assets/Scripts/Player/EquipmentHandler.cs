using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(EnchantableEntity))]
[RequireComponent(typeof(Stamina))]
public class EquipmentHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CombatStats stats;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Stamina stamina;
    [SerializeField] private EnchantableEntity enchantableEntity;
    [SerializeField] private CombatHandler combatHandler;

    [Header("Weapon Information")]
    [SerializeField] public WeaponItem[] equippedWeaponItem;
    public int[] equippedWeaponIndexes; // Set to -1 if nothing is equipped 

    [Header("Armor Information")]
    [SerializeField] public ArmorItem[] equippedArmor; // Holds equipment type items
    public int[] equippedArmorIndexes; // Holds the indexes of the equipped items in respect to the position in the player's inventory
    // ^^^ Set to -1 if nothing is equipped at that slot

    private void Start()
    {
        // Get references
        stats = GetComponent<CombatStats>();
        stamina = GetComponent<Stamina>();
        //inventoryUI = GetComponentInChildren<InventoryUI>();
        enchantableEntity = GetComponent<EnchantableEntity>();
        combatHandler = GetComponent<CombatHandler>();

        // Intialize weapon slots
        equippedWeaponItem = new WeaponItem[2];
        equippedWeaponIndexes = new int[2];
        for (int i = 0; i < equippedWeaponIndexes.Length; i++) {
            equippedWeaponIndexes[i] = -1;
        }

        // Intialize the amount of armor slots
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        equippedArmor = new ArmorItem[numSlots];
        equippedArmorIndexes = new int[numSlots]; // Set to -1 if nothing is equipped at that slot
        for (int i = 0; i < equippedArmorIndexes.Length; i++) {
            equippedArmorIndexes[i] = -1;
        }
    }

    public void equipWeapon(WeaponItem weaponItem, bool onMainHand) // Always equip selected item tho
    {   
        // Equip weapon
        inventoryUI.equipSelectedItem(true); // Set slot in inventory to equipped state

        // Instaniate the the weapon and set as child
        GameObject equippedWeaponPrefab = Instantiate(weaponItem.prefab, transform.position, Quaternion.identity);
        equippedWeaponPrefab.transform.parent = gameObject.transform; // Sets the prefab to a child of 
        // Cache the equipped weapon
        var equippedWeapon = equippedWeaponPrefab.GetComponent<Weapon>();

        // Set the owner of the weapon to the weaponitem
        equippedWeapon.setOwner(weaponItem);

        // Equip on correct hand
        if (onMainHand) {
            // Store reference to weapon item
            equippedWeaponItem[0] = weaponItem;
            // Set new weapon indexes
            equippedWeaponIndexes[0] = inventoryUI.getSelectedItemIndex();
            // Set mainhand reference
            combatHandler.setMainHandWeapon(equippedWeapon);
        }
        else {
            // Store reference to weapon item
            equippedWeaponItem[1] = weaponItem;
            // Set new weapon indexes
            equippedWeaponIndexes[1] = inventoryUI.getSelectedItemIndex();
            // Set offhand reference
            combatHandler.setOffHandWeapon(equippedWeapon);

            equippedWeaponPrefab.transform.position -= new Vector3(0.25f, 0, 0);
        }

        // If the weapon is enchantable, then add the enchantment
        if (equippedWeapon.TryGetComponent(out EnchantableEntity enchantableEntity)) {
            print("added weapn enchat");
            enchantableEntity.addEnchantment(weaponItem.enchantment);
        }
    }

    // Assumes that player is always unequipping his weapon even if its not a equipped weapon index
    public void unEquipWeapon(int index, bool onMainHand) // Should unequip at index rather than selected item
    {
        // Unequip weapon at index
        inventoryUI.equipItemAtIndex(index, false);

        if (onMainHand) {
            // Set mainhand weapon item values to null
            equippedWeaponItem[0] = null;
            equippedWeaponIndexes[0] = -1;

            // Destroy weapon gameobject
            Destroy(combatHandler.getMainHandWeapon().gameObject);

            // Set mainhand to null
            combatHandler.setMainHandWeapon(null);
        }
        else {
            // Set offhand weapon item values to null
            equippedWeaponItem[1] = null;
            equippedWeaponIndexes[1] = -1;

            // Destroy weapon gameobject
            Destroy(combatHandler.getOffHandWeapon().gameObject);

            // Set offhand to null
            combatHandler.setOffHandWeapon(null);
        }
    }

    public void equipArmor(ArmorItem newArmor)
    {
        int slotIndex = (int)newArmor.equipSlot; // Gets equipment index slot by casting the enum to its numerical value
        equippedArmor[slotIndex] = newArmor;
        equippedArmorIndexes[slotIndex] = inventoryUI.getSelectedItemIndex(); // Gets the location of the equipped armor piece
        inventoryUI.equipSelectedItem(true); // Equip item

        // Add new armor's stats to player
        stats.defense += newArmor.defenseValue;
        stats.bonusStamina += newArmor.bonusStamina;

        stamina.maxStamina += newArmor.bonusStamina;

        // Add the armor's enchantment to the player
        enchantableEntity.addEnchantment(newArmor.enchantment);
    }

    public void unEquipArmor(int slot)
    {
        // Remove armor stats
        stats.defense -= equippedArmor[slot].defenseValue;
        stats.bonusStamina -= equippedArmor[slot].bonusStamina;

        // Remove max stamina effects
        stamina.maxStamina -= equippedArmor[slot].bonusStamina;
        if (stamina.currentStamina > stamina.maxStamina)
            stamina.currentStamina = stamina.maxStamina;

        // Remove the armor's enchantment
        enchantableEntity.removeEnchantment(equippedArmor[slot].enchantment);

        // Remove armor item from equip slot
        inventoryUI.equipSelectedItem(false);
        equippedArmor[slot] = null;
        equippedArmorIndexes[slot] = -1;
    }

    public WeaponItem getMainHandWeaponItem() {
        return equippedWeaponItem[0];
    }

    public WeaponItem getOffHandWeaponItem() {
        return equippedWeaponItem[1];
    }
}

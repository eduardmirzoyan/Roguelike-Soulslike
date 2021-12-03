using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(Stamina))]
public class EquipmentHandler : MonoBehaviour
{
    [SerializeField] private CombatStats stats;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Stamina stamina;

    [Header("Weapon Information")]
    [SerializeField] public WeaponItem equippedWeaponItem;
    [SerializeField] public Weapon weapon;
    private GameObject equippedWeaponPrefab;
    public int equippedWeaponIndex; // Set to -1 if nothing is equipped 

    [Header("Armor Information")]
    [SerializeField] public ArmorItem[] equippedArmor; // Holds equipment type items
    public int[] equippedArmorIndexes; // Holds the indexes of the equipped items in respect to the position in the player's inventory
    // ^^^ Set to -1 if nothing is equipped at that slot


    private void Start()
    {
        // Get references
        stats = GetComponent<CombatStats>();
        stamina = GetComponent<Stamina>();
        inventory = GetComponentInChildren<Inventory>();

        // Intialize the amount of armor slots
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        equippedArmor = new ArmorItem[numSlots];
        equippedArmorIndexes = new int[numSlots]; // Set to -1 if nothing is equipped at that slot
    }

    public void equipWeapon(WeaponItem newWeapon) // Always equip selected item tho
    {   // Attempt to equip weapon
        inventory.equipSelectedItem(); // Set slot in inventory to equipped state
        equippedWeaponItem = newWeapon;

        // Set new weapon indexes
        equippedWeaponIndex = inventory.getSelectedItemIndex();

        // Creates the prefab for the weapon
        equippedWeaponPrefab = Instantiate(newWeapon.prefab, transform.position, Quaternion.identity);
        equippedWeaponPrefab.transform.parent = gameObject.transform; // Sets the prefab to a child of 

        // Get reference to equipped weapon
        weapon = GetComponentInChildren<Weapon>();

        // Send event trigger
        GameEvents.current.triggerPlayerEquippedWeapon(equippedWeaponPrefab.GetComponent<Weapon>());
    }

    // Assumes that player is always unequipping his weapon even if its not a equipped weapon index
    public void unEquipWeapon(int index) // Should unequip at index rather than selected item
    {
        inventory.unEquipItemAtIndex(index);

        equippedWeaponItem = null;
        equippedWeaponIndex = -1;

        // Remove reference
        weapon = null;

        Destroy(GameObject.Find(equippedWeaponPrefab.name));
        equippedWeaponPrefab = null;

        // Send event trigger
        GameEvents.current.triggerPlayerEquippedWeapon(null);
    }

    public void equipArmor(ArmorItem newArmor)
    {
        int slotIndex = (int)newArmor.equipSlot; // Gets equipment index slot by casting the enum to its numerical value
        equippedArmor[slotIndex] = newArmor;
        equippedArmorIndexes[slotIndex] = inventory.getSelectedItemIndex(); // Gets the location of the equipped armor piece
        inventory.equipSelectedItem();

        // Add new armor's stats to player

        stats.defense += newArmor.defenseValue;
        stats.bonusStamina += newArmor.bonusStamina;
        stats.damageTakenMultiplier += newArmor.damageTakenMultiplier;

        stamina.maxStamina += newArmor.bonusStamina;

        //poiseThreshold += newArmor.defenseValue;
    }

    public void unEquipArmor(int slot)
    {
        // Remove armor stats
        stats.defense -= equippedArmor[slot].defenseValue;
        stats.bonusStamina -= equippedArmor[slot].bonusStamina;
        stats.damageTakenMultiplier -= equippedArmor[slot].damageTakenMultiplier;

        stamina.maxStamina -= equippedArmor[slot].bonusStamina;
        if (stamina.currentStamina > stamina.maxStamina)
            stamina.currentStamina = stamina.maxStamina;

        // Remove armor item from equip slot
        inventory.unEquipSelectedItem();
        equippedArmor[slot] = null;
        equippedArmorIndexes[slot] = -1;
    }

    public WeaponItem getEquippedWeaponItem()
    {
        return equippedWeaponItem;
    }
}

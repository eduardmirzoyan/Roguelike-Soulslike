using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private SkillTreeUI skillTreeUI;

    [Header("Fields")]
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemType;
    [SerializeField] private Text itemModifer;
    [SerializeField] private Text itemDescription;
    [SerializeField] private Item selectedItem;
    [SerializeField] private Menu menu;

    [SerializeField] private Skill selectedSkill;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI = GameObject.Find("Slot Holder").GetComponent<InventoryUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (menu.window)
        {
            case MenuWindow.Inventory:
                // First check if player is hovering over an actual item (and not empty slot)
                selectedItem = inventoryUI.getSelectedItem();
                // If so...
                if (selectedItem != null) 
                {
                    transform.position = inventoryUI.getSelectedSlotPosition().position; // Find the slot that the player is hovering
                    itemName.text = selectedItem.name; // Set tooltip values to the items values
                    switch (selectedItem.type)
                    {
                        case ItemType.Armor:
                            // Cache armor
                            var armorItem = (ArmorItem)selectedItem;

                            // Set item type
                            itemType.text = armorItem.armorType.ToString() + " " + armorItem.equipSlot.ToString();

                            // Set modifier
                            if (armorItem.enchantment != null) {
                                itemModifer.text = armorItem.enchantment.enchantmentName;
                            }
                            else {
                                itemModifer.text = "";
                            }

                            // Set description
                            itemDescription.text = 
                                "Defense: " + armorItem.defenseValue
                                + "\nBonus Stamina: " + armorItem.bonusStamina
                                + "\nDescription: " + armorItem.description;
                            break;
                        case ItemType.Weapon:
                            // Cache weapon
                            var weaponItem = (WeaponItem)selectedItem;

                            // Set item type
                            itemType.text = weaponItem.weaponType.ToString() + " " + (weaponItem.twoHanded ? "(2H)" : "(1H)");

                            // Set item modifier
                            if (weaponItem.enchantment != null) {
                                itemModifer.text = weaponItem.enchantment.enchantmentName;
                            }
                            else {
                                itemModifer.text = "";
                            }

                            // Set description
                            itemDescription.text = 
                                 "\nDamage: " + weaponItem.damage
                                + "\nCrit Chance: " + weaponItem.critChance * 100 + "%"
                                + "\nDescription: " + weaponItem.description;
                            break;
                        default:
                            // Set item type to name and modifier to null
                            itemType.text = selectedItem.type.ToString();
                            itemModifer.text = "";
                            // Set item text and description
                            itemType.text = selectedItem.name;
                            itemDescription.text = selectedItem.description;
                            break;
                    }
                }
                else // If not, send the tooltip to Narnia
                {
                    transform.position = new Vector3(1000, 1000, 1000);
                }
                break;
            case MenuWindow.Status:
                transform.position = new Vector3(1000, 1000, 1000);
                break;
            case MenuWindow.SkillTree:
                selectedSkill = skillTreeUI.getSelectedSkill();
                if (selectedSkill != null) // If so...
                {
                    transform.position = skillTreeUI.getSelectedSlotPosition().position; // Find the slot that the player is hovering
                    itemName.text = selectedSkill.name; // Set tooltip values to the items values
                    itemType.text = selectedSkill is ActiveSkill ? "Active Skill" : "Passive Skill";
                    itemDescription.text = selectedSkill.description;
                }
                else // If not, send the tooltip to Narnia
                {
                    transform.position = new Vector3(1000, 1000, 1000);
                }
                break;
        }
        

    }
}

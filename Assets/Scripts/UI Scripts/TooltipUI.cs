using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private SkillTreeUI skillTreeUI;

    [SerializeField] private Text itemName;
    [SerializeField] private Text itemType;
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
                            itemType.text = ((ArmorItem)selectedItem).armorType.ToString() + " " + ((ArmorItem)selectedItem).equipSlot.ToString();
                            itemDescription.text = "Defense: " + ((ArmorItem)selectedItem).defenseValue
                                + "\nBonus Stamina: " + ((ArmorItem)selectedItem).bonusStamina
                                + "\nDescription: " + selectedItem.description;
                            break;
                        case ItemType.Weapon:
                            itemType.text = ((WeaponItem)selectedItem).weaponType.ToString();
                            itemDescription.text = "Light Damage: " + ((WeaponItem)selectedItem).lightDamage
                                + "\nHeavy Damage: " + ((WeaponItem)selectedItem).heavyDamage
                                + "\nDescription: " + selectedItem.description;
                            break;
                        case ItemType.Consumable:
                            itemType.text = ((ConsumableItem)selectedItem).name;
                            itemDescription.text = selectedItem.description;
                            break;
                    }
                }
                else // If not, send the tooltip to Narnia
                {
                    transform.position = new Vector3(1000, 1000, 10);
                }
                break;
            case MenuWindow.Status:
                transform.position = new Vector3(1000, 1000, 10);
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
                    transform.position = new Vector3(1000, 1000, 10);
                }
                break;
        }
        

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuWindow
{
    Inventory,
    Status,
    SkillTree
}
public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public MenuWindow window;
    [SerializeField] private Player player;
    [SerializeField] private EquipmentHandler playerEquipment;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private SkillTreeUI skillTreeUI;
    [SerializeField] private MenuUI menuUI;
    [SerializeField] private HUD hud;
    [SerializeField] protected WorldItem dropLoot; // REPLACE THIS WITH 'RESOURCE LOADING'



    [Header("Menu Windows")]
    [SerializeField] private GameObject inventoryScreen;
    [SerializeField] private GameObject statusScreen;
    [SerializeField] private GameObject skillTreeScreen;

    [SerializeField] private GameObject tooltip;

    [SerializeField] public bool menuEnabled;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        playerEquipment = player.GetComponent<EquipmentHandler>();
        skillTreeUI = GameObject.Find("Skill Tree UI").GetComponent<SkillTreeUI>();
        menuUI = GetComponent<MenuUI>();
        inventoryUI = GameObject.Find("Slot Holder").GetComponent<InventoryUI>();
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        tooltip = GameObject.Find("Tooltip Holder");
        inventoryScreen.SetActive(true);
        skillTreeScreen.SetActive(false);
        statusScreen.SetActive(false);
        tooltip.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (menuEnabled)
        {
            menuUI.show();
            hud.show();

            // Changing menu screens logic
            if (Input.GetKeyDown(KeyCode.A))
            {
                changeWindowScreen(-1);
            } 
            else if (Input.GetKeyDown(KeyCode.D))
            {
                changeWindowScreen(1);
            }

            switch (window) 
            {
                case MenuWindow.Inventory:
                    inventoryScreenLogic();
                    break;
                case MenuWindow.Status:
                    statusScreenLogic();
                    break;
                case MenuWindow.SkillTree:
                    skillTreeScreenLogic();
                    break;

            }
        }
        else
        {
            menuUI.hide();
            hud.hide();
        }
    }



    private void inventoryScreenLogic()
    {
        #region Menu logic

        // Inventory selector maneuvering
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            inventoryUI.moveSelectedItem("left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            inventoryUI.moveSelectedItem("right");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            inventoryUI.moveSelectedItem("up");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            inventoryUI.moveSelectedItem("down");
        }

        // Using selected item in inventory
        if (Input.GetKeyDown(KeyCode.E))
        {
            Item item = inventoryUI.getSelectedItem();

            // If item at slot is not empty, use the item
            if (item != null)
            {
                attemptToUseSelectedItem(item, inventoryUI.getSelectedItemIndex());
            }
            else 
            {
                print("no item to use!");
            }
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            Item selectedItem = inventoryUI.getSelectedItem();
            // If the selected item is null or equipped, then don't do anything
            if (selectedItem == null || inventoryUI.isSelectedItemEquipped())
            {
                return; // Do nothing if the selected spot is empty
            }
            
            // Drop selected item
            var prefab = Instantiate(dropLoot, player.transform.position, Quaternion.identity);
            prefab.setItem(selectedItem);

            inventoryUI.deleteSelectedItem();
        }
        inventoryScreen.SetActive(true);

        #endregion
    }

    private void statusScreenLogic()
    {
        // Nothing yet
    }

    private void skillTreeScreenLogic()
    {
        skillTreeUI.updateSkillVisuals();

        // Skill Tree maneuvering
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            skillTreeUI.moveSelectedItem("left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            skillTreeUI.moveSelectedItem("right");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            skillTreeUI.moveSelectedItem("up");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            skillTreeUI.moveSelectedItem("down");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            skillTreeUI.attemptToUnlockSkillAtSelectedSlot();
        }

    }

    private void changeWindowScreen(int direction)
    {
        // Change window enum
        if(window + direction < MenuWindow.Inventory)
        {
            window = MenuWindow.SkillTree;
        }
        else if(window + direction > MenuWindow.SkillTree)
        {
            window = MenuWindow.Inventory;
        }   
        else
        {
            window += direction;
        }
        
        // Set respective window active
        switch(window)
        {
            case MenuWindow.Inventory:
                inventoryScreen.SetActive(true);
                statusScreen.SetActive(false);
                skillTreeScreen.SetActive(false);
                break;
            case MenuWindow.Status:
                inventoryScreen.SetActive(false);
                statusScreen.SetActive(true);
                skillTreeScreen.SetActive(false);
                break;
            case MenuWindow.SkillTree:
                inventoryScreen.SetActive(false);
                statusScreen.SetActive(false);
                skillTreeScreen.SetActive(true);
                break;
        }
    }

    public void attemptToUseSelectedItem(Item item, int itemIndex)
    {
        switch (item.type) // If the chosen item is an equipable, then attempt to equip it, if it is a useable then use it
        {
            case ItemType.Weapon:
                // Check if item already equipped in main hand
                if (itemIndex == playerEquipment.equippedWeaponIndexes[0]) { // If this item is already equipped in mainhand
                    // Unequip from mainhand
                    playerEquipment.unEquipWeapon(itemIndex, true);
                    
                    // Attempt to equip to offhand
                    if (playerEquipment.getOffHandWeaponItem() == null) {
                        playerEquipment.equipWeapon((WeaponItem)item, false);
                    }
                    
                    // Finish
                    return;
                }

                // Check if item already equipped in main hand
                if (itemIndex == playerEquipment.equippedWeaponIndexes[1]) { // If this item is already equipped in offhand
                    // Unequip from offhand
                    playerEquipment.unEquipWeapon(itemIndex, false);
                    
                    // Finish
                    return;
                }

                // Now we can try to equip as a new item

                // If main-hand is free
                if (playerEquipment.getMainHandWeaponItem() == null) {
                    // If weapon item is one-handed
                    if (!((WeaponItem)item).twoHanded) {
                        // if off-hand is open || off-hand is 1handed
                        if (playerEquipment.getOffHandWeaponItem() == null || !playerEquipment.getOffHandWeaponItem().twoHanded) {
                            // Equip normally
                            playerEquipment.equipWeapon((WeaponItem)item, true);
                            return;
                        }
                    }
                    // Else if weapon is 2-handed
                    else {
                        // If there is no offhand weapon
                        if (playerEquipment.getOffHandWeaponItem() == null) {
                            // Equip normally
                            playerEquipment.equipWeapon((WeaponItem)item, true);
                            return;
                        }
                    }
                }
                
                // Now we check if off-hand is free
                if (playerEquipment.getOffHandWeaponItem() == null) {
                    // If weapon item is one-handed
                    if (!((WeaponItem)item).twoHanded) {
                        // if main-hand is open || main-hand is 1handed
                        if (playerEquipment.getMainHandWeaponItem() == null || !playerEquipment.getMainHandWeaponItem().twoHanded) {
                            // Equip normally
                            playerEquipment.equipWeapon((WeaponItem)item, false);
                            return;
                        }
                    }
                    // Else if weapon is 2-handed
                    else {
                        // If there is no mainhand weapon
                        if (playerEquipment.getMainHandWeaponItem() == null) {
                            // Equip normally
                            playerEquipment.equipWeapon((WeaponItem)item, true);
                            return;
                        }
                    }
                }
                // Don't do shit now

                // If main slot is open
                    // if one-handed
                        // if off-hand is open || off-hand is 1handed
                            // equip normally
                        // else 
                            // don't equip
                    // if two handed
                        // if off-hand is open
                            // equip normally
                        // else 
                            // don't equip
                
                // else check offhand is open
                    // if one-handed
                        // if main-hand is open || main-hand is 1handed
                            // equip normally
                        // else 
                            // don't equip
                    // if two handed
                        // if main-hand is open
                            // equip normally
                        // else 
                            // don't equip
                break;
            case ItemType.Armor:
                // Equipping armor logic
                int slotIndex = (int)((ArmorItem)item).equipSlot;

                if (playerEquipment.equippedArmor[slotIndex] != null)
                {
                    // If the selected armor is the same one that is already equipped, then unequip that armor
                    if (itemIndex == playerEquipment.equippedArmorIndexes[slotIndex])
                    {
                        playerEquipment.unEquipArmor(slotIndex);
                    }
                    else
                    {
                        // Do nothing
                    }
                }
                else
                {
                    playerEquipment.equipArmor((ArmorItem)item);
                }

                break;
            case ItemType.Consumable:
                // Use item
                var consumable = (ConsumableItem)item;
                if(consumable != null)
                {
                    consumable.consume(player.gameObject);
                    consumable.count -= 1;
                    if(consumable.count <= 0)
                    {
                        // Remove item from inventory
                        inventoryUI.deleteSelectedItem();
                    }
                }

                // TODO:
                // TO BE IMPLEMENTED
                Debug.Log("Consume!");
                break;
        }
    }
}

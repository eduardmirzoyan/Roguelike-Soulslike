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
    [SerializeField] private Inventory inventory;
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
        inventory = player.GetComponentInChildren<Inventory>();
        playerEquipment = player.GetComponent<EquipmentHandler>();
        skillTreeUI = GameObject.Find("Skill Tree UI").GetComponent<SkillTreeUI>();
        menuUI = GetComponent<MenuUI>();
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
            inventory.moveSelectedItem("left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            inventory.moveSelectedItem("right");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            inventory.moveSelectedItem("up");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            inventory.moveSelectedItem("down");
        }

        // Using selected item in inventory
        if (Input.GetKeyDown(KeyCode.E))
        {
            Item item = inventory.getSelectedItem();
            if (item == null)
            {
                return; // Do nothing if the selected spot is empty
            }
            else
                attemptToUseSelectedItem(item, inventory.getSelectedItemIndex());
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            // If the selected item is null or equipped, then don't do anything
            if (inventory.getSelectedItem() == null || inventory.isSelectedItemEquipped())
            {
                return; // Do nothing if the selected spot is empty
            }
            
            
            // Drop selected item
            var prefab = Instantiate(dropLoot, player.transform.position, Quaternion.identity);
            prefab.setItem(inventory.getSelectedItem());

            inventory.deleteSelectedItem();
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
                // Equipping weapon logic
                if (playerEquipment.getEquippedWeaponItem() != null)
                {
                    // If the selected weapon is the same one that is already equipped, then unequip that weapon
                    if (itemIndex == playerEquipment.equippedWeaponIndex)
                    {
                        playerEquipment.unEquipWeapon(playerEquipment.equippedWeaponIndex);
                    }
                    else
                    {
                        // Do nothing
                        playerEquipment.unEquipWeapon(playerEquipment.equippedWeaponIndex);
                        playerEquipment.equipWeapon((WeaponItem)item);
                    }
                }
                else
                {
                    playerEquipment.equipWeapon((WeaponItem)item);

                }
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
                        inventory.deleteSelectedItem();
                    }
                }

                // TO BE IMPLEMENTED
                Debug.Log("Consume!");
                break;
        }
    }
}

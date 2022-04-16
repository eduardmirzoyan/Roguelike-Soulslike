using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] protected Inventory inventory;

    public Transform itemsParent;
    protected InventorySlotUI[] inventorySlots;
    private int selectedItem;


    // Start is called before the first frame update
    private void Start()
    {
        inventorySlots = itemsParent.GetComponentsInChildren<InventorySlotUI>();
        selectedItem = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(i < inventory.getCurrentSize())
            {
                if(inventory.getItem(i) != null) // If the invetory has an item at location i
                {
                    inventorySlots[i].addItem(inventory.getItem(i)); // Add that item to UI
                }
                else
                    inventorySlots[i].ClearSlot();
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }

            // Handle checking slected item
            inventorySlots[i].isSelected = i == selectedItem;
        }
    }

    public void moveSelectedItem(string direction)
    {
        switch (direction) 
        {
            case "left":
                if (selectedItem % 5 == 0)
                    break;
                inventorySlots[selectedItem].isSelected = false;
                selectedItem -= 1;
                inventorySlots[selectedItem].isSelected = true;
                break;
            case "right":
                if ((selectedItem + 1) % 5 == 0)
                    break;
                inventorySlots[selectedItem].isSelected = false;
                selectedItem += 1;
                inventorySlots[selectedItem].isSelected = true;
                break;
            case "up":
                if (selectedItem - 5 < 0)
                    break;
                inventorySlots[selectedItem].isSelected = false;
                selectedItem -= 5;
                inventorySlots[selectedItem].isSelected = true;
                break;
            case "down":
                if (selectedItem + 5 > 24)
                    break;
                inventorySlots[selectedItem].isSelected = false;
                selectedItem += 5;
                inventorySlots[selectedItem].isSelected = true;
                break;
        }
    }

    public Transform getSelectedSlotPosition()
    {
        return inventorySlots[selectedItem].transform;
    }

    public Item getSelectedItem()
    {
        return inventory.getItem(selectedItem);
    }

    public void removeSelectedItem() {
        inventory.removeItemAt(selectedItem);
    }

    public void reduceSelectedItem()
    {
        inventory.reduceItemAt(selectedItem);
    }

    public int getSelectedItemIndex()
    {
        return selectedItem;
    }

    public void equipSelectedItem(bool state) // Toggle
    {
        inventorySlots[selectedItem].isEquipped = state;
    }

    public bool isSelectedItemEquipped() {
        return inventorySlots[selectedItem].isEquipped;
    }

    public void equipItemAtIndex(int index, bool state)
    {
        inventorySlots[index].isEquipped = state;
    }
}

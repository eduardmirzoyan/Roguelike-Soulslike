using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] protected Inventory inventory;

    public Transform itemsParent;
    protected InventorySlotUI[] slots;
    private int selectedItem;

    // Start is called before the first frame update
    private void Start()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlotUI>();
        selectedItem = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        selectedItem = inventory.selectedSlot;
        for(int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.currentSize)
            {
                if(inventory.items[i].getItem() != null) // If the invetory has an item at location i
                {
                    slots[i].addItem(inventory.items[i].getItem()); // Add that item to UI
                    slots[i].isEquipped = inventory.items[i].isEquipped;
                }
                else
                    slots[i].ClearSlot();
            }
            else
            {
                slots[i].ClearSlot();
            }
            slots[i].isSelected = inventory.items[i].isSelected;

            //Check if selected
            if (i == selectedItem)
            {
                slots[i].isSelected = true;
            }
            else
            {
                slots[i].isSelected = false;
            }
        }
    }

    public Transform getSelectedSlotPosition()
    {
        return slots[selectedItem].transform;
    }

    public Item getSelectedItem()
    {
        return inventory.getSelectedItem();
    }
}

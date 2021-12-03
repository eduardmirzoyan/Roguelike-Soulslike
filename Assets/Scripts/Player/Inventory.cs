using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> items;
    [SerializeField] protected int maxSize;
    public int currentSize;
    [SerializeField] public int selectedSlot;

    protected void Start()
    {
        items = new List<InventorySlot>(maxSize);
        for (int i = 0; i < maxSize; i++)
        {
            items.Add(new InventorySlot());
        }
        currentSize = 0;

        selectedSlot = 0;
        items[selectedSlot].isSelected = true;
    }

    public void addItem(Item itemToAdd)
    {
        for (int i = 0; i < maxSize; i++)
        {
            if(itemToAdd.isStackable && !items[i].isEmpty() && items[i].itemName() == itemToAdd.name)
            {
                items[i].increaseCount(itemToAdd.count);
                return;
            }
            if (items[i].isEmpty())
            {
                items[i].addItem(Instantiate(itemToAdd)); // Add a copy of the scriptable object
                currentSize++;
                return;
            }
        }
    }

    public void clearItems()
    {
        items = new List<InventorySlot>(maxSize);
    }

    public void moveSelectedItem(string direction)
    {
        switch (direction) 
        {
            case "left":
                if (selectedSlot % 5 == 0)
                    break;
                items[selectedSlot].isSelected = false;
                selectedSlot -= 1;
                items[selectedSlot].isSelected = true;
                break;
            case "right":
                if ((selectedSlot + 1) % 5 == 0)
                    break;
                items[selectedSlot].isSelected = false;
                selectedSlot += 1;
                items[selectedSlot].isSelected = true;
                break;
            case "up":
                if (selectedSlot - 5 < 0)
                    break;
                items[selectedSlot].isSelected = false;
                selectedSlot -= 5;
                items[selectedSlot].isSelected = true;
                break;
            case "down":
                if (selectedSlot + 5 > 24)
                    break;
                items[selectedSlot].isSelected = false;
                selectedSlot += 5;
                items[selectedSlot].isSelected = true;
                break;
        }
    }

    public void deleteSelectedItem()
    {
        items[selectedSlot].clear();
    }

    public Item getSelectedItem()
    {
        return items[selectedSlot].getItem();
    }

    public int getSelectedItemIndex()
    {
        return selectedSlot;
    }

    public void equipSelectedItem() // Toggle
    {
        items[selectedSlot].isEquipped = true; // Toggles the state of item being equipped
    }

    public void unEquipSelectedItem()
    {
        items[selectedSlot].isEquipped = false;
    }

    public void unEquipItemAtIndex(int index2)
    {
        items[index2].isEquipped = false;
    }

    public bool isSelectedItemEquipped()
    {
        return items[selectedSlot].isEquipped;
    }
}

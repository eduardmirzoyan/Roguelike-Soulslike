using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    //private int count;
    public Item item;
    public bool isSelected;
    public bool isEquipped;

    public InventorySlot()
    {
        item = null;
        isEquipped = false;
        isSelected = false;
    }

    public InventorySlot(Item item)
    {
        this.item = item;
    }

    public void addItem(Item item)
    {
        this.item = item;
        //count = 1;
    }

    public string itemName()
    {
        if (item == null)
            Debug.Log("Item is null");

        return item?.name ?? "";
    }
    
    public bool isEmpty()
    {
        return item == null;
    }

    public void increaseCount(int amount)
    {
        item.count += amount;
    }

    public Item getItem()
    {
        return item;
    }

    public void clear()
    {
        item = null;
        //count = 0;
        isEquipped = false;
    }
}

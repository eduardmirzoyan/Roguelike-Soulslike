using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public List<Item> items;
    [SerializeField] protected int maxSize;

    protected void Start()
    {
        setMax(maxSize);
    }

    public void addItem(Item itemToAdd)
    {
        // See if item exists in inventory and if it is stackable, then increment it
        for (int i = 0; i < maxSize; i++)
        {
            if (items[i] != null && items[i].name == itemToAdd.name && itemToAdd.isStackable) {
                items[i].count += itemToAdd.count;
                return;
            }
        }

        for (int i = 0; i < maxSize; i++)
        {
            if (items[i] == null) {
                // Add copy
                items[i] = itemToAdd;
                return;
            }
        }

        print("Inventory is full!");

    }

    public void removeItem(ItemType itemType) {
        foreach (var item in items) {
            if (item != null &&  item.type == itemType) {
                item.count--;
                if (item.count < 1) {
                    items.Remove(item);
                }
                return;
            }
        }
    }

    public Item getItemOfType(ItemType itemType) {
        foreach (var item in items) {
            if (item != null && item.type == itemType) {
                return item;
            }
        }
        return null;
    }

    public void setMax(int newMax) {
        maxSize = newMax;
        items = new List<Item>(newMax);
        for (int i = 0; i < maxSize; i++)
        {
            items.Add(null);
        }
    }

    public void deleteItem(int index) {
        items.RemoveAt(index);
    }

    public Item getItem(int index) {
        return items[index];
    }

    public int getCurrentSize() {
        return items.Count;
    }

    public bool isFull() {
        return items.All<Item>(item => (item != null));
    }

    public bool isEmpty() {
        return items.All<Item>(item => (item == null));
    }
}

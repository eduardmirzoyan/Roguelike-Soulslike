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
        // If invetory is full, then send warning and do nothing
        // if (items.Count >= maxSize) {
        //     print("Inventory at max size!");
        //     return;
        // }

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
                print("added");
                items[i] = itemToAdd;
                return;
            }
        }

        // Else...
        print("Inventory at max size!");

        // else, add the item as a new item at the end of the invetory
        //items.Add(Instantiate(itemToAdd));
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

    public void clearItems()
    {
        items = new List<Item>(maxSize);
    }

    public int getCurrentSize() {
        return items.Count;
    }

    public int getMaxSize() {
        return maxSize;
    }

    public bool isFull() {
        return items.All<Item>(item => (item != null));
    }

    public bool isEmpty() {
        return items.All<Item>(item => (item == null));
    }
}

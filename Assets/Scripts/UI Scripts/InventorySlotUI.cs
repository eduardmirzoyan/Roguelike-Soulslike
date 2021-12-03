using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image image;
    public Image selectBorder;
    public Image equipBorder;
    public Item item;
    public Text itemCount;
    public bool isSelected;
    public bool isEquipped;

    private void Update()
    {
        selectBorder.enabled = isSelected; // Enable border if item is selected

 /*       if (isSelected)
        {
            selectBorder.enabled = true;
        }
        else
        {
            selectBorder.enabled = false;
        }*/

        equipBorder.enabled = isEquipped; // Enable border if item is equipped

        /*        if (isEquipped)
                {
                    equipBorder.enabled = true;
                }
                else
                {
                    equipBorder.enabled = false;
                }*/

        if(item != null)
        {
            if (item.isStackable && item.count > 1)
            {
                itemCount.text = "x" + item.count;
            }
            else
                itemCount.text = "";
        }
        else
            itemCount.text = "";
    }

    public void addItem(Item item)
    {
        this.item = item;
        image.sprite = item.sprite;
        image.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        image.sprite = null;
        image.enabled = false;
        isEquipped = false;
    }
}

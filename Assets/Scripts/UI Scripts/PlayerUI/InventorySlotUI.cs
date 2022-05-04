using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image container;
    public Image itemImage;
    public Image selectBorder;
    public Image equipBorder;
    public Text equipIcon;
    public Item item;
    public Text itemCount;
    public bool isSelected;
    public bool isEquipped;

    private void Update()
    {
        selectBorder.enabled = isSelected; // Enable border if item is selected

        equipIcon.enabled = isEquipped;

        // Change color
        var color = container.color;
        if (isEquipped) {
            color = Color.green;
            color.a = 0.2f;
        }
        else {
            color = Color.white;
            color.a = 0.4f;
        }
        container.color = color;

        // equipBorder.enabled = isEquipped; // Enable border if item is equipped

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
        itemImage.sprite = item.sprite;
        itemImage.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.enabled = false;
        isEquipped = false;
    }
}

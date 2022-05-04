using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private Sprite emptyChest;
    [SerializeField] public int moneyAmount;
    [SerializeField] private WorldItem itemHolder;

    [SerializeField] private bool isColleced;

    [SerializeField] private bool infiniteUse;

    public void open()
    {
        if (!isColleced || infiniteUse)
        {
            // Generate random drop
            Item itemDrop = LootManager.instance.getRandomItem();
            
            // Create a holder for the loot drop
            itemHolder.setItem(itemDrop);
            Instantiate(itemHolder, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);

            // Create popup
            PopUpTextManager.instance.createWeakPopup("+" + moneyAmount + " gold", Color.yellow, transform.position, 0.5f);

            GameManager.instance.addGold(moneyAmount);

            // Change sprite
            GetComponent<SpriteRenderer>().sprite = emptyChest;

            // Set the state to collected
            isColleced = true;
        }
        else
        {
            PopUpTextManager.instance.createVerticalPopup("The chest is empty.", Color.gray, transform.position);
        }
    }
}

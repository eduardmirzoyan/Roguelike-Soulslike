using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private Sprite emptyChest;
    [SerializeField] public int moneyMount;
    [SerializeField] private WorldItem itemHolder;

    [SerializeField] private bool isColleced;

    [SerializeField] private bool infiniteUse;

    public void open()
    {
        if (!isColleced || infiniteUse)
        {
            GetComponent<SpriteRenderer>().sprite = emptyChest;

            GameManager.instance.CreatePopup("You opened the chest.", transform.position);
            //Item itemDrop = ItemSpawnManager.instance.randomizeItem();
            Item itemDrop = LootManager.instance.getRandomItem();

            itemHolder.setItem(itemDrop);
            Instantiate(itemHolder, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);

            // Set the state to collected
            isColleced = true;
        }
        else
        {
            GameManager.instance.CreatePopup("Chest is empty.", transform.position);
        }
    }
}

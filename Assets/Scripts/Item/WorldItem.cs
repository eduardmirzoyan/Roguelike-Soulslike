using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private SpriteRenderer spriteRend;

    private void Start()
    {
        setItem(item);
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        spriteRend.sprite = item.sprite;
        if (item.type == ItemType.Weapon)
            spriteRend.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
        else 
            spriteRend.transform.rotation = Quaternion.identity;   
        
    }

    public Item GetItem()
    {
        return item;
    }

    public void setItem(Item newItem)
    {
        item = newItem;         
    }
}

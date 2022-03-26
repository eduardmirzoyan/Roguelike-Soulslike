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
        spriteRend = GetComponent<SpriteRenderer>();
        setItem(item);
    }

    public Item GetItem()
    {
        return item;
    }

    public void setItem(Item newItem)
    {
        this.item = newItem;
        spriteRend.sprite = item.sprite;
        transform.localScale = new Vector3(.15f, .15f, 0);

        if (item.type == ItemType.Weapon)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
        else 
            transform.rotation = Quaternion.identity;            
    }
}

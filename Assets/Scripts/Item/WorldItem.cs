using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private SpriteRenderer spriteRend;
    [SerializeField] private Collider2D collider2d;

    private void Start()
    {
        collider2d = GetComponent<Collider2D>();

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
        item = Instantiate(newItem);
        item.count = newItem.count;    
    }

    public void enableCollider(bool state) {
        collider2d.enabled = state;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ammo")]
public class AmmoItem : Item
{
    public void Awake()
    {
        type = ItemType.Ammo;
        isStackable = true;
    }
}

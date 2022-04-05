using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AmmoItem : Item
{
    public void Awake()
    {
        type = ItemType.Ammo;
        isStackable = true;
        count = 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : Item
{
    public void Awake()
    {
        type = ItemType.Consumable;
        count = 1;
    }

    public abstract void consume(GameObject consumer);
}

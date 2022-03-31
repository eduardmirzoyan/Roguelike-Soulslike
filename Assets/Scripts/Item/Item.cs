using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Consumable,
    Weapon,
    Armor,
    Default
}

public abstract class Item : ScriptableObject
{
    public GameObject prefab;
    public ItemType type;
    public Sprite sprite;
    public bool isStackable;
    public int count;

    [TextArea(15, 20)]
    public string description;
    public new string name = "Name";
}

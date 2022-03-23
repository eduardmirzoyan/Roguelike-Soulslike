using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DefaultItem : Item
{
    public void Awake()
    {
        type = ItemType.Default;
    }
}

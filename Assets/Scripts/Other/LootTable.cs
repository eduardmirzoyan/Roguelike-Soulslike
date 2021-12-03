using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu]
public class LootTable : ScriptableObject
{
 
    [Serializable]
    public class Drop
    {
        public Item item;
        public int weight;
    }
    public List<Drop> table;

    public Item getDrop()
    {
        int totalWeight = table.Sum(item => item.weight);
        int roll = UnityEngine.Random.Range(0, totalWeight);

        foreach(Drop drop in table)
        {
            if (drop.weight > roll)
                return drop.item;
            roll -= drop.weight;
        }

        return table[roll].item;
    }
}

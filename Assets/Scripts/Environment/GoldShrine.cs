using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldShrine : Shrine
{
    [SerializeField] private int storedGold;
    [SerializeField] private bool used;

    private void Start()
    {
        storedGold = 0;
        used = false;
    }

    public void addGold(int amount) 
    {
        storedGold += amount;
        used = false;
    }

    public override void activate(Player player)
    {
        if (!used) 
        {  
            // Take stored gold
            GameManager.instance.CreatePopup("+ " + storedGold + " GOLD", transform.position, Color.yellow);
            GameManager.instance.addGold(storedGold);
            storedGold = 0;
            used = true;
        }
        
    }
}

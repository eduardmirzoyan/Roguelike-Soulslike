using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealingShrine : Shrine
{
    [SerializeField] private bool used;
    public override void activate(Player player)
    {
        if (!used) {
            var flask = player.GetComponentInChildren<Flask>();
            if (flask != null) {
                flask.refill();
                used = true;
            }
        }
        else {
            GameManager.instance.CreatePopup("The fountain runs empty...", transform.position);
        }
        
    } 
}

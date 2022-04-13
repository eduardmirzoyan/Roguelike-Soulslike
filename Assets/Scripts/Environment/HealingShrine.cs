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
            if (flask != null && !flask.isFull()) {
                // Refill flask
                flask.refill();

                // Create popup
                PopUpTextManager.instance.createPopup("Your flask has been refilled.", Color.white, transform.position);

                // Prevent re-usage
                used = true;
            }
            else {
                PopUpTextManager.instance.createPopup("Your flask is already full...", Color.gray, transform.position);
            }
        }
        else {
            PopUpTextManager.instance.createPopup("The fountain runs empty...", Color.gray, transform.position);
        }
        
    } 
}

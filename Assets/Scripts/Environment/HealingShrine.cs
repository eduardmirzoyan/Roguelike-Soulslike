using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealingShrine : Shrine
{
    [SerializeField] private string activateAnimation = "Activate";
    [SerializeField] private bool used;
    public override void activatePress(Player player)
    {
        if (!used) {
            var flask = player.GetComponentInChildren<Flask>();
            if (flask != null && !flask.isFull()) {
                // Play animation
                GetComponent<Animator>().Play(activateAnimation);
                
                // Refill flask
                flask.refill();

                // Create popup
                PopUpTextManager.instance.createVerticalPopup("Your flask has been refilled.", Color.white, transform.position);

                // Prevent re-usage
                used = true;
            }
            else {
                PopUpTextManager.instance.createVerticalPopup("Your flask is already full...", Color.gray, transform.position);
            }
        }
        else {
            PopUpTextManager.instance.createVerticalPopup("The fountain runs empty...", Color.gray, transform.position);
        }
        
    } 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseShrine : Shrine
{
    [SerializeField] private string message = "RIP";

    public override void activate(Player player)
    {
        // Print text
        PopUpTextManager.instance.createPopup(message, Color.green, transform.position);
    }
}

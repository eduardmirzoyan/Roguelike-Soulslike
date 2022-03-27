using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseShrine : Shrine
{
    public override void activate(Player player)
    {
        // Does nothing
        GameManager.instance.CreatePopup("Does nothing", transform.position, Color.green);
    }
}

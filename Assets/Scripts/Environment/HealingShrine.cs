using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealingShrine : Shrine
{
    public override void activate(Player player)
    {
        if (player.TryGetComponent(out Flask flask)) {
            flask.refill();
        }
    } 
}

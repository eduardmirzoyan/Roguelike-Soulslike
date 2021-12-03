using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealingShrine : Shrine
{

    public override void activate()
    {
        collider.checkCollisions(refillPlayerFlask);
    }   

    private void refillPlayerFlask(Collider2D coll)
    {
        var playerFlask = coll.GetComponentInChildren<Flask>();
        if (playerFlask != null)
            playerFlask.refill();
    }
}

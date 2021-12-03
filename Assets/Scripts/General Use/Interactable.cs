using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    private bool hasBeenInteractedWith;

    public void interactWith()
    {
        var chest = GetComponent<Chest>();
        if(!hasBeenInteractedWith && chest != null)
        {
            chest.open();
            hasBeenInteractedWith = true;
        }

        var shrine = GetComponent<Shrine>();
        if(shrine != null)
        {
            shrine.activate();
        }

        var door = GetComponent<Door>();
        if (door != null)
            door.open();
    }
}

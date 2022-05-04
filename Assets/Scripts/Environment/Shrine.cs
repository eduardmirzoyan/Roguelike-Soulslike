using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Shrine : MonoBehaviour
{
    public abstract void activatePress(Player player);

    public virtual void activateRelease(Player player) {
        // Do nothing
    }
}

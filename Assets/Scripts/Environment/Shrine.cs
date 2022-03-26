using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Shrine : MonoBehaviour
{
    public abstract void activate(Player player);
}

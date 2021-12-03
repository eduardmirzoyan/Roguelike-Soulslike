using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{

    protected bool collected; // If this object is collected

    protected virtual void onCollect()
    {
        collected = true;
    }
}

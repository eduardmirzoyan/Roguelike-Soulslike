using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enchantment : ScriptableObject
{
    protected GameObject entity;

    // Basic intialize just takes in the gameobject it is attached to
    public virtual void intialize(GameObject gameObject)
    {
        entity = gameObject;
    }

    public virtual void onTick()
    {
        // Do nothing on tick originally
    }

    // Basic deinitalize just nullifies current attatched gameobject
    public virtual void unintialize()
    {
        entity = null;
    }
}

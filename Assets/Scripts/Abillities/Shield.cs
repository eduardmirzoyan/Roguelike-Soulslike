using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Shield : MonoBehaviour
{
    [SerializeField] public bool isActive { get; protected set; }
    [SerializeField] protected bool isOmidirectional;

    public abstract void blockDamage(Damage dmg);

    public virtual void raiseShield()
    {
        isActive = true;
    }

    public virtual void lowerShield()
    {
        isActive = false;
    }

    public bool checkIfShieldShouldBlock(Vector2 damageOrigin)
    {
        // If shield is blocking omnidirectionally, then you should accept a block from any direction
        if (isOmidirectional)
            return true;

        if (Mathf.Abs(transform.rotation.y) < 0.1f) // if shield is facing right
        {
            if (transform.parent.position.x < damageOrigin.x)
                return true;
            else
                return false;
        }
        else
        {
            if (transform.parent.position.x > damageOrigin.x)
                return true;
            else
                return false;
        }
    }
}

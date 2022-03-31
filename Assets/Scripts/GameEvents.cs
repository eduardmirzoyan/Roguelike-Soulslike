using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }
    
    public event Action<GameObject, GameObject, int> onHit;

    public void triggerOnHit(GameObject attackingEnitiy, GameObject hitEntity, int damageTaken)
    {
        if (onHit != null)
        {
            onHit(attackingEnitiy, hitEntity, damageTaken);
        }
    }
}

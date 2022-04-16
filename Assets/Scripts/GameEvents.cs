using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        instance = this;
    }
    
    public event Action<GameObject, GameObject, int> onHit;
    public event Action<Weapon, bool> onWeaponChange;

    public event Action<TimedEffect, EffectableEntity> onAddStatusEffect;
    public event Action<TimedEffect, EffectableEntity> onRemoveStatusEffect;

    public void triggerOnHit(GameObject attackingEnitiy, GameObject hitEntity, int damageTaken)
    {
        if (onHit != null)
        {
            onHit(attackingEnitiy, hitEntity, damageTaken);
        }
    }

    public void triggerWeaponChange(Weapon weapon, bool onMainHand)
    {
        if (onWeaponChange != null) 
        {
            onWeaponChange(weapon, onMainHand);
        }
    }

    public void triggerAddStatusEffect(TimedEffect timedEffect, EffectableEntity effectableEntity) {
        if (onAddStatusEffect != null) 
        {
            onAddStatusEffect(timedEffect, effectableEntity);
        }
    }
    
    public void triggerRemoveStatusEffect(TimedEffect timedEffect, EffectableEntity effectableEntity) {
        if (onRemoveStatusEffect != null) 
        {
            onRemoveStatusEffect(timedEffect, effectableEntity);
        }
    }
}

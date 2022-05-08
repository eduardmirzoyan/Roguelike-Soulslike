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
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    public event Action<GameObject, GameObject, int> onHit;
    public event Action<Weapon, GameObject> onWeaponHit;
    public event Action<Weapon, bool> onWeaponChange;
    public event Action<Item> onItemPickup;
    public event Action<WorldItem> onItemDrop;
    public event Action<TimedEffect, EffectableEntity> onAddStatusEffect;
    public event Action<TimedEffect, EffectableEntity> onRemoveStatusEffect;
    public event Action<Weapon, Transform> onCrit;
    public event Action onRoll;

    public void triggerOnHit(GameObject attackingEnitiy, GameObject hitEntity, int damageTaken)
    {
        if (onHit != null)
        {
            onHit(attackingEnitiy, hitEntity, damageTaken);
        }
    }

    public void triggerOnWeaponHit(Weapon weapon, GameObject hitEntity) 
    {
        if (onWeaponHit != null)
        {
            onWeaponHit(weapon, hitEntity);
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

    public void triggerOnCrit(Weapon weapon, Transform target) {
        if (onCrit != null) 
        {
            onCrit(weapon, target);
        }
    }

    public void triggerOnItemDrop(WorldItem item) {
        if (onItemDrop != null) 
        {
            onItemDrop(item);
        }
    }

    public void triggerOnItemPickup(Item item) {
        if (onItemPickup != null) 
        {
            onItemPickup(item);
        }
    }

    public void triggerOnRoll() {
        if (onRoll != null) 
        {
            onRoll();
        }
    }
}

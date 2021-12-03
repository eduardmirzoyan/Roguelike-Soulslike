using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class DamageDealer : MonoBehaviour
{
    [SerializeField] private Damage damage;

    public void setDamage(Damage dmg)
    {
        damage = dmg;
        damage.origin = transform.position;
        if(transform.parent != null)
            damage.origin = transform.parent.position; // Set position to the parent's position if possible
    }

    public void dealDamageTo(Damageable entity)
    {
        if(damage.damageAmount > 0)
        {
            entity.takeDamage(damage);
        }
    }   
}

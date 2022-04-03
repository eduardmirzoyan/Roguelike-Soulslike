using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageSource
{
    fromPlayer, 
    fromEnemy,
    fromEnvironment,
    fromSelf
}

[System.Serializable]
public struct Damage 
{
    public int damageAmount;
    public DamageSource source;
    public Transform origin;
    public List<BaseEffect> effects; // List of effects that came with the damage
    public Color color;
    public float pushForce;
}

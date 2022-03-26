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
    public bool isTrue;
    public bool isAvoidable;
    public bool triggersIFrames;
    public List<BaseEffect> effects; // List of effects that came with the damage
    public List<BuildupEffect> buildupEffects; // List of buildup's that came with the damage
    public Color color;
}

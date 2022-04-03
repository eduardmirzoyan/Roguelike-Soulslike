using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStats : MonoBehaviour
{
    [Header("Basic Stats")]
    public int defense;
    public int bonusStamina;
    public int percentCritChance;
    public int percentDodgeChance;

    public float damageDealtMultiplier;
    public float movespeedMultiplier;
    public float damageTakenMultiplier;
}

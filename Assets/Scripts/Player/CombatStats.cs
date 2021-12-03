using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStats : MonoBehaviour
{
    [Header("Visible Stats")]
    public int attackPower;
    public int skillEfficiency;
    public int defense;
    public int bonusStamina;
    public int percentCritChance;
    public int percentDodgeChance;
    public int percentBlockChance;

    public int poiseMeter;
    public int basePoiseThreshold;

    public int percentBleedResistance;
    public int percentFireResistance;
    public int percentPoisonResistance;
    public int percentFrostResistance;
    public int percentCurseResistance;

    public float damageDealtMultiplier;
    public float movespeedMultiplier;
    public float damageTakenMultiplier;
    public List<bool> effects;

    [Header("Invisible Stats")]
    public int percentFumbleChance;

    
}

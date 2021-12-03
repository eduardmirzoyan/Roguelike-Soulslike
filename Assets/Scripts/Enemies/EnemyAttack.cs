using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyAttack : ScriptableObject
{
    [Header("Attack Damage Values")]
    [SerializeField] public Damage damage;

    [Header("Basic Attack Values")]
    [SerializeField] public float attackDelay;
    [SerializeField] public float attackDuration;
    [SerializeField] public float attackRecovery;
    [SerializeField] public string attackName;
    [SerializeField] public int ID;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class EnemyAI : MonoBehaviour
{
    [Header("Enemy Components")]
    [SerializeField] protected Rigidbody2D body;
    [SerializeField] protected Movement mv;
    [SerializeField] protected AnimationHandler animationHandler;
    [SerializeField] protected Collider2D boxCollider2D;
    [SerializeField] protected Health health;
    [SerializeField] protected LineOfSight lineOfSight;
    [SerializeField] protected Displacable displacable;
    [SerializeField] protected CombatStats stats;

    [Header("Wandering Settings")]
    [SerializeField] protected float wanderRadius;
    [SerializeField] protected float wanderRate;

    [Header("Combat Settings")]
    [SerializeField] protected float aggroRange;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackDuration;
    [SerializeField] protected float attackDashSpeed;

    [Header("Death Drops")]
    [SerializeField] protected int xpValue = 100;
    [SerializeField] protected int goldValue = 10;

    [Header("Debugging")]
    [SerializeField] protected Transform target;
    [SerializeField] protected Transform attacker;
    
    // Private fields
    protected float wanderTimer;
    protected float attackCooldownTimer;
    protected float attackTimer;
    protected bool hitStun = true;

    protected virtual void Start()
    {
        // Get components
        body = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<Collider2D>();
        health = GetComponent<Health>();
        lineOfSight = GetComponent<LineOfSight>();
        animationHandler = GetComponent<AnimationHandler>();
        mv = GetComponent<Movement>();
        displacable = GetComponent<Displacable>();
        stats = GetComponent<CombatStats>();

        target = attacker = null;
    }
    public virtual void Die()
    {
        GameManager.instance.addExperience(xpValue);
        GameManager.instance.addGold(goldValue);
    }

    public virtual void isAttacked(Transform transform) {
        attacker = transform;
    }

    public Transform getTarget() {
        return target;
    } 

    protected void faceTarget() {
        mv.setFacingDirection(target.transform.position.x - transform.position.x);
    }
    
    // DISPLACEMENT LOGIC

    public bool isAttacking() {
        return attackTimer > 0;
    }

    protected virtual void resetValues() {
        // Resets any attacking values
        attackTimer = 0;
        attackCooldownTimer = attackCooldown;
        wanderTimer = wanderRate;
    }

    protected virtual void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
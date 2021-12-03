using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(LineOfSight))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyAI : MonoBehaviour
{
    [Header("Enemy Animation")]
    [SerializeField] protected Rigidbody2D body;
    [SerializeField] protected Animator animator;
    [SerializeField] protected AnimationHandler animationHandler;

    [Header("Enemy Stats")]
    [SerializeField] protected int xpValue = 100;
    [SerializeField] protected int goldValue = 10;

    [Header("Pathfinding")]
    [SerializeField] public Player player;
    [SerializeField] protected Transform target;

    [SerializeField] protected LineOfSight lineOfSight;
    [SerializeField] protected float aggroRange;
    [SerializeField] protected float aggroDropTimeLimit;

    [SerializeField] protected bool jumpEnabled = true;
    [SerializeField] protected bool roamEnabled = true;
    
    // Combat Stats
    [Header("Attack distance Values")]
    [SerializeField] protected float maxAttackRange;
    [SerializeField] protected float minAttackRange;  

    [Header("Roaming Values")]
    [SerializeField] protected float roamCooldown = 0.5f;
    [SerializeField] protected float idleCooldown = 1f;

    [Header("Attack Cooldown Values")]
    [SerializeField] protected float maxAttackCooldown; // Use random to get attack cooldown, but if you don't want random, then use max
    [SerializeField] protected float minAttackCooldown;
    [SerializeField] protected float currentCooldownTimer;

    [Header("Enemy Roaming Values")]
    protected int roamDirection;
    protected float roamTimer = 0;
    protected float idleTimer = 0;
    protected float aggroTimer = 0;

    [Header("Enemy Attacking Values")]
    [SerializeField] protected List<EnemyAttack> currentSequenceOfAttacks;
    [SerializeField] public EnemyAttack currentAttack { get; private set; }
    protected float delayTimer;
    protected float attackTimer;
    protected float recoveryTimer;

    [Header("Enemy State Values")]
    protected float stunTimer;
    protected bool isJumping;

    // Make Knockback it's own state
    protected enum EnemyState
    {
        idle,
        aggro,
        charging,
        attacking,
        recovering,
        repositioning,
        knockedback,
        dead
    }

    [SerializeField] protected EnemyState state;

    protected virtual void Start()
    {
        // Get components
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lineOfSight = GetComponent<LineOfSight>();
        animationHandler = GetComponent<AnimationHandler>();

        // Set starting information
        state = EnemyState.idle; // Set starting state
        player = GameObject.Find("Player").GetComponent<Player>(); // Find player
        target = player.transform; // Get player transform
        lineOfSight.setTarget(target);
        lineOfSight.setMaxDistance(aggroRange);
        roamDirection = Random.Range(-1, 2); // Set roaming direction
        roamTimer = roamCooldown;
    }
    public virtual void Die()
    {
        GameManager.instance.addExperience(xpValue);
        GameManager.instance.addGold(goldValue);
    }

    protected virtual void setUpSequenceOfAttacks(List<EnemyAttack> enemyAttacks)
    {
        aggroTimer = aggroDropTimeLimit;
        if (enemyAttacks.Count < 1)
            Debug.Log("Enemy has set up a sequence of 0 attacks");
        currentSequenceOfAttacks = enemyAttacks;
        currentAttack = currentSequenceOfAttacks[0];
        delayTimer = currentAttack.attackDelay;
        state = EnemyState.charging;
    }

    public virtual void onAggro()
    {
        currentCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown);
        aggroTimer = aggroDropTimeLimit;
        state = EnemyState.aggro;
    }

    public void halfAttackCooldown() => currentCooldownTimer /= 2;

    public bool isIdle() => state == EnemyState.idle;

    public bool isFree() => state == EnemyState.aggro || state == EnemyState.idle;

    public bool isRecovering() => state == EnemyState.recovering;
}
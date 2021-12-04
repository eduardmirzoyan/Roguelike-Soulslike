using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeapnState
{
    Ready,
    WindingUp,
    Active,
    Recovering
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Weapon : MonoBehaviour
{
    public int damage; // Damage the weapon deals

    protected float windupTimer;
    protected float activeTimer;
    protected float recoveryTimer;
    protected float attackMoveSpeed;

    [Header("Weapon Light Attack Values")]
    [SerializeField] public float lightStunTime;
    [SerializeField] public float lightWindupTime;
    [SerializeField] public float lightActiveTime;
    [SerializeField] public float lightRecoveryTime;
    [SerializeField] public int lightStaminaCost;
    [SerializeField] public float lightAttackMoveSpeed;

    [Header("Weapon Heavy Attack Values")]
    [SerializeField] public float heavyStunTime;
    [SerializeField] public float heavyWindupTime;
    [SerializeField] public float heavyActiveTime;
    [SerializeField] public float heavyRecoveryTime;
    [SerializeField] public int heavyStaminaCost;
    [SerializeField] public float heavyAttackMoveSpeed;

    [SerializeField] protected bool screenShakeOnFinish;

    [SerializeField] protected WeapnState state; // current state of the weapon

    // Swing
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRend;

    // Back reference
    [SerializeField] protected WeaponItem owner; // The reference to the weaponitem that created this object

    [SerializeField] protected Movement wielderMovement;

    // Keeps track of the combo that this weapon is on
    [SerializeField] public int currentCombo { get; private set; }

    [SerializeField] protected int maxCombo = 2;

    [SerializeField] public string weaponIdleAnimation;
    [SerializeField] public string weaponLightAttackAnimation;
    [SerializeField] public string weaponHeavyAttackAnimation;
    [SerializeField] public string weaponSpecialAttackAnimation;

    // Assuming instaniation means equippment
    protected void Start()
    {
        // Set weapon animator
        animator = GetComponent<Animator>();
        wielderMovement = GetComponentInParent<Movement>();

        // Set the owner of the weapon in order to get it's stats
        owner = GetComponentInParent<EquipmentHandler>().equippedWeaponItem;

        // Set weapon sprite
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.sprite = owner.sprite;

        // Set the weapon to ready state
        state = WeapnState.Ready;
    }

    protected void FixedUpdate()
    {
        switch (state)
        {
            case WeapnState.Ready:
                // Ready to use
                
                break;
            case WeapnState.WindingUp:
                // Weapon is winding up the attack
                if (windupTimer > 0)
                {
                    wielderMovement.Stop();
                    windupTimer -= Time.deltaTime;
                }
                else {
                    // Spawn trail if possible
                    var weaponTrail = GetComponent<WeaponSwingTrail>();
                    if(weaponTrail != null)
                        weaponTrail.spawnTrail();

                    state = WeapnState.Active; 
                }
                break;
            case WeapnState.Active:
                // Weapon is capable of dealing damage, hitbox active
                if (activeTimer > 0)
                {   
                    // Check for enemies hit
                    wielderMovement.Walk(wielderMovement.getFacingDirection() * attackMoveSpeed); /// HERE

                    //collidable.checkCollisions(damageFilteredEntities);
                    activeTimer -= Time.deltaTime;
                }
                else 
                {
                    if (screenShakeOnFinish)
                        GameManager.instance.verticalShakeCamera(0.15f, 0.15f);

                    currentCombo += 1;
                    wielderMovement.Stop();
                    GameEvents.current.triggerActionFinish(); // Trigger that the weapon has finished attacking
                    state = WeapnState.Recovering; 
                }
                break;
            case WeapnState.Recovering:
                // Weapon is recovering to ready state
                if (recoveryTimer > 0)
                    recoveryTimer -= Time.deltaTime;
                else {
                    // Reset Combo if you finish recovering
                    currentCombo = 0;
                    animator.Play(weaponIdleAnimation);
                    state = WeapnState.Ready;
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if(collision.gameObject != this.gameObject && damageable != null)
        {
            Damage dmg = new Damage
            {
                source = DamageSource.fromPlayer,
                damageAmount = (int)(damage * (1 + GetComponentInParent<CombatStats>()?.damageDealtMultiplier)),
                origin = transform.parent.position,
                isTrue = false,
                isAvoidable = true,
                triggersIFrames = true,
            };

            damageable.takeDamage(dmg);
        }
    }

    public void stopCurrentAttack()
    {
        animator.Play(weaponIdleAnimation);
        state = WeapnState.Ready;
    }

    public virtual void lightAttack()
    {
        animator.Play(weaponLightAttackAnimation + " " + currentCombo);
        damage = owner.lightDamage;
        windupTimer = lightWindupTime;
        activeTimer = lightActiveTime;
        recoveryTimer = lightRecoveryTime;
        attackMoveSpeed = lightAttackMoveSpeed;

        state = WeapnState.WindingUp; // Begin attack process
    }

    public virtual void heavyAttack()
    {
        animator.Play(weaponHeavyAttackAnimation);
        damage = owner.heavyDamage;
        windupTimer = heavyWindupTime;
        activeTimer = heavyActiveTime;
        recoveryTimer = heavyRecoveryTime;
        attackMoveSpeed = heavyAttackMoveSpeed;

        state = WeapnState.WindingUp; // Begin attack process
    }

    public virtual void specialAttack()
    {
        animator.Play(weaponSpecialAttackAnimation);
    }

    public bool isActive() => state == WeapnState.Active || state == WeapnState.WindingUp;

    public bool isReady() => state == WeapnState.Ready;

    public bool isRecovering() => state == WeapnState.Recovering;

    public float getActiveTime() => activeTimer;

    public bool atMaxCombo() => currentCombo > maxCombo;
}

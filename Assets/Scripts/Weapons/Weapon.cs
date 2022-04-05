using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState
{
    Ready,
    WindingUp,
    Active,
    Recovering
}

[RequireComponent(typeof(AnimationHandler))]
public abstract class Weapon : MonoBehaviour
{
    protected float windupTimer;
    protected float activeTimer;
    protected float recoveryTimer;
    protected int currentCombo; // Keeps track of the combo that this weapon is on
    protected float cooldownTimer;

    [Header("Weapon Values")]
    [SerializeField] protected List<BaseEffect> weaponEffects;
    [SerializeField] protected WeaponState state; // current state of the weapon
    [SerializeField] protected float windupDuration;
    [SerializeField] protected float activeDuration;
    [SerializeField] protected float recoveryDuration;
    [SerializeField] protected float attackMoveSpeed;
    [SerializeField] protected int maxCombo = 2;
    [SerializeField] protected float cooldown;
    
    [Header("Animation")]
    [SerializeField] protected string weaponIdleAnimation;
    [SerializeField] protected string weaponAttackAnimation;

    [Header("Components")]
    [SerializeField] protected AnimationHandler animationHandler;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected WeaponItem owner; // The reference to the weaponitem that created this object
    [SerializeField] protected CombatStats wielderStats;
    [SerializeField] protected Movement wielderMovement;
    

    // Assuming instaniation means equippment
    protected void Awake()
    {
        // Set weapon animator
        animationHandler = GetComponent<AnimationHandler>();
        wielderMovement = GetComponentInParent<Movement>();
        wielderStats = GetComponentInParent<CombatStats>();

        // Set weapon sprite
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the weapon to ready state
        state = WeaponState.Ready;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Damageable damageable) &&  collision.gameObject != this.gameObject)
        {
            var damage = (int) (owner.damage * (1 + wielderStats.damageDealtMultiplier));

            // Check crit
            int rand = Random.Range(0, 100);
            if(rand <= (wielderStats.percentCritChance + owner.critChance) * 100 )
            {
                print("crit!");
                GameManager.instance.CreatePopup("CRIT", transform.parent.position, Color.yellow);
                damage = (int) (damage * (1 + owner.critDamage));
            }

            Damage dmg = new Damage
            {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = transform,
                effects = weaponEffects
            };
            damageable.takeDamage(dmg);
        }
    }

     public virtual bool canInitiate() {
        if (currentCombo > maxCombo) {
            return false;
        }

        return cooldownTimer <= 0 && state == WeaponState.Ready || state == WeaponState.Recovering;
    }

    public virtual void initiateAttack()
    {
        animationHandler.changeAnimationState(weaponAttackAnimation + " " + currentCombo);
        windupTimer = windupDuration;
        activeTimer = activeDuration;
        recoveryTimer = recoveryDuration;

        state = WeaponState.WindingUp; // Begin attack process
    }

    public virtual bool canRelease() {
        return false;
    }

    public virtual void releaseAttack(float time) {
        // Does nothing for most weapons
    }

    public void addEffect(BaseEffect effect) {
        if (weaponEffects == null) {
            weaponEffects = new List<BaseEffect>();
        }
        weaponEffects.Add(effect);
    }

    public float getCooldownRatio() {
        if (cooldown == 0)
            return 0;
        return cooldownTimer / cooldown;
    }

    public void setOwner(WeaponItem weaponItem) {
        owner = weaponItem;
        spriteRenderer.sprite = weaponItem.sprite;
    }

    public WeaponItem getOwner() {
        return owner;
    }

    public string getAnimationName() {
        return weaponAttackAnimation + " " + currentCombo;
    }

    public bool isReady() {
        return state == WeaponState.Ready;
    }
}

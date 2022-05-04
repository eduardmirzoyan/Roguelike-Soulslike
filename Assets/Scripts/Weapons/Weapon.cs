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
    [SerializeField] protected int maxCombo = 2;
    [SerializeField] protected float cooldown;
    
    [Header("Animation")]
    [SerializeField] protected string weaponIdleAnimation;
    [SerializeField] protected string weaponAttackAnimation;

    [Header("Components")]
    [SerializeField] protected AnimationHandler animationHandler;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected WeaponItem owner; // The reference to the weaponitem that created this object
    [SerializeField] protected Stats wielderStats;
    [SerializeField] protected Movement wielderMovement;
    
    // Assuming instaniation means equippment
    protected void Awake()
    {
        // Set weapon animator
        animationHandler = GetComponent<AnimationHandler>();
        wielderMovement = GetComponentInParent<Movement>();
        wielderStats = GetComponentInParent<Stats>();

        // Set weapon sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        // If the main object does not have a renderer, then check children
        if (spriteRenderer == null) {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // Set the weapon to ready state
        state = WeaponState.Ready;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out Damageable damageable) && collider.gameObject != this.gameObject)
        {
            // Roll for miss
            int rand = Random.Range(0, 100);
            if(rand < (wielderStats.percentMissChance) * 100 )
            {
                PopUpTextManager.instance.createPopup("Miss", Color.gray, collider.transform.position);
                return;
            }

            var damage = (int) (owner.damage * (1 + wielderStats.damageDealtMultiplier));
            var damageColor = Color.white;

            // Roll for crit
            rand = Random.Range(0, 100);
            if(rand <= (wielderStats.percentCritChance + owner.critChance) * 100 )
            {
                // Change damage amount and color
                damage = (int) (damage * (1 + owner.critDamage));
                damageColor = Color.yellow;
                // Trigger event
                GameEvents.instance.triggerOnCrit(this, damageable.transform);
            }

            Damage dmg = new Damage
            {
                damageAmount = damage,
                source = DamageSource.fromPlayer,
                origin = transform,
                effects = weaponEffects,
                color = damageColor
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

    public virtual void initiateAttack() {
        // Stop movement
        wielderMovement.Walk(0);
        
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

    public bool isRecovering() {
        return state == WeaponState.Recovering;
    }

    public virtual void cancelAttack() {
        // Reset values
        if (state != WeaponState.Ready) {
            animationHandler.changeAnimationState(weaponIdleAnimation);
            cooldownTimer = cooldown;
            currentCombo = 0;
            state = WeaponState.Ready;
        }
    }
}

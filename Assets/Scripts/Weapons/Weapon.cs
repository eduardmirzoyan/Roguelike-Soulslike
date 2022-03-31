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
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Weapon : MonoBehaviour
{
    protected int damage;
    protected float windupTimer;
    protected float activeTimer;
    protected float recoveryTimer;
    protected int currentCombo; // Keeps track of the combo that this weapon is on

    [Header("Weapon Values")]
    [SerializeField] protected List<BaseEffect> weaponEffects;
    [SerializeField] protected WeaponState state; // current state of the weapon
    [SerializeField] protected float windupDuration;
    [SerializeField] protected float activeDuration;
    [SerializeField] protected float recoveryDuration;
    [SerializeField] protected float attackMoveSpeed;
    [SerializeField] protected int maxCombo = 2;

    [Header("Animation")]
    [SerializeField] protected string weaponIdleAnimation;
    [SerializeField] protected string weaponLightAttackAnimation;

    [Header("Components")]
    [SerializeField] protected AnimationHandler animationHandler;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected WeaponItem owner; // The reference to the weaponitem that created this object
    [SerializeField] protected Movement wielderMovement;
    

    // Assuming instaniation means equippment
    protected void Start()
    {
        // Set weapon animator
        animationHandler = GetComponent<AnimationHandler>();
        wielderMovement = GetComponentInParent<Movement>();

        // Set weapon sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = owner.sprite;

        // Set the weapon to ready state
        state = WeaponState.Ready;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Damageable damageable) &&  collision.gameObject != this.gameObject)
        {
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

    public void attack()
    {
        animationHandler.changeAnimationState(weaponLightAttackAnimation + " " + currentCombo);
        damage = owner.damage;
        windupTimer = windupDuration;
        activeTimer = activeDuration;
        recoveryTimer = recoveryDuration;

        state = WeaponState.WindingUp; // Begin attack process
    }

    public void addEffect(BaseEffect effect) {
        if (weaponEffects == null) {
            weaponEffects = new List<BaseEffect>();
        }
        weaponEffects.Add(effect);
    }

    public void removeEffect(BaseEffect effect) {
        weaponEffects.Remove(effect);
    }

    public void setOwner(WeaponItem weaponItem) {
        owner = weaponItem;
    }

    public bool canAttack() {
        if (currentCombo > maxCombo) {
            return false;
        }

        return state == WeaponState.Ready || state == WeaponState.Recovering;
    }

    public string getAnimationName() {
        return weaponLightAttackAnimation + " " + currentCombo;
    }
    public bool isReady() => state == WeaponState.Ready;
}

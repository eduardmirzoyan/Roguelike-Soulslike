using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EquipmentHandler))]
public class CombatHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Movement mv;
    [SerializeField] private EquipmentHandler equipment;
    [SerializeField] private Stamina stamina;
    [SerializeField] private Animator animator;
    [SerializeField] private Keybindings keybindings;
    [SerializeField] public AbilityHolder signatureAbilityHolder;
    [SerializeField] public AbilityHolder utilityAbilityHolder;
    [SerializeField] public List<Ability> allPlayerAbilities;
    [SerializeField] private ActiveSkill tempSkill;
    [SerializeField] private AnimationHandler animationHandler;

    [Header("Weapons")]
    [SerializeField] private Weapon mainHandWeapon;
    [SerializeField] private Weapon offHandWeapon;

    [Header("Rolling")]
    [SerializeField] private string rollAnimation;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollSpeed = 350f;
    private float rollTimer;
    private float rollDirection;

    public bool attacking { get; private set; }

    private void Awake()
    {
        mv = GetComponent<ComplexMovement>();
        animator = GetComponent<Animator>();
        stamina = GetComponent<Stamina>();
        equipment = GetComponent<EquipmentHandler>();
        keybindings = GetComponent<Keybindings>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    private void Start()
    {
        // Temp add the block ability to the player
        var abilityCopy = Instantiate(tempSkill.ability);

        if (utilityAbilityHolder.isEmpty())
            utilityAbilityHolder.changeAbility(abilityCopy);

        allPlayerAbilities.Add(abilityCopy);
        GetComponent<Player>().playerSkills.Add(tempSkill);

        GameEvents.current.triggerPlayerEquippedUtility(utilityAbilityHolder);
    }

    public Weapon getMainHandWeapon() {
        return mainHandWeapon;
    }

    public Weapon getOffHandWeapon() {
        return offHandWeapon;
    }

    public void setMainHandWeapon(Weapon weapon) {
        mainHandWeapon = weapon;
    }

    public void setOffHandWeapon(Weapon weapon) {
        offHandWeapon = weapon;
    }

    public void mainHandAttack() {
        if (mainHandWeapon != null && mainHandWeapon.canAttack()) {
            mainHandWeapon.attack();

            // Get animation for player from the weapon
            animationHandler.changeAnimationState(mainHandWeapon.getAnimationName());

            GameEvents.current.togglePlayerLightAttack(true);
            GameEvents.current.triggerActionStart();
        }
    }

    public void offhandAttack() {
        if (offHandWeapon != null && offHandWeapon.canAttack()) {
            offHandWeapon.attack();

            // Animation is based on the current combo you are on
            animationHandler.changeAnimationState(offHandWeapon.getAnimationName());

            GameEvents.current.togglePlayerLightAttack(true);
            GameEvents.current.triggerActionStart();
        }
    }

    public bool mainCanCombo() {
        if (offHandWeapon == null || offHandWeapon.isReady()) {
            return true;
        }
        return false;
    }

    public bool offCanCombo() {
        if (mainHandWeapon == null || mainHandWeapon.isReady()) {
            return true;
        }
        return false;
    }

    public void attemptToUseSignatureAbility()
    {
        // Check if player has an ability equipped and ready and if he has pressed the correct key
        if (signatureAbilityHolder.isReady())
        {
            // Check if the player has enough stamina to use the ability
            if (stamina.drainStamina(signatureAbilityHolder.getAbility().staminaCost))
            {
                // Check if ability requires a weapon to use
                if (signatureAbilityHolder.getAbility().requiresWeapon && GetComponentInChildren<Weapon>() == null)
                    GameManager.instance.CreatePopup("You need a weapon equipped to use this ability.", transform.position);
                else
                {
                    // if all pass, then use the ability and change the player's state
                    mv.Stop();
                    GameEvents.current.triggerActionStart();
                    signatureAbilityHolder.useAbility();
                    attacking = true;

                    // Trigger event
                    GameEvents.current.triggerPlayerUseSignature();
                }
            }
            else
                GameManager.instance.CreatePopup("Not enough stamina.", transform.position);
        }
    }

    public void attemptToUseUtilityAbility()
    {
        // Check if player has an ability equipped and ready and if he has pressed the correct key
        if (utilityAbilityHolder.isReady())
        {
            // Check if the player has enough stamina to use the ability
            if (stamina.drainStamina(utilityAbilityHolder.getAbility().staminaCost))
            {
                // Check if ability requires a weapon to use
                if (utilityAbilityHolder.getAbility().requiresWeapon && GetComponentInChildren<Weapon>() == null)
                    GameManager.instance.CreatePopup("You need a weapon equipped to use this ability.", transform.position);
                else
                {
                    // if all pass, then use the ability and change the player's state
                    mv.Stop();
                    GameEvents.current.triggerActionStart();
                    utilityAbilityHolder.useAbility();

                    //state = PlayerState.locked;
                    attacking = true;

                    // Trigger event
                    GameEvents.current.triggerPlayerUseUtility();
                }
            }
            else
                GameManager.instance.CreatePopup("Not enough stamina.", transform.position);
        }
    }

    public void cancelCurrentAttack()
    {
        // if(equipment.weapon != null && !equipment.weapon.isReady())
        // {
        //     equipment.weapon.stopCurrentAttack();
        // }

        if (!signatureAbilityHolder.isReady())
        {
            signatureAbilityHolder.cancelAbility();
        }

        if (!utilityAbilityHolder.isReady())
        {
            utilityAbilityHolder.cancelAbility();
        }
    }

    public void equipSignatureAbility(Ability ability)
    {
        signatureAbilityHolder.changeAbility(ability);
        GameEvents.current.triggerPlayerEquippedSignature(signatureAbilityHolder);
    }

    public AbilityHolder getSignatureAbilityHolder() => signatureAbilityHolder;

    public AbilityHolder getUtilityAbilityHolder() => utilityAbilityHolder;

    private void resetCombatValues()
    {
        GameEvents.current.togglePlayerLightAttack(false);
        GameEvents.current.togglePlayerHeavyAttack(false);
    }
    
    public bool isDoneAttacking() {
        // Either mainhand weapon is not equipped or in ready state
        if (mainHandWeapon == null || mainHandWeapon.isReady()) {
            // Either offhand weapon is not equipped or in ready state
            if (offHandWeapon == null || offHandWeapon.isReady()) {
                // Only then are you done attacking
                return true;
            }
            return false;
        }
        return false;
    }

    // Rolling values
    public void startRoll(float direction) {
        rollTimer = rollDuration;
        rollDirection = direction;
    }

    public void roll() {
        animationHandler.changeAnimationState(rollAnimation);
        mv.WalkAtSpeed(rollDirection, rollSpeed);
        if (rollTimer > 0) {
            rollTimer -= Time.deltaTime;
        }
    }

    public bool isDoneRolling() {
        return rollTimer <= 0;
    }
}

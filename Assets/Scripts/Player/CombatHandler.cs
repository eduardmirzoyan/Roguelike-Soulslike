using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EquipmentHandler))]
public class CombatHandler : MonoBehaviour
{
    [SerializeField] private Movement mv;
    [SerializeField] private EquipmentHandler equipment;
    [SerializeField] private Stamina stamina;
    [SerializeField] private Animator animator;
    [SerializeField] private Keybindings keybindings;

    [SerializeField] public AbilityHolder signatureAbilityHolder;
    [SerializeField] public AbilityHolder utilityAbilityHolder;

    [SerializeField] public List<Ability> allPlayerAbilities;

    [SerializeField] private Ability temp;
    [SerializeField] private ActiveSkill tempSkill;

    [SerializeField] private string lightAttackAnimation;

    [SerializeField] private AnimationHandler animationHandler;

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

    private void Update()
    {
        if (attacking)
        {
            if ((equipment.weapon?.isReady() ?? true) && !signatureAbilityHolder.isInUse() && !utilityAbilityHolder.isInUse())
            {
                attacking = false;
                resetCombatValues();
            }
        }
    }

    public void attemptToLightAttack()
    {
        if (equipment.weapon.isReady() || equipment.weapon.isRecovering())
        {
            if (equipment.weapon.atMaxCombo())
                return;
            if (stamina.drainStamina(equipment.weapon.lightStaminaCost)) // Attempts to drain players stamina for attack, if player has enough, initiates attack, else not
            {
                // Stop the player from moving
                mv.Stop();

                // Animate the weapon itself
                equipment.weapon.lightAttack();

                // Get animation for player from the weapon
                // Animation is based on the current combo you are on
                animationHandler.changeAnimationState("");
                animator.Play(equipment.weapon.weaponLightAttackAnimation + " " + equipment.weapon.currentCombo);

                GameEvents.current.togglePlayerLightAttack(true);
                GameEvents.current.triggerActionStart();
                attacking = true;
            }
            else
                GameManager.instance.CreatePopup("Not enough stamina.", transform.position);
        }     
    }

    public void attemptToHeavyAttack()
    {
        if (equipment.weapon.isReady()) /// HERE
        {
            if (stamina.drainStamina(equipment.weapon.heavyStaminaCost)) // Attempts to drain players stamina for attack, if player has enough, initiates attack, else not
            {
                mv.Stop();

                // Animate the weapon itself
                equipment.weapon.heavyAttack();

                // Get animation for player from the weapon
                // Animation is based on the current combo you are on
                animationHandler.changeAnimationState("");
                animator.Play(equipment.weapon.weaponHeavyAttackAnimation);
                

                GameEvents.current.togglePlayerHeavyAttack(true);
                GameEvents.current.triggerActionStart();
                attacking = true;
            }
            else
                GameManager.instance.CreatePopup("Not enough stamina.", transform.position);
        }
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
        if(equipment.weapon != null && !equipment.weapon.isReady())
        {
            equipment.weapon.stopCurrentAttack();
        }

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
}

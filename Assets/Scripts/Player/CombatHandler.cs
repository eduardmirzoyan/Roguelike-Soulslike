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
    [SerializeField] private Stamina stamina;
    [SerializeField] public AbilityHolder signatureAbilityHolder;
    [SerializeField] public AbilityHolder utilityAbilityHolder;
    [SerializeField] public List<Ability> allPlayerAbilities;
    [SerializeField] private AnimationHandler animationHandler;

    [Header("Settings")]
    [SerializeField] private float attackMoveSpeedMultiplier;

    [Header("Weapons")]
    [SerializeField] private Weapon mainHandWeapon;
    [SerializeField] private Weapon offHandWeapon;

    private void Awake()
    {
        mv = GetComponent<ComplexMovement>();
        stamina = GetComponent<Stamina>();
        animationHandler = GetComponent<AnimationHandler>();
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

    // Returns if the attack can happen
    public bool mainHandAttack() {
        if ((mainHandWeapon != null && mainHandWeapon.canInitiate()) && (offHandWeapon == null || offHandWeapon.isReady())) {
            // Animation is based on the current combo you are on
            animationHandler.changeAnimationState(mainHandWeapon.getAnimationName());

            mainHandWeapon.initiateAttack();

            return true;
        }
        return false;
    }

    // Returns if the attack can happen
    public bool offhandAttack() {
        if ((offHandWeapon != null && offHandWeapon.canInitiate()) && (mainHandWeapon == null || mainHandWeapon.isReady())) {
            // Animation is based on the current combo you are on
            animationHandler.changeAnimationState(offHandWeapon.getAnimationName());

            offHandWeapon.initiateAttack();
            return true;
        }
        return false;
    }

    public bool mainHandRelease(float time) {
        if (mainHandWeapon != null && mainHandWeapon.canRelease()) {
            // Release weapon
            mainHandWeapon.releaseAttack(time);

            return true;
        }
        return false;
        
    }

    public bool offHandRelease(float time) {
        if (offHandWeapon != null && offHandWeapon.canRelease()) {
            // Release weapon
            offHandWeapon.releaseAttack(time);

            return true;
        }
        return false;
        
    }

    public void cancelAllAttacks() {
        if (mainHandWeapon != null) {
            mainHandWeapon.cancelAttack();
        }

        if (offHandWeapon != null) {
            offHandWeapon.cancelAttack();
        }
    }
    
    public void equipSignatureAbility(Ability ability)
    {
        signatureAbilityHolder.changeAbility(ability);
    }

    public AbilityHolder getUtilityAbilityHolder() => utilityAbilityHolder;
    
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

    public bool weaponsAreRecovering() {
        // Either mainhand weapon is not equipped or in recover state
        if (mainHandWeapon == null || mainHandWeapon.isRecovering() || mainHandWeapon.isReady()) {
            // Either offhand weapon is not equipped or in recover state
            if (offHandWeapon == null || offHandWeapon.isRecovering() || offHandWeapon.isReady()) {
                // Then you are recovering
                return true;
            }
            return false;
        }
        return false;
    }
    
}

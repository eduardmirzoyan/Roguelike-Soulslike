using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }
    
    public event Action onHit;
    public event Action onPerfectBlock;

    // Player Related events
    public event Action<Weapon> onPlayerEquippedWeapon;
    public event Action<AbilityHolder> onPlayerEquippedSignature;
    public event Action<AbilityHolder> onPlayerEquippedUtility;

    public event Action<bool> onPlayerIsLightAttack;
    public event Action<bool> onPlayerIsHeavyAttack;

    public event Action onPlayerUseSignature;
    public event Action onPlayerUseUtility;

    public event Action onActionStart;
    public event Action onActionFinish;

    public void triggerOnHit()
    {
        if (onHit != null)
        {
            onHit();
        }
    }

    public void triggerOnPerfectBlock()
    {
        if (onPerfectBlock != null)
        {
            onPerfectBlock();
        }
    }

    public void togglePlayerLightAttack(bool state)
    {
        if (onPlayerIsLightAttack != null)
        {
            onPlayerIsLightAttack(state);
        }
    }

    public void togglePlayerHeavyAttack(bool state)
    {
        if (onPlayerIsHeavyAttack != null)
        {
            onPlayerIsHeavyAttack(state);
        }
    }

    public void triggerPlayerEquippedWeapon(Weapon weapon)
    {
        if(onPlayerEquippedWeapon != null)
        {
            onPlayerEquippedWeapon(weapon);
        }
    }

    public void triggerPlayerEquippedSignature(AbilityHolder abilityHolder)
    {
        if (onPlayerEquippedSignature != null)
        {
            onPlayerEquippedSignature(abilityHolder);
        }
    }

    public void triggerPlayerEquippedUtility(AbilityHolder abilityHolder)
    {
        if (onPlayerEquippedUtility != null)
        {
            onPlayerEquippedUtility(abilityHolder);
        }
    }

    public void triggerPlayerUseSignature()
    {
        if (onPlayerUseSignature != null)
        {
            onPlayerUseSignature();
        }
    }
    public void triggerPlayerUseUtility()
    {
        if (onPlayerUseUtility != null)
        {
            onPlayerUseUtility();
        }
    }

    public void triggerActionStart()
    {
        if (onActionStart != null)
        {
            onActionStart();
        }
    }

    public void triggerActionFinish()
    {
        if (onActionFinish != null)
        {
            onActionFinish();
        }
    }
}

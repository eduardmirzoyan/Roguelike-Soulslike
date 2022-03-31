using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityHolderUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private AbilityHolder abilityHolder;
    [SerializeField] private Cooldownbar cooldownbar;

    private enum AbilityToTrack
    {
        Signature,
        Utility
    }

    [SerializeField] private AbilityToTrack toTrack;

    private void Start()
    {
        image.enabled = false;
        if (toTrack == AbilityToTrack.Signature)
        {
            //GameEvents.current.onPlayerEquippedSignature += updateAbility;
            //GameEvents.current.onPlayerUseSignature += onAbilityUse;
        }
        else
        {
            //GameEvents.current.onPlayerEquippedUtility += updateAbility;
            //GameEvents.current.onPlayerUseUtility += onAbilityUse;
            abilityHolder = GameObject.Find("Player").GetComponent<CombatHandler>().utilityAbilityHolder;
            //abilityHolder = GameManager.instance.player.combatHandler.utilityAbilityHolder;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (abilityHolder != null)
        {
            image.enabled = true;
            image.sprite = abilityHolder.getAbility().sprite;
        }
        else
        {
            image.enabled = false;
        }

        if (abilityHolder != null && abilityHolder.isOnCooldown())
            cooldownbar.setCooldown(abilityHolder.getCooldown());
        else
            cooldownbar.setCooldown(0);
    }

    private void updateAbility(AbilityHolder holder)
    {
        if (holder != null)
        {
            image.enabled = true;
            image.sprite = holder.getAbility().sprite;
            abilityHolder = holder;
        }
        else
        {
            image.enabled = false;
            cooldownbar.setMaxCooldown(0);
            image.sprite = null;
            abilityHolder = null;
        }
    }

    private void onAbilityUse()
    {
        float value = abilityHolder.getMaxCooldown();
        cooldownbar.setMaxCooldown(value);
    }
}

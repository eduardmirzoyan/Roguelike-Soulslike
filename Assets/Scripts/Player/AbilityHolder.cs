using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MOVE TOGGLING ABILITY INTO ABILITY ITSELF
public class AbilityHolder : MonoBehaviour
{
    [Header("Ability Details")]
    [SerializeField] protected Ability ability;

    protected float abilityChargeUpTime;
    protected float abilityActiveTime;
    protected float abilityCooldownTime;

    public enum AbilityState
    {
        Ready,
        Charging,
        Active,
        Cooldown
    }
    [SerializeField] protected AbilityState state = AbilityState.Ready;

    private void FixedUpdate()
    {
        if(ability != null)
        {
            switch (state)
            {
                case AbilityState.Ready:
                    // Wait until activation is called by Player

                    break;
                case AbilityState.Charging:
                    if (abilityChargeUpTime > 0)
                    {
                        ability.performDuringChargeUp(gameObject);
                        abilityChargeUpTime -= Time.deltaTime;
                    }
                    else
                    {
                        ability.performAfterChargeUp(gameObject);
                        abilityActiveTime = ability.activeTime;
                        state = AbilityState.Active;
                    }
                    break;
                case AbilityState.Active:
                    if (abilityActiveTime > 0)
                    {
                        abilityActiveTime -= Time.deltaTime;

                        ability.performDuringActive(gameObject);
                    }
                    else
                    {
                        ability.performAfterActive(gameObject);

                        state = AbilityState.Cooldown;
                        abilityCooldownTime = ability.cooldownTime;
                    }
                    break;
                case AbilityState.Cooldown:
                    if (abilityCooldownTime > 0)
                    {
                        abilityCooldownTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = AbilityState.Ready;
                    }
                    break;
            }
        }
    }

    public void changeAbility(Ability ability)
    {
        // If you already have an ability, uninstanitate it
        if (this.ability != null)
            this.ability.uninstantiate(gameObject);

        // Set new ability and intanitate it
        this.ability = ability;
        this.ability.instantiate(gameObject);

    }

    public Ability getAbility()
    {
        return ability;
    }

    public bool isEmpty()
    {
        return ability == null;
    }

    public void useAbility()
    {
        ability.perfromBeforeChargeUp(gameObject);
        abilityChargeUpTime = ability.chargeUpTime;
        state = AbilityState.Charging;
    }

    // Returns true if the ability is charging or active
    public bool isInUse()
    {
        return ability != null && (state == AbilityState.Active || state == AbilityState.Charging);
    }

    public bool isReady()
    {
        return ability != null && state == AbilityState.Ready;
    }

    public bool isOnCooldown()
    {
        return ability != null && state == AbilityState.Cooldown;
    }

    public void reduceAbilityCooldown(float seconds)
    {
        abilityCooldownTime -= seconds;
    }

    public void finishActiveTime()
    {
        if(ability != null)
            abilityActiveTime = 0;
    }

    public void refreshActiveTime()
    {
        if (ability != null)
            abilityActiveTime = ability.activeTime;
    }

    public void cancelAbility()
    {
        if (ability == null)
            return;

        abilityCooldownTime = ability.cooldownTime;
        state = AbilityState.Cooldown;
    }

    public float getCooldown()
    {
        return abilityCooldownTime;
    }

    public float getMaxCooldown()
    {
        return ability.cooldownTime;
    }
}

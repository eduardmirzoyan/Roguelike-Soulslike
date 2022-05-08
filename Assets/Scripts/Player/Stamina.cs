using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    private enum StaminaState { Full, Depleted, Regenerating };
    [SerializeField] private StaminaState staminaState;
    public int maxStamina;
    public float currentStamina;

    [SerializeField] private float regenDelay = 2f;
    [SerializeField] private float regenDuration = 5f;

    private float regenDurationTimer;
    private float regenDelayTimer;

    private void Start() {
        GameEvents.instance.triggerStaminaChange(this);
    }

    private void FixedUpdate() {

        switch (staminaState) {
            case StaminaState.Full:
                // Do nothing until used

            break;
            case StaminaState.Depleted:
                if (regenDelayTimer > 0) {
                    regenDelayTimer -= Time.deltaTime;
                }
                else {
                    // Begin to refill
                    staminaState = StaminaState.Regenerating;
                }
            break;
            case StaminaState.Regenerating:
                // Refill Stamina
                currentStamina += (maxStamina / regenDuration) * Time.deltaTime;

                 if (currentStamina >= maxStamina) {
                    // Stamina is reset
                    currentStamina = maxStamina;
                    
                    // Change to Full state
                    staminaState = StaminaState.Full;
                }   

                // Trigger event
                GameEvents.instance.triggerStaminaChange(this);
                
                
            break;
        }
    }

    private void altRegenMethod() {
        /// Regen based off "regenRate" which is how much stam gained per sec

        // if (regenTickTimer >= 0) {
        //     regenTickTimer -= Time.deltaTime;

        // }
        // else {
        //     print(Time.time);
        //     // Refill slowly
        //     currentStamina += regenRate * regenTick;
            
        //     // Check if full
        //     if (currentStamina >= maxStamina) {
        //         // Stamina is filled
        //         currentStamina = maxStamina;

        //         // Change to Full state
        //         staminaState = StaminaState.Full;
        //     }   
        //     else {
        //         // Restart timer
        //         regenTickTimer = regenTick;
        //     }
            
        //     // Trigger event
        //     GameEvents.instance.triggerStaminaChange(this);
        // }
    }

    public bool useStamina(int amount) {
        // If stamina is depleted, then dont use stamina
        if (currentStamina <= 0) {
            // Visual feedback
            PopUpTextManager.instance.createVerticalPopup("Not Enough Stamina", Color.gray, transform.position);
            return false;
        }

        // If you use 0, then do nothing
        if (amount == 0) {
            return true;
        }

        if (amount < 0) {
            throw new System.Exception("NEGATIVE STAMINA USED!");
        }

        // Subtract amount (we can go negative)
        currentStamina -= amount;

        // Calculate amount of time to generate 1 stamina
        regenDelayTimer = regenDelay;

        // Change state
        staminaState = StaminaState.Depleted;

        // Trigger event
        GameEvents.instance.triggerStaminaChange(this);

        // Return result
        return true;
    }

    public void restoreStamina(int amount) {
        // Restore stamina up to Max
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        
        // Change state if stamina is refilled
        if (currentStamina >= maxStamina) {
            // Change states
            staminaState = StaminaState.Full;
        }

        // Trigger event
        GameEvents.instance.triggerStaminaChange(this);
    }

    public int getCurrent() {
        return (int) currentStamina;
    }

    public int getMax() {
        return maxStamina;
    }

    public string getStatus()
    {
        return ((int) currentStamina).ToString() + " / " + maxStamina.ToString();
    }
}

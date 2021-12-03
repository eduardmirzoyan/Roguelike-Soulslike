using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public int maxStamina;
    public int currentStamina;
    [SerializeField] private int regenerationValue;
    [SerializeField] private float regenerationRate;

    public float regenTimer;

    public float getRegenerationRate()
    {
        return regenerationRate;
    }

    // Returns True if after reduction, the player has 0 or more stamina, else returns False if player would have gone negative
    public bool drainStamina(int value)
    {
        if (currentStamina >= value)
        {
            regenTimer = 2f; // Reset timer
            currentStamina -= value; // Drain stamina
            return true; // Ability can be used
        }
        return false;
    }

    public bool drainStaminaTilEmpty(int value)
    {
        if (currentStamina >= value)
        {
            regenTimer = 2f; // Reset timer
            currentStamina -= value; // Drain stamina
            return true; // Ability can be used
        }
        currentStamina = 0;
        return false;
    }

    // Returns true if player would normal use their stamina, but returns false if the player has gone over, and should be punished
    public bool forceddrainStamina(int value)
    {
        // Normal drain
        if (currentStamina >= value)
        {
            regenTimer = 5f; // Reset timer
            currentStamina -= value; // Drain stamina
            return true;
        }
        currentStamina = 0;
        return false;
    }

    public void regenerateStamina()
    {
        if (currentStamina + regenerationValue >= maxStamina)
        {
            currentStamina = maxStamina;
        }
        else
        {
            currentStamina += regenerationValue;
        }
    }

    public bool isFull()
    {
        return currentStamina >= maxStamina;
    }

    public bool isEmpty()
    {
        return currentStamina <= 0;
    }

    public string getStatus()
    {
        return currentStamina.ToString() + " / " + maxStamina.ToString();
    }
}

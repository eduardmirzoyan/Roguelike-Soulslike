using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public void intialize(int amount)
    {
        maxHealth = amount;
        currentHealth = amount;
    }

    public void reduceHealth(int amount)
    {
        if (currentHealth - amount < 0)
            currentHealth = 0;
        else
            currentHealth -= amount;
    }

    public void increaseHealth(int amount)
    {
        if (currentHealth + amount > maxHealth)
            currentHealth = maxHealth;
        else
            currentHealth += amount;
    }

    public void increaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
    }

    public bool isEmpty() => currentHealth <= 0;

    public bool isFull() => currentHealth >= maxHealth; 

    public int getMaxHP() => maxHealth;

    public int getHP() => currentHealth;

    public string getStatus()
    {
        return currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}

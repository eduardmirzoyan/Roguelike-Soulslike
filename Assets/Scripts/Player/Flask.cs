using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flask : MonoBehaviour
{
    [SerializeField] private int maxCharges;
    [SerializeField] private int currentCharges;
    [SerializeField] private float healingRatio; // Should be <= 1

    public void recharge()
    {
        currentCharges += 1;
        if (currentCharges > maxCharges)
            currentCharges = maxCharges;
    }

    public void refill()
    {
        currentCharges = maxCharges;
    }

    public bool use()
    {
        if (currentCharges <= 0) {
            currentCharges = 0;
            return false;
        }
        currentCharges -= 1;
        return true;
    }

    public int getCurrentCharges()
    {
        return currentCharges;
    }

    public int getMaxCharges()
    {
        return maxCharges;
    }

    public float getHealPercentage()
    {
        return healingRatio;
    }

    public bool isEmpty()
    {
        return currentCharges == 0;
    }

    public bool isFull() {
        return currentCharges >= maxCharges;
    }
}

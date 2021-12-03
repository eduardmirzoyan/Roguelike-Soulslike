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
        GameManager.instance.CreatePopup("Your fkask has been refilled.", transform.position);
        currentCharges = maxCharges;
    }

    public bool use()
    {
        currentCharges -= 1;
        if (currentCharges < 0)
        {
            currentCharges = 0;
            return false;
        }
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
        return currentCharges == 0 ? true : false;
    }
}

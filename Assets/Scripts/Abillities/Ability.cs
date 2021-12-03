using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public new string name;
    public float chargeUpTime;
    public float activeTime;
    public float cooldownTime;
    public int staminaCost;

    public Sprite sprite;
    public int abilityLevel;
    public bool requiresWeapon;
    public bool activeWhileHeld;

    [SerializeField] protected LinkedList<Upgrade> upgrades = new LinkedList<Upgrade>();

    // Called when ability is equipped
    public virtual void instantiate(GameObject parent)
    {
        // Check for any upgrades
        var currentUpgrade = upgrades.First;
        while (currentUpgrade != null)
        {
            currentUpgrade.Value.upgradeInstaniation(parent, this);
            currentUpgrade = currentUpgrade.Next;
        }
    }

    // Called before charge up
    public virtual void perfromBeforeChargeUp(GameObject parent)
    {

        // Check for any upgrades
        var currentUpgrade = upgrades.First;
        while (currentUpgrade != null)
        {
            currentUpgrade.Value.upgradeBeforeChargeUp(parent, this);
            currentUpgrade = currentUpgrade.Next;
        }
    }

    // Called during the charge up time
    public virtual void performDuringChargeUp(GameObject parent)
    {
        // Check for any upgrades
        var currentUpgrade = upgrades.First;
        while (currentUpgrade != null)
        {
            currentUpgrade.Value.upgradeDuringChargeUp(parent, this);
            currentUpgrade = currentUpgrade.Next;
        }
    }

    // Called right after charge up is finished or during first frame of active time
    public virtual void performAfterChargeUp(GameObject parent)
    {
        // Check for any upgrades
        var currentUpgrade = upgrades.First;
        while (currentUpgrade != null)
        {
            currentUpgrade.Value.upgradeAfterChargeUp(parent, this);
            currentUpgrade = currentUpgrade.Next;
        }
    }

    // Called during the abilities active time
    public virtual void performDuringActive(GameObject parent)
    {
        // Check for any upgrades
        var currentUpgrade = upgrades.First;
        while (currentUpgrade != null)
        {
            currentUpgrade.Value.upgradeDuringActive(parent, this);
            currentUpgrade = currentUpgrade.Next;
        }
    }

    // Called right after ability has finished activating
    public virtual void performAfterActive(GameObject parent)
    {
        // Send alert that you finished the action
        GameEvents.current.triggerActionFinish();

        // Check for any upgrades
        var currentUpgrade = upgrades.First;
        while (currentUpgrade != null)
        {
            currentUpgrade.Value.upgradeAfterActive(parent, this);
            currentUpgrade = currentUpgrade.Next;
        }
    }

    // Called when ability is unequipped
    public virtual void uninstantiate(GameObject parent)
    {
        // Check for any upgrades
        var currentUpgrade = upgrades.First;
        while (currentUpgrade != null)
        {
            currentUpgrade.Value.upgradeUninstaniate(parent, this);
            currentUpgrade = currentUpgrade.Next;
        }
    }

    public void addUpgrade(Upgrade upgrade)
    {
        upgrades.AddFirst(upgrade);
    }

    public void listUpgrades()
    {
        var currentUpgrade = upgrades.First;
        while (currentUpgrade != null)
        {
            Debug.Log(currentUpgrade.Value.name);
            currentUpgrade = currentUpgrade.Next;
        }
    }
}

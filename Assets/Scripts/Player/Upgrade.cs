using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : ScriptableObject
{
    public virtual void upgradeInstaniation(GameObject parent, Ability ability) { }
    public virtual void upgradeBeforeChargeUp(GameObject parent, Ability ability) { }
    public virtual void upgradeDuringChargeUp(GameObject parent, Ability ability) { }
    public virtual void upgradeAfterChargeUp(GameObject parent, Ability ability) { }
    public virtual void upgradeDuringActive(GameObject parent, Ability ability) { }
    public virtual void upgradeAfterActive(GameObject parent, Ability ability) { }
    public virtual void upgradeUninstaniate(GameObject parent, Ability ability) { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : Shield
{
    public override void blockDamage(Damage dmg)
    {
        GameManager.instance.CreatePopup("Enemy BLOCK!", transform.position);

        var damageable = GetComponentInParent<Damageable>();
        if (damageable != null)
            damageable.resetImmunityFrames();

    }
}

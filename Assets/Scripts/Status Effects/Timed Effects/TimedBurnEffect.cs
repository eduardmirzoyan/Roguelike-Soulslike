using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBurnEffect : TimedEffect
{
    private Health health;
    private Rigidbody2D rigidbody2D;

    public TimedBurnEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
    {
        health = parent.GetComponent<Health>();
        rigidbody2D = parent.GetComponent<Rigidbody2D>();
    }

    public override void End()
    {
        // Does nothing on end
    }

    protected override void ApplyEffect()
    {
        // Does nothing on apply
    }

    protected override void onTick()
    {
        BurnEffect burnEffect = (BurnEffect)Effect;
        
        if (health != null) {
            // If the entitiy is standing still, increase the tickrate
            if (rigidbody2D.velocity.magnitude < 1f) {
                burnEffect.tickRate = 0.5f;
            }
            else {
                burnEffect.tickRate = 1f;
            }

            health.reduceHealth(burnEffect.tickDamage);
            GameManager.instance.CreatePopup(burnEffect.tickDamage + "", health.transform.position, burnEffect.damageColor);
        }
    }
}

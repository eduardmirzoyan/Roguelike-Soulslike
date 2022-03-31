using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPoisonEffect : TimedEffect
{
    private Health health;
    private Rigidbody2D rigidbody2D;

    public TimedPoisonEffect(BaseEffect effect, GameObject parent) : base(effect, parent)
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
        PoisonEffect poisonEffect = (PoisonEffect)Effect;
        Debug.Log(Duration);
        if (health != null) {
            // If the entitiy is moving, increase the tickrate
            if (rigidbody2D.velocity.magnitude < 1f) {
                poisonEffect.tickRate = 1f;
            }
            else {
                poisonEffect.tickRate = 0.5f;
            }

            health.reduceHealth(poisonEffect.tickDamage);
            GameManager.instance.CreatePopup(poisonEffect.tickDamage + "", health.transform.position, poisonEffect.damageColor);
        }
    }
}

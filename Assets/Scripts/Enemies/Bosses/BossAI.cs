using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Displacable))]
public class BossAI : EnemyAI
{
    [Header("Boss Components")]
    [SerializeField] protected Health health;
    [SerializeField] protected Movement mv;
    [SerializeField] protected Displacable displacable;
    [SerializeField] protected Collidable collidable;
    [SerializeField] protected Damageable damageable;

    [Header("Boss Animations")]
    [SerializeField] protected string idleAnimation = "Idle";
    [SerializeField] protected string walkAnimation = "Walk";
    [SerializeField] protected string risingAnimation = "Rise";
    [SerializeField] protected string fallingAnimation = "Fall";
    [SerializeField] protected string deadAnimation = "Die";

    // Use this for bosses
    protected enum BossState
    {
        dormant,
        introducing,
        aggro,
        charging,
        attacking,
        recovering,
        repositioning,
        locked,
        dead
    }
    protected override void Start()
    {
        base.Start();

        // Get required components
        damageable = GetComponent<Damageable>();
        mv = GetComponent<Movement>();
        displacable = GetComponent<Displacable>();
        collidable = GetComponent<Collidable>();
        health = GetComponent<Health>();
    }

    public override void Die()
    {
        base.Die();
        animationHandler.changeAnimationState(deadAnimation);
        Destroy(gameObject, 10f); // Despawn boss corpse in 10 seconds
        state = EnemyState.dead; // Set state to dead
    }

    public override void onAggro()
    {
        facePlayer();
        player.setBossHealthBar(this);
    }

    protected void facePlayer()
    {
        mv.setFacingDirection(target.transform.position.x - transform.position.x);
    }

    protected void handleMovementAnimations()
    {
        if (!mv.isGrounded())
        {
            if (body.velocity.y > 0)
                animationHandler.changeAnimationState(risingAnimation);
            else
                animationHandler.changeAnimationState(fallingAnimation);
        }
        else
        {
            if (Mathf.Abs(body.velocity.x) > 0.2f)
                animationHandler.changeAnimationState(walkAnimation);
            else
                animationHandler.changeAnimationState(idleAnimation);
        }
    }
}

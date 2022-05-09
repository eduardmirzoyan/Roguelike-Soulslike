using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
public class BossAI : EnemyAI
{   
    [SerializeField] protected BossHealthBarUI bossHealthBarUI;

    [Header("Boss Animations")]
    [SerializeField] protected string idleAnimation = "Idle";
    [SerializeField] protected string walkAnimation = "Walk";
    [SerializeField] protected string risingAnimation = "Rise";
    [SerializeField] protected string fallingAnimation = "Fall";
    [SerializeField] protected string deadAnimation = "Die";

    private void Start() {

        // Set bossUI
        bossHealthBarUI = GameObject.Find("Boss Health Bar").GetComponent<BossHealthBarUI>();
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
            if (Mathf.Abs(body.velocity.x) > 0.1f)
                animationHandler.changeAnimationState(walkAnimation);
            else
                animationHandler.changeAnimationState(idleAnimation);
        }
    }

    private void OnDestroy() {
        // If the target was the player, then disalbe the UI
        if (target != null && target.TryGetComponent(out Player player)) {
            bossHealthBarUI.setBoss(null);
        }
    }
}

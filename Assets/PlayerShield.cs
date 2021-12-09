using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShield : Shield
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [SerializeField] private Slider slider;
    [SerializeField] private GameObject sparkle;

    [SerializeField] private float perfectBlockTimeFrame = 0.5f;
    [SerializeField] private float perfectBlockTimer;

    [SerializeField] private int baseBlockDrain;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void FixedUpdate()
    {
        if (perfectBlockTimer > 0)
            perfectBlockTimer -= Time.deltaTime;
    }

    public override void blockDamage(Damage dmg)
    {
        GameManager.instance.CreatePopup("BLOCKED!", transform.position);

        // If player manages to block an attack during the perfect block time frame, then they don't take extra block stamina damage
        if (perfectBlockTimer > 0)
        {
            takeStaminaDamage(0); // 0 extra drain 
            Instantiate(sparkle, new Vector2(transform.position.x + 0.33f, transform.position.y + 0.25f), Quaternion.identity);
            GameEvents.current.triggerOnPerfectBlock(); // Trigger perfect block event for everyone who cares
        }
        else
            takeStaminaDamage(dmg.damageAmount); // Drain stamina

        // Knock holder back
        GetComponentInParent<Rigidbody2D>().AddForce(dmg.origin * 4);
    }

    private void takeStaminaDamage(int damage)
    {
        // Drains stamina and gets result about what to do to player
        damage -= GetComponentInParent<CombatStats>().defense / 4; // Reduce stamina damage by fourth of defense value
        if (damage < 1)
            damage = 1;

        var stamina = GetComponentInParent<Stamina>();
        if (stamina != null && !stamina.forceddrainStamina(baseBlockDrain + damage))
        {
            GameManager.instance.CreatePopup("Your posture has broken!", transform.position);

            // Do stun here
        }
    }

    public override void raiseShield()
    {
        isActive = true;
        perfectBlockTimer = perfectBlockTimeFrame; // Reset perfect block timer on enable
        spriteRenderer.enabled = true;
    }

    public override void lowerShield()
    {
        isActive = false;
        spriteRenderer.enabled = false;
    }

}

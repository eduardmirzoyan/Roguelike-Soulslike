using UnityEngine;
using UnityEngine.UI;

public class PlayerShield : Shield
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [SerializeField] private Slider slider;
    [SerializeField] private GameObject sparkle;

    [SerializeField] private InputBuffer inputBuffer;

    [SerializeField] private float perfectBlockTimeFrame = 0.5f;
    [SerializeField] private float perfectBlockTimer;

    [SerializeField] private int baseBlockDrain;

    [SerializeField] private float blockTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputBuffer = GetComponentInParent<InputBuffer>();
    }

    protected void FixedUpdate()
    {
        if (perfectBlockTimer > 0)
            perfectBlockTimer -= Time.deltaTime;

        if(blockTime > 0)
        {
            GetComponentInParent<Movement>().dashWithVelocity(10, 1); // Fix this logic
            blockTime -= Time.deltaTime;
        }
        else
        {
            GetComponentInParent<Movement>().dashWithVelocity(10, inputBuffer.moveDirection);
        }
    }

    public override void blockDamage(Damage dmg)
    {
        GameManager.instance.CreatePopup("BLOCKED!", transform.position);

        // If player manages to block an attack during the perfect block time frame, then they don't take extra block stamina damage
        if (perfectBlockTimer > 0)
        {
            takeBlockStaminaDamage(0); // 0 extra drain 
            Instantiate(sparkle, new Vector2(transform.position.x + 0.33f, transform.position.y + 0.25f), Quaternion.identity);
            GameEvents.current.triggerOnPerfectBlock(); // Trigger perfect block event for everyone who cares
        }
        else
            takeBlockStaminaDamage(dmg.damageAmount); // Drain stamina

        // Knock holder back
        blockTime = 0.25f;
    }

    private void takeBlockStaminaDamage(int damage)
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

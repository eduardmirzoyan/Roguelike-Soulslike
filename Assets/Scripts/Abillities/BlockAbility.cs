using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockAbility : Ability 
{
    [SerializeField] private GameObject shield;
    [SerializeField] private bool isActive;
    [SerializeField] private Stamina stamina;

    [SerializeField] private int drainAmount = 5;
    private float staminaDrainTimer;
    private PlayerShield playerShield;

    public override void instantiate(GameObject parent)
    {
        // Create the shield object as a child of the player
        playerShield = Instantiate(shield, new Vector3(parent.transform.position.x, parent.transform.position.y - 0.1f, parent.transform.position.z), Quaternion.identity, parent.transform).GetComponent<PlayerShield>();
        stamina = parent.GetComponent<Stamina>();

        base.instantiate(parent);
    }

    public override void performAfterChargeUp(GameObject parent)
    {
        parent.GetComponentInChildren<Shield>().raiseShield();
        parent.GetComponent<AnimationHandler>().changeAnimationState("Defend");
        parent.GetComponent<Movement>().Stop();
        staminaDrainTimer = 1f;

        base.performAfterChargeUp(parent);
    }

    public override void performDuringActive(GameObject parent)
    {
        parent.GetComponent<CombatHandler>().getUtilityAbilityHolder().refreshActiveTime();

        if (playerShield.blockTime <= 0 && Input.GetKeyUp(KeyCode.D))
        {
            parent.GetComponent<CombatHandler>().getUtilityAbilityHolder().finishActiveTime();
        }

        if (staminaDrainTimer > 0)
            staminaDrainTimer -= Time.deltaTime;
        else
        {
            stamina.drainStamina(drainAmount);
            staminaDrainTimer = 1f;
        }

        base.performDuringActive(parent);
    }

    public override void performAfterActive(GameObject parent)
    {
        parent.GetComponentInChildren<Shield>().lowerShield();
        base.performAfterActive(parent);
    }

    public override void uninstantiate(GameObject parent)
    {
        Destroy(parent.GetComponent<Shield>().gameObject);

        base.uninstantiate(parent);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockAbility : Ability 
{
    [SerializeField] private GameObject shield;
    [SerializeField] private bool isActive;
    [SerializeField] private Player player;

    [SerializeField] private int drainAmount = 5;
    private float staminaDrainTimer;

    public override void instantiate(GameObject parent)
    {
        // Create the shield object as a child of the player
        Instantiate(shield, new Vector3(parent.transform.position.x, parent.transform.position.y - 0.1f, parent.transform.position.z), Quaternion.identity, parent.transform);
        player = parent.GetComponent<Player>();

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
        //parent.GetComponent<Movement>().Stop();

        if (staminaDrainTimer > 0)
            staminaDrainTimer -= Time.deltaTime;
        else
        {
            player.GetComponent<Stamina>().drainStamina(drainAmount);
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

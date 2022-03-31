using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Spikes Immunity")]
public class SpikeImmunityEnchantment : Enchantment
{
    private SpikeHandler spikeHandler;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);

        spikeHandler = entity.GetComponent<SpikeHandler>();
        if (spikeHandler != null) {
            spikeHandler.setImmune(true);
        }
    }

    public override void unintialize()
    {
        if (spikeHandler != null) {
            spikeHandler.setImmune(false);
        }
        spikeHandler = null;

        base.unintialize();
    }
}

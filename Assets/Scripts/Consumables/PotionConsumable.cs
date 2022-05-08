using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable/Potion")]
public class PotionConsumable : ConsumableItem
{
    public BaseEffect baseEffect;

    public override void consume(GameObject consumer)
    {
        var consumerEffectable = consumer.GetComponent<EffectableEntity>();
        if (consumerEffectable != null)
        {
            PopUpTextManager.instance.createPopup("You take a sip of the potion...", Color.white, consumer.gameObject.transform.position);
            consumerEffectable.addEffect(baseEffect.InitializeEffect(consumer));
        }
    }
}

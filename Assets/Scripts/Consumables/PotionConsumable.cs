using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PotionConsumable : ConsumableItem
{
    public BaseEffect baseEffect;

    public override void consume(GameObject consumer)
    {
        var consumerEffectable = consumer.GetComponent<EffectableEntity>();
        if (consumerEffectable != null)
        {
            GameManager.instance.CreatePopup("You take a sip of the potion...", consumer.gameObject.transform.position);
            consumerEffectable.addEffect(baseEffect.InitializeEffect(consumer));
        }
    }
}

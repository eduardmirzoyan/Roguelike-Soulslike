using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ChickenConsumable : ConsumableItem
{
    public float healPercentage;

    public override void consume(GameObject consumer)
    {
        var consumerHealth = consumer.GetComponent<Health>();
        if(consumerHealth != null)
        {
            var healAmount = (int)(consumerHealth.maxHealth * healPercentage);
            PopUpTextManager.instance.createVerticalPopup("+" + healAmount + "HP", Color.green, consumer.gameObject.transform.position);
            consumerHealth.increaseHealth((int)(consumerHealth.maxHealth * healPercentage));
        }
    }
}

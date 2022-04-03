using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Gold On Hit Melee")]
public class GoldOnHitMeleeEnchantment : MeleeEchantment
{
    [SerializeField] private float goldGainRatio;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        GameEvents.instance.onHit += giveGoldOnHit;
    }

    public override void unintialize()
    {
        GameEvents.instance.onHit -= giveGoldOnHit;

        base.unintialize();
    }

    private void giveGoldOnHit(GameObject attackingEnitiy, GameObject hitEntity, int damageTaken) {
        if (attackingEnitiy == entity.transform.gameObject && damageTaken > 1) {
            var goldGenerated = (int) (damageTaken * goldGainRatio);
            Debug.Log("You gained: " + goldGenerated + " gold.");
            GameManager.instance.addGold(goldGenerated);
        }
    }
}

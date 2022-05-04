using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Gold On Hit Melee")]
public class GoldOnHitMeleeEnchantment : MeleeEchantment
{
    [SerializeField] private float goldGainRatio;

    private GameObject wielder;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        wielder = weaponGameObject.transform.parent.gameObject;

        GameEvents.instance.onHit += giveGoldOnHit;
    }

    public override void unintialize()
    {
        GameEvents.instance.onHit -= giveGoldOnHit;

        base.unintialize();
    }

    private void giveGoldOnHit(GameObject attackingEnitiy, GameObject hitEntity, int damageTaken) {
        if (attackingEnitiy == wielder && damageTaken > 1) {
            var goldGenerated = (int) (damageTaken * goldGainRatio);

            // Min gold gain is 1
            goldGenerated = Mathf.Max(goldGenerated, 1);

            GameManager.instance.addGold(goldGenerated);

            // Create popup
            PopUpTextManager.instance.createWeakVertPopup("+" + goldGenerated + " G", Color.yellow, attackingEnitiy.transform.position, 0.5f);
        }
    }
}

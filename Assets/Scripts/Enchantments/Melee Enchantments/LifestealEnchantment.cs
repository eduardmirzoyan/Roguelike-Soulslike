using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Life Steal Melee")]
public class LifestealEnchantment : MeleeEchantment
{
    [SerializeField] private float lifestealRatio;
    private Health wielderHealth;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        wielderHealth = entity.transform.parent.GetComponent<Health>();

        GameEvents.instance.onHit += giveHealthOnHit;
    }

    public override void unintialize()
    {
        GameEvents.instance.onHit -= giveHealthOnHit;
        wielderHealth = null;
        base.unintialize();
    }

    private void giveHealthOnHit(GameObject attackingEnitiy, GameObject hitEntity, int damageTaken) {
        if (wielderHealth != null && attackingEnitiy == wielderHealth.gameObject && damageTaken > 1) {
            // Calculate heal amount
            var lifeGained = (int) (damageTaken * lifestealRatio);

            if (lifeGained > 0) {
                wielderHealth.increaseHealth(lifeGained);
                // Create popup
                PopUpTextManager.instance.createPopup("+" + lifeGained, Color.green, wielderHealth.transform.position, 0.75f);
            }
        }
    }
}

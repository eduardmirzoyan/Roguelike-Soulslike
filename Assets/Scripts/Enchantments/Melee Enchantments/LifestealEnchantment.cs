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

        GameEvents.instance.onHit += giveGoldOnHit;
    }

    public override void unintialize()
    {
        GameEvents.instance.onHit -= giveGoldOnHit;
        wielderHealth = null;
        base.unintialize();
    }

    private void giveGoldOnHit(GameObject attackingEnitiy, GameObject hitEntity, int damageTaken) {
        if (wielderHealth != null && attackingEnitiy == entity.transform.gameObject && damageTaken > 1) {
            var lifeGained = (int) (damageTaken * lifestealRatio);
            wielderHealth.increaseHealth(lifeGained);
            Debug.Log("You healed: " + lifeGained + " hp.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Summon Spririts on Crit")]
public class SpiritsOnCritEnchantment : MeleeEchantment
{
    [SerializeField] private int amountOfSpirits = 3;
    [SerializeField] private float damageRatio = 0.1f;
    [SerializeField] private GameObject spiritPrefab;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);

        // Get the weapon
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        // Subscribe to event
        GameEvents.instance.onCrit += summonSpirits;
    }

    public override void unintialize()
    {
        // Unsubscribe to event
        GameEvents.instance.onCrit -= summonSpirits;
        // Remove weapon
        meleeWeapon = null;

        base.unintialize();
    }

    private void summonSpirits(Weapon weapon, Transform target) {
        if (weapon == meleeWeapon) {
            // Calculate damage of spirits
            int damage = (int) (weapon.getOwner().damage * damageRatio);
            // Min damage is 1
            damage = Mathf.Max(damage, 1);

            // Summon entities
            for (int i = 0; i < amountOfSpirits; i++)
            {
                var spirit = Instantiate(spiritPrefab, weapon.transform.position, Quaternion.identity).GetComponent<SpiritProjectile>();
                
                spirit.intialize(damage, target, entity.transform);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Restore Stamina On Crit")]
public class RestoreStaminaOnCritEnchantment : MeleeEchantment
{
    [SerializeField] private float restorePercent = 0.5f;
    private Stamina stamina;
    private MeleeWeapon meleeWeapon;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        stamina = weaponGameObject.GetComponent<Stamina>();
        if (stamina != null) {
            GameEvents.instance.onCrit += restoreStaminaOnCrit;
        }
    }

    public override void unintialize()
    {
        if (stamina != null) {
            GameEvents.instance.onCrit -= restoreStaminaOnCrit;
        }
        stamina = null;
        meleeWeapon = null;
        base.unintialize();
    }

    private void restoreStaminaOnCrit(Weapon weapon, Transform target) {
        if (weapon == meleeWeapon) {
            // Restore stamina based on the size of the weapon, raw
            int amountToRestore = (int) (weapon.getOwner().rawStaminaCost() * restorePercent);
            stamina.restoreStamina(amountToRestore);
        }
    }
}
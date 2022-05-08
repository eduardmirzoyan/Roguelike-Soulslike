using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Melee Enchantment/Ramp on Hit")]
public class RampingDamageEnchantment : MeleeEchantment
{
    [SerializeField] private float bonusPerHit = 0.1f;
    [SerializeField] private float resetBufferTime = 1.5f;
    private int hitCount = 0;
    private MeleeWeapon meleeWeapon;
    private WeaponItem weaponItem;
    private int originalDamage;
    private Coroutine resetRoutine;

    // Get weapon's gameobject
    public override void intialize(GameObject weaponGameObject)
    {
        base.intialize(weaponGameObject);
        meleeWeapon = weaponGameObject.GetComponentInChildren<MeleeWeapon>();
        weaponItem = meleeWeapon.getOwner();
        originalDamage = weaponItem.damage;
        GameEvents.instance.onWeaponHit += increaseWeaponDamage;
    }

    public override void unintialize()
    {
        GameEvents.instance.onWeaponHit -= increaseWeaponDamage;
        meleeWeapon = null;
        weaponItem = null;
        base.unintialize();
    }

    private void increaseWeaponDamage(Weapon weapon, GameObject hitEntity) {
        if (weapon == meleeWeapon) {
            // Increase damge
            hitCount++;
            weaponItem.damage = (int) (originalDamage * (1 + hitCount * bonusPerHit));

            // wait for reset
            if (resetRoutine != null) {
                weapon.StopCoroutine(resetRoutine);
            }
            resetRoutine = weapon.StartCoroutine(resetHitCount(resetBufferTime));
        }
    }

    private IEnumerator resetHitCount(float duration) {
        yield return new WaitForSeconds(duration);
        hitCount = 0;
        weaponItem.damage = originalDamage;
    }
}

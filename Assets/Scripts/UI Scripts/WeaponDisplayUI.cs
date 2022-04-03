using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplayUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Slider slider;
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private CombatHandler combatHandler;
    [SerializeField] private WeaponItem currentWeaponItem;
    [SerializeField] private EquipmentHandler equipmentHandler;
    [SerializeField] private bool displayMainHand;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = 0;
        
        var player = GameObject.Find("Player");
        combatHandler = player.GetComponent<CombatHandler>();
        equipmentHandler = player.GetComponent<EquipmentHandler>();

        // Initalize components
        currentWeapon = null;
        currentWeaponItem = null;
        image.enabled = false;

        // Subscribe to weaponchange
        GameEvents.instance.onWeaponChange += changeWeapon;
    }

    // Update is called once per frame
    private void Update()
    {
        // Update cooldown
        if (currentWeapon != null)
            slider.value = currentWeapon.getCooldownRatio();
    }

    private void changeWeapon(Weapon weapon, bool onMainHand) {
        // If weapon is equipped on the hand we don't care about then skip
        if (displayMainHand != onMainHand)
            return;

        // If the weapon hasn't changed, then skip
        if (currentWeapon == weapon)
            return;
        
        // Change the image based on the new weapon
        if (weapon == null) {
            image.enabled = false;
            slider.value = 0;
        }
        else {
            image.enabled = true;
            image.sprite = weapon.GetComponent<SpriteRenderer>().sprite;
        }

        // Set new current weapon
        currentWeapon = weapon;
    }
}

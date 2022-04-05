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
    [SerializeField] private AmmoCountUI ammoCountUI;
    [SerializeField] private bool displayMainHand;

    private void Start()
    {
        ammoCountUI = GetComponentInChildren<AmmoCountUI>();

        slider = GetComponent<Slider>();
        slider.value = 0;
        
        var player = GameObject.Find("Player");
        combatHandler = player.GetComponent<CombatHandler>();

        // Initalize components
        currentWeapon = null;
        image.enabled = false;

        // Subscribe to weaponchange
        GameEvents.instance.onWeaponChange += changeWeapon;
    }

    // Update is called once per frame
    private void Update()
    {
        // Update cooldown
        if (currentWeapon != null) {
            slider.value = currentWeapon.getCooldownRatio();
        }
            
    }

    private void changeWeapon(Weapon weapon, bool onMainHand) {
        // If weapon is equipped on the hand we don't care about then skip
        if (displayMainHand != onMainHand) {
            // If the new weapon is null, then dip
            if (weapon == null) {
                // Set image to false
                image.enabled = false;
                return;
            }
            // If the weapon is equipped to the off-hand, but this display's main-hand
            if(displayMainHand && !onMainHand) {
                if (combatHandler.getOffHandWeapon().getOwner().twoHanded) {
                    image.enabled = true;
                    image.sprite = combatHandler.getOffHandWeapon().GetComponent<SpriteRenderer>().sprite;
                    slider.value = 0;

                    // Change transparancy
                    var color = image.color;
                    color.a = 0.5f;
                    image.color = color;
                }
            } // If the weapon is equipped to the main-hand, but this display's off-hand
            else if(!displayMainHand && onMainHand) {
                if (combatHandler.getMainHandWeapon().getOwner().twoHanded) {
                    image.enabled = true;
                    image.sprite = combatHandler.getMainHandWeapon().GetComponent<SpriteRenderer>().sprite;
                    slider.value = 0;

                    // Change transparancy
                    var color = image.color;
                    color.a = 0.5f;
                    image.color = color;
                }
            }

            return;
        }
            

        // If the weapon hasn't changed, then skip
        if (currentWeapon == weapon)
            return;
        
        // Change the image based on the new weapon
        if (weapon == null) {
            image.enabled = false;
            slider.value = 0;
            ammoCountUI.trackAmmo(false);
        }
        else {
            image.enabled = true;
            image.sprite = weapon.GetComponent<SpriteRenderer>().sprite;

            // Change transparancy
            var color = image.color;
            color.a = 1f;
            image.color = color;

            if (weapon.GetComponent<RangedWeapon>() != null) {
                ammoCountUI.trackAmmo(true);
            }
            else {
                ammoCountUI.trackAmmo(false);
            }
        }

        // Set new current weapon
        currentWeapon = weapon;
    }
}

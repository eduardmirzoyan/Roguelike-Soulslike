using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplayUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Cooldownbar cooldownbar;
    [SerializeField] private WeaponItem currentWeapon;
    [SerializeField] private EquipmentHandler playerEquipment;
    [SerializeField] private bool displayMainHand;

    private void Start()
    {
        playerEquipment = GameObject.Find("Player").GetComponent<EquipmentHandler>();
        image.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        changeWeaponDisplay();
    }

    private void changeWeaponDisplay() {
        
        // Check witch weapon to display
        if (displayMainHand) {
            // If the weapon hasn't changed, then dip
            if (currentWeapon == playerEquipment.getMainHandWeaponItem())
                return;
            
            // Set the new weapon
            currentWeapon = playerEquipment.getMainHandWeaponItem();

            // Change the image based on the new weapon
            if (currentWeapon == null) {
                image.enabled = false;
                cooldownbar.setMaxCooldown(0);
            }
            else {
                image.enabled = true;
                image.sprite = playerEquipment.getMainHandWeaponItem().sprite;
            }

        } // Offhand
        else {
            // If the weapon hasn't changed, then dip
            if (currentWeapon == playerEquipment.getOffHandWeaponItem())
                return;
            
            // Set the new weapon
            currentWeapon = playerEquipment.getOffHandWeaponItem();

            // Change the image based on the new weapon
            if (currentWeapon == null) {
                image.enabled = false;
                cooldownbar.setMaxCooldown(0);
            }
            else {
                image.enabled = true;
                image.sprite = playerEquipment.getOffHandWeaponItem().sprite;
            }
            
        }
    }
}

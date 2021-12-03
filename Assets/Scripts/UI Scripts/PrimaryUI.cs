using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Cooldownbar cooldownbar;
    [SerializeField] private WeaponItem currentWeapon;

    [SerializeField] private EquipmentHandler playerEquipment;

    private void Start()
    {
        playerEquipment = GameObject.Find("Player").GetComponent<EquipmentHandler>();
        image.enabled = false;
        //GameEvents.current.onPlayerEquippedWeapon += updateWeapon;
        GameEvents.current.onPlayerIsLightAttack += onWeaponAttack;
        GameEvents.current.onPlayerIsHeavyAttack += onWeaponAttack;
    }

    // Update is called once per frame
    private void Update()
    {
        currentWeapon = playerEquipment.getEquippedWeaponItem() ?? null;
        if(currentWeapon != null)
        {
            image.enabled = true;
            image.sprite = playerEquipment.getEquippedWeaponItem().sprite;
            if (playerEquipment.GetComponentInChildren<Weapon>().isActive())
                cooldownbar.setCooldown(playerEquipment.GetComponentInChildren<Weapon>().getActiveTime());
            else
                cooldownbar.setCooldown(0);
        }
        else
        {
            image.enabled = false;
            currentWeapon = null;
            cooldownbar.setMaxCooldown(0);
        }
    }

    private void onWeaponAttack(bool hasStarted)
    {
        if (hasStarted)
            cooldownbar.setMaxCooldown(playerEquipment.GetComponentInChildren<Weapon>().getActiveTime());
    }
}

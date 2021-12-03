using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DisplayEquipment
{
    Helmet,
    Gloves,
    Cape,
    Chestplate,
    Boots,
    Weapon
}
public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private DisplayEquipment display;
    [SerializeField] private EquipmentHandler playerEquipment;

    private void Start()
    {
        playerEquipment = GameObject.Find("Player").GetComponent<EquipmentHandler>();
        icon.sprite = null;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (display)
        {
            case DisplayEquipment.Helmet:
                if (playerEquipment.equippedArmor[(int)EquipmentSlot.Helmet] != null)
                {
                    icon.sprite = playerEquipment.equippedArmor[(int)EquipmentSlot.Helmet].sprite;
                }
                else
                {
                    icon.sprite = null;
                }
                break;
            case DisplayEquipment.Gloves:
                if (playerEquipment.equippedArmor[(int)EquipmentSlot.Gloves] != null)
                {
                    icon.sprite = playerEquipment.equippedArmor[(int)EquipmentSlot.Gloves].sprite;
                }
                else
                {
                    icon.sprite = null;
                }
                break;
            case DisplayEquipment.Cape:
                if (playerEquipment.equippedArmor[(int)EquipmentSlot.Cape] != null)
                {
                    icon.sprite = playerEquipment.equippedArmor[(int)EquipmentSlot.Cape].sprite;
                }
                else
                {
                    icon.sprite = null;
                }
                break;
            case DisplayEquipment.Chestplate:
                if(playerEquipment.equippedArmor[(int)EquipmentSlot.Chestpiece] != null)
                {
                    icon.sprite = playerEquipment.equippedArmor[(int)EquipmentSlot.Chestpiece].sprite;
                }
                else
                {
                    icon.sprite = null;
                }
                break;
            case DisplayEquipment.Boots:
                if (playerEquipment.equippedArmor[(int)EquipmentSlot.Boots] != null)
                {
                    icon.sprite = playerEquipment.equippedArmor[(int)EquipmentSlot.Boots].sprite;
                }
                else
                {
                    icon.sprite = null;
                }
                break;
            case DisplayEquipment.Weapon:
                if(playerEquipment.getEquippedWeaponItem() != null)
                {
                    icon.sprite = playerEquipment.getEquippedWeaponItem().sprite;
                }
                else
                {
                    icon.sprite = null;
                }
                break;
        }
    }
}

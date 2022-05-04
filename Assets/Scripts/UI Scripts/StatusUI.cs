using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Player player;
    [SerializeField] private Health health;
    [SerializeField] private Stamina stamina;
    [SerializeField] private Stats stats;
    [SerializeField] private EquipmentHandler equipmentHandler;
    [SerializeField] private EnchantableEntity enchantableEntity;
    [SerializeField] private List<GameObject> enchantmentHolders;
    [SerializeField] private List<Enchantment> allEnchantments;

    [Header("Status Fields")]
    [SerializeField] private Text basicStatsLeft;
    [SerializeField] private Text basicStatsRight;
    [SerializeField] private Text combatStatusLeft;
    [SerializeField] private Text combatStatusRight;

    [SerializeField] private GameObject enchantmentHolderPrefab;
    [SerializeField] private VerticalLayoutGroup mainWeaponVerticalLayoutGroup;
    [SerializeField] private VerticalLayoutGroup offWeaponVerticalLayoutGroup;
    [SerializeField] private VerticalLayoutGroup armorVerticalLayoutGroup;

    [Header("Selection")]
    [SerializeField] private int selectedItemIndex = -1;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameManager.instance.GetPlayer();
        enchantableEntity = player.GetComponent<EnchantableEntity>();
        equipmentHandler = player.GetComponent<EquipmentHandler>();
        health = player.GetComponent<Health>();
        stamina = player.GetComponent<Stamina>();
        stats = player.GetComponent<Stats>();

        selectedItemIndex = -1;
    }

    public void updateUI() {
        selectedItemIndex = -1;
        updateBasicStatus();
        updateCombatStatus();
        updateEnchantmentsUI();
    }

    public void clearUI() {
        removeEnchantmentsUI();
    }
    
    private void updateBasicStatus() {
        basicStatsLeft.text = 
            "Player: " + player.name
        + "\nLevel: " + GameManager.instance.level
        + "\nExperience: " + GameManager.instance.getExperienceStatus()
        + "\nSkill Points: " + GameManager.instance.skillpoints;

        basicStatsRight.text = 
            "Health: " + health.getStatus()
        + "\nStamina: " + stamina.getStatus()
        + "\nGold: " + GameManager.instance.gold
        + "\n(TBD): ?";
    }

    private void updateCombatStatus() {
        combatStatusLeft.text 
        = "Armor: " + stats.defense
        + "\nGlobal Crit: " + stats.percentCritChance * 100 +  "%"
        + "\nDmg Bonus: " + stats.damageDealtMultiplier * 100 + "%"
        + "\nDmg Reduction: " + stats.damageTakenMultiplier * 100 + "%";

        combatStatusRight.text 
        = "Dodge Chance: " + stats.percentDodgeChance * 100 + "%"
        + "\nMovespeed Bonus: " + stats.movespeedMultiplier * 100 + "%";
    }

    private void updateEnchantmentsUI() {
        // Add main-hand weapon enchantment
        var mainWeaponItem = equipmentHandler.getMainHandWeaponItem();
        if (mainWeaponItem != null && mainWeaponItem.enchantments != null) {
            foreach (Enchantment enchantment in mainWeaponItem.enchantments) {
                var enchantmentHolder = Instantiate(enchantmentHolderPrefab, mainWeaponVerticalLayoutGroup.transform);
                enchantmentHolder.GetComponentInChildren<Text>().text = enchantment.enchantmentName;
                enchantmentHolders.Add(enchantmentHolder);
                allEnchantments.Add(enchantment);
            }
            
        }

        // Add off-hand weapon enchantment
        var offWeaponItem = equipmentHandler.getOffHandWeaponItem();
        if (offWeaponItem != null && offWeaponItem.enchantments != null) {
            foreach (Enchantment enchantment in offWeaponItem.enchantments) {
                var enchantmentHolder = Instantiate(enchantmentHolderPrefab, offWeaponVerticalLayoutGroup.transform);
                enchantmentHolder.GetComponentInChildren<Text>().text = enchantment.enchantmentName;
                enchantmentHolders.Add(enchantmentHolder);
                allEnchantments.Add(enchantment);
            }
        }

        // Add amor enchantments
        var enchantments = enchantableEntity.getEnchantments();
        foreach(var enchantment in enchantments) {
            var enchantmentHolder = Instantiate(enchantmentHolderPrefab, armorVerticalLayoutGroup.transform);
            enchantmentHolder.GetComponentInChildren<Text>().text = enchantment.enchantmentName;
            enchantmentHolders.Add(enchantmentHolder);
            allEnchantments.Add(enchantment);
        }
    }

    private void removeEnchantmentsUI() {
        foreach(var armor in enchantmentHolders) {
            Destroy(armor.gameObject);
        }
        enchantmentHolders.Clear();
        allEnchantments.Clear();
    }

    public void moveSelectedUp(bool up) {
        if (up) {
            selectedItemIndex--;
            if (selectedItemIndex < 0) {
                selectedItemIndex = -1;
            }
        }
        else {
            selectedItemIndex++;
            if (selectedItemIndex > enchantmentHolders.Count - 1) {
                selectedItemIndex = enchantmentHolders.Count - 1;
            }
        }
    }

    public (Transform, Enchantment) getSelectedEnchantment() {
        // If nothing is selected, return null
        if (selectedItemIndex < 0 || selectedItemIndex > enchantmentHolders.Count) {
            return (null, null);
        }
        // Else return the selected gameobject's transform
        return (enchantmentHolders[selectedItemIndex].transform, allEnchantments[selectedItemIndex]);
    }
}

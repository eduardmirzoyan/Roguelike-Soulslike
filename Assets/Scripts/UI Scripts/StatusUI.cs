using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private Text textbox; // TEMP!
    [SerializeField] private Player player;
    [SerializeField] private Health health;
    [SerializeField] private Stamina stamina;
    [SerializeField] private CombatStats stats;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        textbox = GetComponentInChildren<Text>();
        health = player.GetComponent<Health>();
        stamina = player.GetComponent<Stamina>();
        stats = player.GetComponent<CombatStats>();
    }

    // Update is called once per frame
    private void Update()
    {
        textbox.text = "(I will make the status screen look better later..)\n"
            + "Class: Knight" + "\n"
            + "Level :" + GameManager.instance.level + "\n"
            + "Health: " + health.getStatus() + "\n"
            + "Stamina: " + stamina.getStatus() + "\n"
            + "Defense: " + stats.defense + "\n"
            + "Damage Reduction: " + stats.damageTakenMultiplier * 100 + " % Damage reduced" + "\n"
            + "Damage Increase: " + stats.damageDealtMultiplier * 100 + " % Damage increased" + "\n"
            + "Gold: " + GameManager.instance.gold + "\n"
            + "Experience: " + GameManager.instance.experience + "\n"
            + "Skill Points: " + GameManager.instance.skillpoints;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] private Text healthText;
    [SerializeField] private Health health;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        healthText = GetComponentInChildren<Text>();
    }

    public void setEntity(Health health) {
        this.health = health;
        slider.maxValue = health.getMaxHP();
        slider.value = health.getHP();
        healthText.text = slider.value.ToString() + " / " + slider.maxValue.ToString();
    }

    private void FixedUpdate()
    {
        if (health != null) {
            slider.maxValue = health.getMaxHP();
            slider.value = health.getHP();
            healthText.text = slider.value.ToString() + " / " + slider.maxValue.ToString();
        }
        
    }
}

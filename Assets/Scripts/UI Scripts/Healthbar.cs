using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] private Text healthText;
    [SerializeField] private Health health;

    private void Start()
    {
        healthText = GetComponentInChildren<Text>();
    }

    private void FixedUpdate()
    {
        slider.maxValue = health.getMaxHP();
        slider.value = health.getHP();
        healthText.text = slider.value.ToString() + " / " + slider.maxValue.ToString();
    }
}

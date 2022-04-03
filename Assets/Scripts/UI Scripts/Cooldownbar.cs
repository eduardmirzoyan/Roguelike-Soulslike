using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldownbar : MonoBehaviour
{
    [SerializeField] public Slider slider;

    public void setMaxCooldown(float cooldown)
    {
        slider.maxValue = cooldown;
        slider.value = 0;
    }

    public void setCooldown(float cooldown)
    {
        slider.value = cooldown;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] public Slider slider;
    public void setMaxExperience(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }

    public void setExperience(int stamina)
    {
        slider.value = stamina;
    }
}

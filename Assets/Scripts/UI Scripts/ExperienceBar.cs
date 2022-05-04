using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Text text;

    public void setMaxExperience(int maxExp)
    {
        slider.maxValue = maxExp;
        text.text =  "XP: 0/" + maxExp;
    }

    public void setExperience(int exp)
    {
        slider.value = exp;
        text.text = "XP: " + exp + "/" + slider.maxValue;
    }
}

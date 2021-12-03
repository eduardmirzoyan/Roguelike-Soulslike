using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectBar : MonoBehaviour
{
    [SerializeField] public Image icon;
    [SerializeField] public Image fill;
    [SerializeField] public Slider slider;
    [SerializeField] public Text effectName;

    private void Start()
    {
        slider.maxValue = 100;
    }
}

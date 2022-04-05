using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollDisplayUI : MonoBehaviour
{
    [SerializeField] private RollingHandler rollingHandler;
    [SerializeField] private Slider slider;

    private void Start() {
        // Get the slider
        slider = GetComponentInChildren<Slider>();
    }

    private void Update() {
        slider.value = rollingHandler.getCooldownRatio();
    }

}

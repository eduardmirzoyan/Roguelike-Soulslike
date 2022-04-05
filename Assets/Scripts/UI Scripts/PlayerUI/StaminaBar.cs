using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] private Stamina stamina;
    [SerializeField] private Text staminaText;

    private void Start()
    {
        staminaText = GetComponentInChildren<Text>();
    }

    private void FixedUpdate()
    {
        slider.maxValue = stamina.maxStamina;
        slider.value = stamina.currentStamina;
        staminaText.text = slider.value.ToString() + " / " + slider.maxValue.ToString();
    }

    public void setMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }

    public void setStamina(int stamina)
    {
        slider.value = stamina;
    }
}

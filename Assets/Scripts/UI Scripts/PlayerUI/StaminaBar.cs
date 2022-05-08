using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] private Text staminaText;

    private void Awake() {
        staminaText = GetComponentInChildren<Text>();
    }

    private void Start() {
        GameEvents.instance.onStaminaChange += updateStaminaUI;
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

    private void updateStaminaUI(Stamina stamina) {
        slider.maxValue = stamina.getMax();
        slider.value = stamina.getCurrent();
        staminaText.text = stamina.getStatus();
    }
}

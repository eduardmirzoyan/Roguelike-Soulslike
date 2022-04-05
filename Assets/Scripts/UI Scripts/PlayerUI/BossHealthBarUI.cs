using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] private Health bossHealth;
    [SerializeField] private Image[] images;

    private void Awake() {
        bossHealth = null;
        images = GetComponentsInChildren<Image>();
        enableImages(false);
    }

    private void FixedUpdate()
    {
        if(bossHealth != null)
        {
            slider.value = bossHealth.getHP();
        }
    }

    public void setBoss(BossAI boss)
    {
        bossHealth = boss.GetComponent<Health>();
        if (bossHealth != null) {
            // Set values
            slider.maxValue = bossHealth.getMaxHP();
            slider.value = bossHealth.getHP();
            
            // Enable visuals
            enableImages(true);
        }
    }

    private void enableImages(bool enable) {
        foreach (var image in images) {
            image.enabled = enable;
        }
    }
}

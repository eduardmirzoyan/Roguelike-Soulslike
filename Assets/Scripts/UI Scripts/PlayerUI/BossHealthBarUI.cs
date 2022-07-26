using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] private Health bossHealth;
    [SerializeField] private StatusEffectsUI statusEffectsUI;
    [SerializeField] private Image[] images;
    
    private void Awake() {
        bossHealth = null;
        images = GetComponentsInChildren<Image>();
        statusEffectsUI = GetComponentInChildren<StatusEffectsUI>();
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
        var src = GameObject.Find("Audio Manager").GetComponent<AudioSource>();

        // If boss is null, the disable
        if (boss == null) {
            enableImages(false);
            if (src != null)
                src.Stop();
            return;
        }
        
        bossHealth = boss.GetComponent<Health>();
        if (bossHealth != null) {
            // Set values
            slider.maxValue = bossHealth.getMaxHP();
            slider.value = bossHealth.getHP();
            
            // Set status UI
            statusEffectsUI.setEntity(bossHealth.GetComponent<EffectableEntity>());

            // Enable visuals
            enableImages(true);
        }

        // Start boss music if possible
        if (src != null)
            src.Play();
    }

    private void enableImages(bool enable) {
        foreach (var image in images) {
            image.enabled = enable;
        }
    }
}

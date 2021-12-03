using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private Image[] effectIconHolder; // Holds any number of effects you may want
    [SerializeField] private EffectableEntity playerEffectable;

    private void Start()
    {
        effectIconHolder = GetComponentsInChildren<Image>();
    }

    private void FixedUpdate()
    {
        updateEffects();
    }

    public void updateEffects()
    {
        int count = 0;
        foreach (var effect in playerEffectable.GetKeyValuePairs().Values.ToList())
        {
            effectIconHolder[count].enabled = true;
            effectIconHolder[count].sprite = effect.icon;
            if (effect.getStacks() > 1)
                effectIconHolder[count].GetComponentInChildren<Text>().text = "x" + effect.getStacks().ToString();
            else
                effectIconHolder[count].GetComponentInChildren<Text>().text = "";

            count++;
        }
        while(count < effectIconHolder.Length)
        {
            effectIconHolder[count].enabled = false;
            effectIconHolder[count].GetComponentInChildren<Text>().text = "";
            count++;
        }
    }
}

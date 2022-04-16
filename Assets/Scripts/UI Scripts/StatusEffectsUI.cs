using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private Dictionary<TimedEffect, GameObject> effectHolderDict; // Holds any number of effects you may want
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private EffectableEntity effectableEntity;
    [SerializeField] private GameObject effectHolderPrefab;

    private void Awake()
    {
        // Intialize
        horizontalLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();

        // Intialize dictionary
        effectHolderDict = new Dictionary<TimedEffect, GameObject>();
    }

    private void Start() {
        // Subscribe to events
        GameEvents.instance.onAddStatusEffect += addStatusEffect;
        GameEvents.instance.onRemoveStatusEffect += removeStatusEffect;
    }

    public void addStatusEffect(TimedEffect timedEffect, EffectableEntity effectableEntity) {
        // If this is the entity getting the effect
        if (effectableEntity == this.effectableEntity) {
            // If the effect is already in the dictionary, then increment its stacks
            foreach (var effect in effectHolderDict.Keys.ToList()) {
                if (effect.GetType() == timedEffect.GetType()) {
                    // Display the stacks
                    effectHolderDict[effect].gameObject.GetComponentInChildren<Text>().text = "x" + effect.getStacks().ToString();
                    // Exit function
                    return;
                }
            }

            // Create holder gameobject as child
            var holderObject = Instantiate(effectHolderPrefab, horizontalLayoutGroup.transform);
            // Set Sprite
            holderObject.GetComponent<Image>().sprite = timedEffect.icon;

            // Set stacks
            if (timedEffect.getStacks() > 1)
                holderObject.GetComponentInChildren<Text>().text = "x" + timedEffect.getStacks().ToString();
            else
                holderObject.GetComponentInChildren<Text>().text = "";

            // Save reference
            effectHolderDict.Add(timedEffect, holderObject);
        }
        // else {
        //     print("effectable entity did not match: " + effectableEntity.name + " vs " + this.effectableEntity.gameObject.name);
        // }
    }

    public void removeStatusEffect(TimedEffect timedEffect, EffectableEntity effectableEntity) {
        // If this is the entity removing the effect
        if (effectableEntity == this.effectableEntity) {
            if (effectHolderDict.ContainsKey(timedEffect)) {
                // Destroy gameobject
                Destroy(effectHolderDict[timedEffect].gameObject);

                // Remove from dictionary
                effectHolderDict.Remove(timedEffect);
                
            }
            else {
                print("Effect to remove not found: " + timedEffect.ToString());
            }
        }
    }
}

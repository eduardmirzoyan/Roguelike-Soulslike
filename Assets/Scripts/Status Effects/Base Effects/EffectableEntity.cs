using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectableEntity : MonoBehaviour
{
    [SerializeField] private Dictionary<BaseEffect, TimedEffect> activeEffects = new Dictionary<BaseEffect, TimedEffect>();
    private bool isPaused;

    private void FixedUpdate()
    {
        if (isPaused)
            return;

        // For every item in the dictionary, tick its effect
        foreach (var effect in activeEffects.Values.ToList())
        {
            effect.Tick();
            if (effect.IsFinished)
            {
                // Remove effect
                activeEffects.Remove(effect.Effect);
                // Trigger event
                GameEvents.instance.triggerRemoveStatusEffect(effect, this);
            }
        }
    }

    public void addEffect(TimedEffect timedEffect)
    {
        print("added effect to: " + gameObject.name);
        if (activeEffects.ContainsKey(timedEffect.Effect))
        {
            activeEffects[timedEffect.Effect].Activate();
        }
        else
        {
            // Add effect
            activeEffects.Add(timedEffect.Effect, timedEffect);
            // Activate effect
            timedEffect.Activate();
        }

        // Trigger event
        GameEvents.instance.triggerAddStatusEffect(timedEffect, this);
    }

    public bool removeEffect(BaseEffect baseEffect) {
        if (activeEffects == null)
            return false;

        // Find an effect with the same type
        foreach (var effect in activeEffects.Keys.ToList()) {
            if (effect.GetType() == baseEffect.GetType()) {
                // Force end the effect
                activeEffects[effect].End();

                // Then remove it
                activeEffects.Remove(effect);
                return true;
            }
        }
        return false;
    }

    public Dictionary<BaseEffect, TimedEffect> GetKeyValuePairs()
    {
        return activeEffects;
    }
}

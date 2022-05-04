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

    public TimedEffect removeEffect(BaseEffect baseEffect) {
        // Find an effect with the same type
        foreach (var effect in activeEffects.Keys.ToList()) {
            if (effect.GetType() == baseEffect.GetType()) {
                // Cache effect
                var cache = activeEffects[effect];

                // Force end the effect
                activeEffects[effect].End();

                // Trigger event
                GameEvents.instance.triggerRemoveStatusEffect(activeEffects[effect], this);

                // Then remove it
                activeEffects.Remove(effect);

                return cache;
            }
        }
        return null;
    }

    public int getActiveEffectCount() {
        return activeEffects.Count;
    }

    public bool containsEffect(BaseEffect baseEffect) {
        foreach (var bEffect in activeEffects.Keys) {
            if (bEffect.GetType() == baseEffect.GetType()) {
                return true;
            }
        }
        return false;
    }

}

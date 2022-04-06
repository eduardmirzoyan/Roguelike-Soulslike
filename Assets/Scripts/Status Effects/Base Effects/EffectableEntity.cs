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
                activeEffects.Remove(effect.Effect);
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
            activeEffects.Add(timedEffect.Effect, timedEffect);
            timedEffect.Activate();
        }
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

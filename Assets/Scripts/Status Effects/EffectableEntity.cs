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

    public void addEffect(TimedEffect effect)
    {
        if (activeEffects.ContainsKey(effect.Effect))
        {
            activeEffects[effect.Effect].Activate();
            GameManager.instance.CreatePopup(effect.Effect.name, gameObject.transform.position);
        }
        else
        {
            activeEffects.Add(effect.Effect, effect);
            effect.Activate();
        }
    }

    public Dictionary<BaseEffect, TimedEffect> GetKeyValuePairs()
    {
        return activeEffects;
    }

    public int getNumberOfEffects() => activeEffects.Count;
}

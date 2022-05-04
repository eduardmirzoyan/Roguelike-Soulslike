using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedEffect
{
    protected float Duration;
    protected int stacks;
    public BaseEffect Effect;
    protected readonly GameObject Obj;
    public bool IsFinished;
    public Sprite icon;

    private float timer;

    public TimedEffect(BaseEffect effect, GameObject parent)
    {
        Effect = effect;
        Obj = parent;
        icon = effect.icon;
    }

    public void Tick()
    {
        // Every tickRate, apply the effect
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            onTick();
            Duration -= Effect.tickRate;
            
            if (Duration <= 0)
            {
                End();
                IsFinished = true;
            }

            timer = Effect.tickRate;
        }
        
    }

    /**
     * Activates buff or extends duration if ScriptableBuff has IsDurationStacked or IsEffectStacked set to true.
     */
    public void Activate()
    {
        if (stacks < Effect.maxStacks || Duration <= 0)
        {
            ApplyEffect();
            stacks++;
        }

        if (Effect.isDurationStacked || Duration <= 0)
        {
            Duration += Effect.Duration;
            timer = Effect.tickRate;
        }
        else if (Effect.isDurationReset)
        {
            Duration = Effect.Duration;
            timer = Effect.tickRate;
        }

        
    }

    // Logic for what to do if effect is applied
    protected abstract void ApplyEffect();

    // Logic for what to do every tick
    protected abstract void onTick();

    // Logic for what to if effect is removed
    public abstract void End();

    public int getStacks() => stacks;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StatusType
{
    Burn,
    Poison,
    Cold,
    Curse
}

[CreateAssetMenu]
public class BuildupEffect : ScriptableObject
{
    public StatusType type;
    public Sprite icon;
    public Color color;
    public BaseEffect triggerEffect;
    public int buildUpAmount;

    [SerializeField] private float delay = 3f;
    [SerializeField] private float tickDuration = 0.1f;
    [SerializeField] private float tickTimer = 0.1f;

    public void addBuildUp(int amount)
    {
        buildUpAmount += amount;
        delay = 3f;
    }

    public void tick()
    {
        // Delay before decaying
        if(delay > 0) // Count down delay
        {
            delay -= Time.deltaTime;
        }
        else if (tickTimer > 0) // Count down timer
        {
            tickTimer -= Time.deltaTime;
        }
        else // Reduce buildup
        {
            buildUpAmount -= 1;
            tickTimer = tickDuration;
        }
    }
}

using UnityEngine;

public abstract class BaseEffect : ScriptableObject
{
    /// Time duration of the buff in seconds.
    public float Duration;

    /// Duration is reset each time the buff is applied
    public bool isDurationReset;

    /// Duration is increased each time the buff is applied.
    public bool isDurationStacked;

    /// The maximum amount of stacks this buff can reach, -1 means infinite
    public int maxStacks = 1;

    /// How often the effect ticks
    public float tickRate = 1;

    // Icon of the effect for status screen
    public Sprite icon;

    [TextArea(15, 20)]
    public string flavorText;

    public abstract TimedEffect InitializeEffect(GameObject parent);

}
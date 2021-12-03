using UnityEngine;

public abstract class BaseEffect : ScriptableObject
{
    /**
     * Time duration of the buff in seconds.
     */
    public float Duration;

    /**
     * Duration is increased each time the buff is applied.
     */
    public bool IsDurationStacked;

    /**
     * Effect value is increased each time the buff is applied.
     */
    public bool IsEffectStacked;

    public float tickRate = 1;

    // Icon of the effect for status screen
    public Sprite icon;

    [TextArea(15, 20)]
    public string flavorText;

    public abstract TimedEffect InitializeEffect(GameObject parent);

}

// Names: Perk, Attribute, Modifier, Passive, Enchantment
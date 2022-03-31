using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Summon Long Range Familiar")]
public class LongFamiliarEnchantment : Enchantment
{
    [SerializeField] private GameObject familiarObject;
    private FamiliarHandler familiarHandler;
    private Familiar familiarInstance;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        familiarHandler = entity.GetComponent<FamiliarHandler>();
        if (familiarHandler != null)
            familiarInstance = familiarHandler.spawnFamiliar(familiarObject);
        else
            Debug.Log(gameObject.name + " does not have a familiarhandler.");
    }

    public override void unintialize()
    {
        if (familiarHandler != null)
            familiarHandler.despawnFamiliar(familiarInstance);
        base.unintialize();
    }
}

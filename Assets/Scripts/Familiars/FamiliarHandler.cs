using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamiliarHandler : MonoBehaviour
{
    [SerializeField] private Transform gatherPoint;
    [SerializeField] private List<Familiar> familiars = new List<Familiar>();

    public Familiar spawnFamiliar(GameObject familiarObject)
    {
        // Creates the familiar
        var familiar = Instantiate(familiarObject, transform.position, Quaternion.identity).GetComponent<Familiar>();
        if(familiar == null)
            throw new NullReferenceException("A non-familar game object was passed into Spawn Familiar function");

        // Sets the starting data for familiar
        familiar.setOwner(gameObject);
        familiar.setHome(gatherPoint);
        familiar.setPosition(familiars.Count);

        // Add familair to list for easy removal
        familiars.Add(familiar);

        return familiar;
    }

    public void despawnFamiliar(Familiar familiarToRemove)
    {
        foreach (var familiar in familiars)
        {
            if (familiar.gameObject == familiarToRemove.gameObject)
            {
                familiar.Despawn();
                familiars.Remove(familiar);
                break;
            }
        }
        familiars.TrimExcess();
        for (int i = 0; i < familiars.Count; i++)
            familiars[i].setPosition(i);
    }
}

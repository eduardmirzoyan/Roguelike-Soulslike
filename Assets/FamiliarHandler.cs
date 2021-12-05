using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamiliarHandler : MonoBehaviour
{
    [SerializeField] private Transform gatherPoint;
    [SerializeField] private List<GameObject> familiars = new List<GameObject>();

    public void spawnFamiliar(GameObject familiarObject)
    {
        // Creates the familiar
        var familiar = Instantiate(familiarObject, transform.position, Quaternion.identity).GetComponent<Familiar>();
        if(familiar == null)
            throw new NullReferenceException("A non-familar game object was passed into Spawn Familiar function");

        // Sets the starting data for familiar
        familiar.setOwner(gameObject);
        familiar.setHome(gatherPoint);
        //familiar.setOffset(new Vector2(familiars.Count * 0.75f, UnityEngine.Random.Range(-0.1f, 0.1f)));
        familiar.setPosition(familiars.Count);

        // Add familair to list for easy removal
        familiars.Add(familiarObject);
    }
}

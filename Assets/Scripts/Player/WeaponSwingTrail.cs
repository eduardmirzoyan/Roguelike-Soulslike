using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwingTrail : MonoBehaviour
{
    [SerializeField] private GameObject trail;

    public void spawnTrail()
    {
        Instantiate(trail, transform);
    }
}

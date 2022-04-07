using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageParticles : MonoBehaviour
{
    [SerializeField] private GameObject damageParticlesPrefab;

    public void spawnParticles()
    {
        // Creates the particles
        Instantiate(damageParticlesPrefab, transform);
    }
}

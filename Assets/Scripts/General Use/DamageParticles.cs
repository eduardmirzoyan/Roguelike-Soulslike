using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageParticles : MonoBehaviour
{
    [SerializeField] private GameObject damageParticlesPrefab;
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private Transform spawnPoint;

    public void spawnDamageParticles()
    {
        // Creates the particles
        Instantiate(damageParticlesPrefab, spawnPoint);
    }

    public void spawnDeathParticles()
    {
        // Creates the particles
        Instantiate(deathParticlesPrefab, spawnPoint);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageParticles : MonoBehaviour
{
    [SerializeField] private GameObject particlesPrefab;
    [SerializeField] private ParticleSystem damageParticles;
    [SerializeField] private Color color;

    private void Awake()
    {
        damageParticles = Instantiate(particlesPrefab, transform).GetComponent<ParticleSystem>();
        damageParticles.startColor = color;
    }

    public void spawnParticles()
    {
        damageParticles.Play();
    }
}

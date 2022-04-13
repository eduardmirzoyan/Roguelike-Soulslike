using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private Projectile projectile;
    [SerializeField] private GameObject segment;
    [SerializeField] private Transform segmentSpawnPoint; // Spawn point for each segment
    [SerializeField] private float spawnInterval; // Rate at which segments spawn

    [SerializeField] private int shockwaveDamage;
    [SerializeField] private float shockwavePushForce;

    private float timer; // Timer to keep track of each segment

    private void Start() {
        projectile = GetComponent<Projectile>();
    }

    private void FixedUpdate()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            GameObject spike = Instantiate(segment, segmentSpawnPoint.position, segmentSpawnPoint.rotation);
            spike.transform.localScale = transform.localScale;
            timer = spawnInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        var damageable = collision.GetComponent<Damageable>();
        // If arrow hits something dmaageableo other than the creator
        if (damageable != null && collision.gameObject != projectile.creator) {
           
            // Deal damage
            Damage dmg = new Damage {
                damageAmount = shockwaveDamage,
                source = DamageSource.fromEnemy,
                origin = projectile.creator.transform,
                pushForce = shockwavePushForce,
                color = Color.red
            };
            damageable.takeDamage(dmg);
        }
    }
}

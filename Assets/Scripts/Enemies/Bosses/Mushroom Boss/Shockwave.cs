using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private GameObject segment;
    [SerializeField] private Transform segmentSpawnPoint; // Spawn point for each segment
    [SerializeField] private float spawnInterval; // Rate at which segments spawn

    [SerializeField] private Damage shockwaveDamage;

    private float timer; // Timer to keep track of each segment

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
        if (collision.gameObject != this.gameObject && damageable != null && collision.tag != "Enemy")
        {
            damageable.takeDamage(shockwaveDamage);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnchantableEntity))]
public class RangedWeapon : Weapon
{
    [Header("Components")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firepoint;

    [Header("Projectile Settings")]
    [SerializeField] protected float projectileSpeed = 15f;
    [SerializeField] protected float projectileSizeMult = 1f;
    [SerializeField] protected float projectileSpeedMult = 1f;
    [SerializeField] protected int numberOfPierces = 0;
    [SerializeField] protected int numberOfBounces = 0;

    public void addPierces(int amount) {
        numberOfPierces += amount;
    }

    public void addBounces(int amount) {
        numberOfBounces += amount;
    }

    public void increaseProjectileSpeed(float amount) {
        projectileSpeedMult += amount;
    }

    public void increaseProjectileSize(float amount) {
        projectileSizeMult += amount;
    }
}

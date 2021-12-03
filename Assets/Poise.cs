using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poise : MonoBehaviour
{
    [SerializeField] private int currentPoise;
    [SerializeField] private int basePoise;

    [SerializeField] private KnockbackEffect staggerEffect;

    private float timer = 30f;

    private void FixedUpdate()
    {
        if (currentPoise < basePoise && timer > 0)
            timer -= Time.deltaTime;

        if(timer <= 0)
        {
            currentPoise = basePoise;
            timer = 30f;
        }
    }

    public void attackPoise(int weaponPoise)
    {
        currentPoise += weaponPoise; // Add weapon poise to current poise
    }

    public void postAttackPoise(int weaponPoise)
    {
        float percentage = currentPoise / (basePoise + weaponPoise);
        currentPoise = Mathf.RoundToInt(percentage * basePoise);
    }

    public void damagePoise(int amount, Vector3 origin)
    {
        currentPoise -= amount;
        timer = 30f; // Reset timer
        if (currentPoise <= 0)
        {
            currentPoise = basePoise;

            // Trigger stagger
            var displace = GetComponent<Displacable>();
            if (displace != null)
                displace.triggerDisplace(15 * displace.getDirectionFromPoint(origin), 0.25f);
        }
    }

    public void resetPoise() => currentPoise = basePoise;

    public void addBasePoise(int amount) => basePoise += amount;

    public void removeBasePoise(int amount) => basePoise -= amount;

    public int getPoise() => currentPoise;
}

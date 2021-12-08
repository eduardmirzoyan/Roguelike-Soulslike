using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EffectBuildupHandler : MonoBehaviour
{

    [SerializeField] private GameObject buildUpParticles;

    // Pairs of element type and percent build up as an int
    [SerializeField] private Dictionary<StatusType, BuildupEffect> activeEffectBuildups = new Dictionary<StatusType, BuildupEffect>();
    //[SerializeField] private List<BuildupEffect> buildupEffects = new List<BuildupEffect>();
    private bool isPaused;

    private void FixedUpdate()
    {
        // If game is paused, then don't tick bars
        if (isPaused)
            return;

        // For every item in the dictionary, tick its effect
        foreach (var buildUpEffect in activeEffectBuildups.Values.ToList())
        {
            buildUpEffect.tick();

            // If effect is over, then remove it from the dict
            if (buildUpEffect.buildUpAmount <= 0)
            {
                activeEffectBuildups.Remove(buildUpEffect.type);
            }
        }
    }

    public void addEffectBuildUp(BuildupEffect buildUpEffect)
    {
        var amount = buildUpEffect.buildUpAmount;
        var stats = GetComponent<CombatStats>();
        if(stats != null)
        {
            // Remove this at some point lol
            switch (buildUpEffect.type)
            {
                case StatusType.Burn:
                    amount = (int)(amount * (1 - stats.percentFireResistance / 100f));
                    break;
                case StatusType.Poison:
                    amount = (int)(amount * (1 - stats.percentPoisonResistance / 100f));
                    break;
                case StatusType.Cold:
                    amount = (int)(amount * (1 - stats.percentFrostResistance / 100f));
                    break;
                case StatusType.Curse:
                    amount = (int)(amount * (1 - stats.percentCurseResistance / 100f));
                    break;
            }
        }


        // If dictionary contains the build up, then increment it or add new effect
        if (activeEffectBuildups.ContainsKey(buildUpEffect.type))
        {
            // Increment build up
            activeEffectBuildups[buildUpEffect.type].addBuildUp(amount);
        }
        else
        {
            // Add new effect build up
            activeEffectBuildups.Add(buildUpEffect.type, Instantiate(buildUpEffect));
        }

        // Display particle effect as a child
        if (buildUpParticles != null) {
            var particles = Instantiate(buildUpParticles, transform);
            particles.GetComponent<ParticleSystem>().startColor = buildUpEffect.color;
        }
            

        // Check if the associate effect bar is filled completely
        // If so remove it, and apply the corresponding effect
        if (activeEffectBuildups[buildUpEffect.type].buildUpAmount >= 100)
        {
            activeEffectBuildups.Remove(buildUpEffect.type);
            var effectable = GetComponent<EffectableEntity>();
            if (effectable != null)
            {
                // Add effect
                effectable.addEffect(buildUpEffect.triggerEffect.InitializeEffect(gameObject));
            }
        }
    }

    public List<BuildupEffect> getListOfBuildUps()
    {
        return activeEffectBuildups.Values.ToList();
    }
}
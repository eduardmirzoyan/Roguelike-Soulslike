using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EffectBuildupUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> effectBars; // Holds any number of effects you may want
    [SerializeField] private EffectBuildupHandler playerBuildupHandler;
    [SerializeField] private GameObject barPrefab;

    private int maxSize;
    private int currSize;

    private void Start()
    {
        // Set the size to the total amount of unique status effects
        int numSlots = System.Enum.GetNames(typeof(StatusType)).Length;
        maxSize = numSlots;
    }

    private void FixedUpdate()
    {
        updateEffectsUI();
    }

    public void addEffectBar()
    {
        // Instaniate prefav and add to bar
        var newBar = Instantiate(barPrefab, transform);
        newBar.GetComponentInChildren<Slider>().maxValue = 100;
        newBar.GetComponentInChildren<Slider>().value = 1;
    }

    public void updateEffectsUI()
    {
        // Adjust the amount of gameobjects to match the amount of dictionary entries
        if (currSize < playerBuildupHandler.getListOfBuildUps().Count)
        {
            // Add new bar
            var newBar = Instantiate(barPrefab, transform); // As a parent
            effectBars.Add(newBar);
            currSize++;
        }
        else if (currSize > playerBuildupHandler.getListOfBuildUps().Count)
        {
            // Remove bar at first location
            Destroy(effectBars[0]);
            effectBars.RemoveAt(0);
            currSize--;
        }

        // Go through each entry and update UI
        int count = 0;
        foreach (var buildupEffect in playerBuildupHandler.getListOfBuildUps())
        {
            // Update effects
            var effectBar = effectBars[count].GetComponentInChildren<EffectBar>();
            if(effectBar != null)
            {
                effectBar.slider.value = buildupEffect.buildUpAmount;
                effectBar.icon.sprite = buildupEffect.icon;
                effectBar.fill.color = buildupEffect.color;
                effectBar.effectName.text = buildupEffect.type.ToString();
            }
            count++;
        }


    }
}

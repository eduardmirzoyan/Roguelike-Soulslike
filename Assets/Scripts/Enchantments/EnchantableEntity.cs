using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnchantableEntity : MonoBehaviour
{
    [SerializeField] private List<Enchantment> enchantments;
    private bool isPaused;

    private void FixedUpdate()
    {
        if (isPaused)
            return;

        // Tick every enchantment
        foreach (var enchant in enchantments)
            enchant.onTick();
    }

    public void addEnchantment(Enchantment enchantment)
    {
        enchantment = Instantiate(enchantment); // Make a copy

        if (!enchantments.Any(x => x.GetType() == enchantment.GetType())) // Check if any enchantments in your current enchantments are used
        {
            enchantments.Add(enchantment);
            enchantment.intialize(gameObject);
            GameManager.instance.CreatePopup("New enchantment added.", transform.position);
        }
        else
        {
            Debug.Log("you already have this enchantment.");
        }
    }

    public void removeEnchantment(Enchantment enchantment)
    {
        foreach (var enchant in enchantments)
        {
            // If you have the requested enchantment, uninsitalize it, remove it then break the loop
            if (enchant.GetType() == enchantment.GetType())
            {
                enchant.unintialize();
                enchantments.Remove(enchant);
                Debug.Log("Enchantment removed");
                break;
            }
        }
    }
}

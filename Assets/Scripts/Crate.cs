using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(CombatStats))]
public class Crate : MonoBehaviour
{
    [SerializeField] private WorldItem itemHolder;

    [SerializeField] private Item containedItem;

    // Update is called once per frame
    private void FixedUpdate()
    {
        var health = GetComponent<Health>();
        if(health != null)
        {
            // If crate is broken destroy itself and spawn item
            if (health.isEmpty())
            {
                if(itemHolder != null)
                {
                    // Get a random drop
                    containedItem = GameManager.instance.getConsumableDrop();
                    itemHolder.setItem(containedItem); // Add drop to containeer
                    GameManager.instance.CreatePopup("You broke a crate...", transform.position);
                    Instantiate(itemHolder, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }
    }
}

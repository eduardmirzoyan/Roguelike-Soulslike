using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private Inventory inventory;

    [Header("Settings")]
    [SerializeField] private List<Shrine> interactedShrines;
    [SerializeField] private float minRange = 1f;

    private void Start()
    {
        collider2d = GetComponent<Collider2D>();
        inventory = GetComponentInChildren<Inventory>();
        interactedShrines = new List<Shrine>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (interactedShrines.Count > 0) {
            foreach (var shrine in interactedShrines) {
                // If you move too far from a shrine, then release it
                //print(Vector2.Distance(shrine.transform.position, collider2d.bounds.center));
                if (Vector2.Distance(shrine.transform.position, collider2d.bounds.center) > minRange) {
                    shrine.activateRelease(GetComponent<Player>());
                    interactedShrines.Remove(shrine);
                    break;
                }
            }
        }
        
    }

    public void interactWithNearbyObjects()
    {
        var iteractables = Physics2D.OverlapBoxAll(collider2d.bounds.center, collider2d.bounds.size, 0, 1 << LayerMask.NameToLayer("Interactables"));

        // If no interactables, then dip
        if (iteractables.Length == 0) {
            return;
        }
            
        foreach (var interactable in iteractables) {
            if (interactable.TryGetComponent(out Shrine shrine)) {
                // Activate shrine
                shrine.activatePress(GetComponent<Player>());
                // Add shrine to list
                interactedShrines.Add(shrine);
            }

            if (interactable.TryGetComponent(out Chest chest)) {
                chest.open();
            }
            
            if (interactable.TryGetComponent(out Door door)) {
                door.enter();
            }
        }
    }

    public void releaseNearbyObjects() {
        var iteractables = Physics2D.OverlapBoxAll(collider2d.bounds.center, collider2d.bounds.size, 0, 1 << LayerMask.NameToLayer("Interactables"));
        if (iteractables.Length == 0) {
            return;
        }

        foreach (var interactable in iteractables) {
            if (interactable.TryGetComponent(out Shrine shrine)) {
                // Shrine must have been activated first
                if (interactedShrines.Contains(shrine)) {
                    // Release and remove from list
                    shrine.activateRelease(GetComponent<Player>());
                    interactedShrines.Remove(shrine);
                }
                
            }
        }
    }

    public void pickUpNearbyItems()
    {   
        var droppedItems = Physics2D.OverlapBoxAll(collider2d.bounds.center, collider2d.bounds.size, 0, 1 << LayerMask.NameToLayer("Loot"));

        // If no items, then dip
        if (droppedItems.Length == 0) {
            return;
        }
            
        foreach (var droppedItem in droppedItems) {
            var worldItem = droppedItem.GetComponent<WorldItem>();
            if (worldItem != null)
            {
                var pickedUpItem = worldItem.GetItem();
                PopUpTextManager.instance.createVerticalPopup("You picked up " + pickedUpItem.name, Color.white, transform.position);
                // Trigger event
                GameEvents.instance.triggerOnItemPickup(pickedUpItem);

                inventory.addItem(pickedUpItem);
                
                Destroy(droppedItem.gameObject);   
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(collider2d.bounds.center, collider2d.bounds.size);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(collider2d.bounds.center, minRange);
    }
}

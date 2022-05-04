using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ExchangeShrine : Shrine
{
    [Header("Visuals")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Text text;
    [SerializeField] private Image arrowIndicator;
    [SerializeField] private Image confirmationCircle;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string enchantingAnimation = "Enchant";
    [SerializeField] private float animationDuration = 5f;
    [SerializeField] private float itemTossVelocity = 10f;
    [SerializeField] private WorldItem targetItem;
    [SerializeField] private WorldItem sacrificeItem;
    [SerializeField] private Transform targetItemLocation;
    [SerializeField] private Transform sacrificeItemLocation;
    [SerializeField] private Transform centerLocation;
    [SerializeField] private float grabRadius;
    [SerializeField] private float travelTime = 1f;
    [SerializeField] private float minHoldTime = 2f;
    [SerializeField] private int enchantmentToRemoveIndex;

    [Header("Particles")]
    [SerializeField] private ParticleSystem passiveParticles;
    [SerializeField] private GameObject enchantingParticlesPrafab;
    [SerializeField] private GameObject destructionParticlesPrefab;

    private float holdTimer;
    private bool isHeld;

    private void Start() {
        animator = GetComponent<Animator>();
        GameEvents.instance.onItemDrop += grabDroppedItem;
        GameEvents.instance.onItemPickup += checkItem;
        canvas.enabled = false;
        canvas.enabled = true;
        canvas.enabled = false;
    }

    public override void activatePress(Player player)
    {
        // If both items are chosen, then you can interact with the shrine
        if (targetItem != null && sacrificeItem != null) {
            isHeld = true;
        }
        else {
            PopUpTextManager.instance.createVerticalPopup("Drop 2 compatible gear.", Color.gray, transform.position);
        }

    }

    public override void activateRelease(Player player)
    {
        if (targetItem != null && sacrificeItem != null) {
            // If held less than 0.25 sec
            if (holdTimer < 0.25f) {
                // If the sacrifice is an armor item, then you can't rotate through any enchantments
                if (sacrificeItem.GetItem() is ArmorItem) {
                    return;
                }

                // Cache weapon
                var weaponItem = (WeaponItem) sacrificeItem.GetItem();

                // Increment index
                enchantmentToRemoveIndex++;

                if (enchantmentToRemoveIndex >= weaponItem.enchantments.Count) {
                    enchantmentToRemoveIndex = 0;
                }

                // Update UI
                text.text = weaponItem.enchantments[enchantmentToRemoveIndex].enchantmentName;
            }
            
            // Reset hold time
            holdTimer = 0;
            confirmationCircle.fillAmount = 0;

            isHeld = false;
        }
    }

    private void FixedUpdate() {
        if (isHeld) {
            holdTimer += Time.deltaTime;
            confirmationCircle.fillAmount = Mathf.Min(holdTimer / minHoldTime, 1);

            // If held long enough, intiate transfer
            if (holdTimer >= minHoldTime) {
                // Disable UI
                text.text = null;
                confirmationCircle.fillAmount = 0;
                canvas.enabled = false;

                // Reset timers
                isHeld = false;
                holdTimer = 0;

                // Transfer process
                StartCoroutine(transferAnimation(animationDuration));
            }
        }
    }

    private void grabDroppedItem(WorldItem worldItem) {
        // If an item is dropped within range then take that item
        if (Vector2.Distance(transform.position, worldItem.transform.position) < grabRadius) {
            
            // Cache item
            var item = worldItem.GetItem();

            // If target item is not chosen, then attempt to choose it
            if (targetItem == null) {
                // If you already have a sacrifice item, then make sure it's the same type
                if (sacrificeItem != null) {
                    // If items are not the same type, then ignore item
                    if (sacrificeItem.GetItem().GetType() != item.GetType()) {
                        PopUpTextManager.instance.createVerticalPopup("Items must be the same gear type", Color.gray, transform.position);
                        // Ignore!
                        return;
                    }
                    // Check if the combo is range + melee
                    if (item is WeaponItem) {
                        var target = (WeaponItem) item;
                        var sacrifice = (WeaponItem) sacrificeItem.GetItem();
                        if (   (target.weaponType == WeaponType.LongBow && sacrifice.weaponType != WeaponType.LongBow)
                            || (target.weaponType != WeaponType.LongBow && sacrifice.weaponType == WeaponType.LongBow) ) {
                            PopUpTextManager.instance.createVerticalPopup("Cannot transfer between Melee and Ranged weapons", Color.gray, transform.position);
                            // Ignore!
                            return;
                        }
                    }
                }

                // Check if weapon item has open slots
                if (item is WeaponItem) {
                    var weaponItem = (WeaponItem) item;

                    // Check if weapon has any slots and that they are open
                    if (weaponItem.enchantmentSlots > 0 && weaponItem.enchantmentSlots > weaponItem.enchantments.Count) {
                        worldItem.GetComponent<Rigidbody2D>().gravityScale = 0;
                        targetItem = worldItem;
                        StartCoroutine(travelToLocation(targetItem.transform, targetItemLocation, travelTime));
                        return;
                    }
                    // Check as sacrifice
                }

                if (worldItem.GetItem() is ArmorItem) {
                    var armorItem = (ArmorItem) item;
                    // Check if the enchantment slot of armor is empty
                    if (armorItem.enchantment == null) {
                        // Then choose item
                        worldItem.GetComponent<Rigidbody2D>().gravityScale = 0;
                        targetItem = worldItem;
                        StartCoroutine(travelToLocation(targetItem.transform, targetItemLocation, travelTime));
                        return;
                    }
                }
                
            }

            // If target item did not choose, then try as sacrifice
            if (sacrificeItem == null) {

                // If you already have a target item, then make sure it's the same type
                if (targetItem != null) {
                    // If items are not the same type, then ignore item
                    if (targetItem.GetItem().GetType() != item.GetType()) {
                        PopUpTextManager.instance.createVerticalPopup("Items must be the same gear type", Color.gray, transform.position);
                        // Ignore!
                        return;
                    }

                    // Check mixing
                    if (item is WeaponItem) {
                        var sacrif = (WeaponItem) item;
                        var target = (WeaponItem) targetItem.GetItem();
                        if (   (sacrif.weaponType == WeaponType.LongBow && target.weaponType != WeaponType.LongBow)
                            || (sacrif.weaponType != WeaponType.LongBow && target.weaponType == WeaponType.LongBow) ) {
                            PopUpTextManager.instance.createVerticalPopup("Cannot transfer between Melee and Ranged weapons", Color.gray, transform.position);
                            // Ignore!
                            return;
                        }
                    }
                }

                // Check if weapon item has open slots
                if (item is WeaponItem) {
                    var weaponItem = (WeaponItem) item;
                    // Check if weapon has any enchantments
                    if (weaponItem.enchantments.Count > 0) {
                        // Set index
                        enchantmentToRemoveIndex = 0;

                        // Then choose item
                        worldItem.GetComponent<Rigidbody2D>().gravityScale = 0;
                        sacrificeItem = worldItem;
                        StartCoroutine(travelToLocation(sacrificeItem.transform, sacrificeItemLocation, travelTime));
                        return;  
                    }
                    // Else provide feedback
                    PopUpTextManager.instance.createVerticalPopup("This item does not have enchantments to offer", Color.gray, transform.position);
                    return;
                }

                if (worldItem.GetItem() is ArmorItem) {
                    var armorItem = (ArmorItem) item;
                    // Check if armor has an enchantment
                    if (armorItem.enchantment != null) {
                        // Then choose item
                        worldItem.GetComponent<Rigidbody2D>().gravityScale = 0;
                        sacrificeItem = worldItem;
                        StartCoroutine(travelToLocation(sacrificeItem.transform, sacrificeItemLocation, travelTime));
                        return;
                    }
                }
            }
            
            // Else provide feedback
            //if ()
            PopUpTextManager.instance.createVerticalPopup("This item does not have any EMPTY slots", Color.gray, transform.position);
            print("2 Items already offered or the weapon doesn't meet criteria.");
        }
    }

    private IEnumerator travelToLocation(Transform target, Transform destination, float duration) {
        float elapsedTime = 0;
        
        // Smoothly move to target
        while (elapsedTime < duration)
        {
            // If target is removed, then dip
            if (target == null) {
                yield break;
            }

            // Lerp position
            target.position = Vector2.Lerp(target.position, destination.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Finally set position to end
        target.position = destination.position;

        // If both items are choosen, then display UI
        if (targetItem != null && sacrificeItem != null) {
            // Enable visuals
            canvas.enabled = true;

            // Set name
            var sacItem = sacrificeItem.GetItem();
            if (sacItem is WeaponItem) {
                text.text = ((WeaponItem) sacItem).enchantments[enchantmentToRemoveIndex].enchantmentName;
            }
            else if (sacItem is ArmorItem) {
                text.text = ((ArmorItem) sacItem).enchantment.enchantmentName;
            }
        }
        
    }

    private IEnumerator transferAnimation(float duration) {
        // Disable colliders
        targetItem.enableCollider(false);
        sacrificeItem.enableCollider(false);
        
        // Hide sacrifice
        sacrificeItem.GetComponentInChildren<SpriteRenderer>().enabled = false;

        // Transfer enchantment
        transferEnchantment();

        // Show destruction particles
        Instantiate(destructionParticlesPrefab, sacrificeItemLocation.position, Quaternion.identity);

        // Increase particles for visual feedback
        var emission = passiveParticles.emission;
        emission.rateOverTime = 15;
        var main = passiveParticles.main;
        main.startSpeed = 1.5f;
        main.startLifetime = 2;

        // Timer
        float elapsedTime = 0;

        // Smoothly move to target
        while (elapsedTime < 1f)
        {
            // If target is removed, then dip
            if (targetItem.transform == null) {
                yield break;
            }

            // Lerp target item to its spot
            targetItem.transform.position = Vector2.Lerp(targetItem.transform.position, centerLocation.position, elapsedTime / 1f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Give target item particles
        var ps = Instantiate(enchantingParticlesPrafab, targetItem.transform).GetComponent<ParticleSystem>();
        var psMain = ps.main;
        psMain.duration = duration;
        ps.Play();

        // Play animation
        animator.Play(enchantingAnimation);

        // Wait for animation to end
        yield return new WaitForSeconds(duration);

        // Wait 1s for dramatic effect
        yield return new WaitForSeconds(1f);

        // Revert particles
        emission.rateOverTime = 3;
        main.startSpeed = 1;
        main.startLifetime = 1;

        // Return gravity to transfer item
        targetItem.GetComponent<Rigidbody2D>().gravityScale = 1;

        // Toss item up
        targetItem.GetComponent<Rigidbody2D>().velocity = Vector2.up * itemTossVelocity;

        // Enable collider
        targetItem.enableCollider(true);

        // Destroy sacrifice item
        Destroy(sacrificeItem.gameObject);

        // Clear items
        targetItem = null;
        sacrificeItem = null;
    }

    private void transferEnchantment() {
        // Cache
        var sacItem = sacrificeItem.GetItem();

        if (sacItem is WeaponItem) {
            var targetWeapon = (WeaponItem) targetItem.GetItem();
            // Add enchantment at the remove index
            targetWeapon.enchantments.Add(((WeaponItem) sacItem).enchantments[enchantmentToRemoveIndex]);
        }
        else if (sacItem is ArmorItem) {
            var targetArmor = (ArmorItem) targetItem.GetItem();
            // Set enchantment
            targetArmor.enchantment = ((ArmorItem) sacItem).enchantment;
        }
    }

    private void checkItem(Item item) {
        // If the item picked up was one of the offerings then reset
        if (targetItem != null && item == targetItem.GetItem()) {
            targetItem = null;
            canvas.enabled = false;
        }
        else if (sacrificeItem != null && item == sacrificeItem.GetItem()) {
            sacrificeItem = null;
            canvas.enabled = false;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }
}

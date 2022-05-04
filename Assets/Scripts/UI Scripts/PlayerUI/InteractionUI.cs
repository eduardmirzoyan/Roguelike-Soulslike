using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer hotkeyImage;
    [SerializeField] private Player player;
    [SerializeField] private Collider2D collider2d;

    [Header("Settings")]
    [SerializeField] private float waitDuration = 2f;
    [SerializeField] private Sprite interactKeySprite;
    [SerializeField] private Sprite pickupKeySprite;

    private float waitTimer;


    // Start is called before the first frame update
    private void Awake()
    {
        player = GetComponent<Player>();
        collider2d = GetComponent<Collider2D>();

        hotkeyImage.enabled = false;
        waitTimer = 0;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // If player is idle
        if (player.isIdle()) {
            // Wait, then display hotkey
            if (waitTimer > 0) {
                waitTimer -= Time.deltaTime;
            }
            else {
                displayNearbyKeys();
            }
        }
        else {
            // Disable
            hotkeyImage.enabled = false;
            // Set duration
            waitTimer = waitDuration;
        }
    }

    private void displayNearbyKeys() {
        // Check for any interactables
        var hit = Physics2D.OverlapBox(collider2d.bounds.center, collider2d.bounds.size, 0, 1 << LayerMask.NameToLayer("Interactables"));
        if (hit) {
            hotkeyImage.sprite = interactKeySprite;
            hotkeyImage.enabled = true;
            return;
        }

        // Check for any loot
        hit = Physics2D.OverlapBox(collider2d.bounds.center, collider2d.bounds.size, 0, 1 << LayerMask.NameToLayer("Loot"));
        if (hit) {
            hotkeyImage.sprite = pickupKeySprite;
            hotkeyImage.enabled = true;
            return;
        }

        // else disable UI
        hotkeyImage.enabled = false;
    }
}

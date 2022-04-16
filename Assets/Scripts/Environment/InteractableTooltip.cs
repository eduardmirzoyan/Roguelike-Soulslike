using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTooltip : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform playerTranform;
    [SerializeField] private float minDistance;
    
    private void Start() {
        playerTranform = GameManager.instance.GetPlayer().transform;
        spriteRenderer.enabled = false;
    }

    private void Update() {
        if (Vector2.Distance(transform.position, playerTranform.position) < minDistance) {
            // Enable tooltip
            spriteRenderer.enabled = true;
        }
        else {
            // Disable tooltip
            spriteRenderer.enabled = false;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}

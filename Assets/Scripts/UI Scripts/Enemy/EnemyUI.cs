using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Movement mv;
    [SerializeField] private Health health;

    // Start is called before the first frame update
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        mv = GetComponentInParent<Movement>();
        health = GetComponentInParent<Health>();
        canvas.enabled = false;

        GameEvents.instance.onHit += enableCanvasOnHit;
    }

    // Update is called once per frame
    private void Update()
    {
        // If canvas is disabled, then don't do anything
        if (!canvas.enabled)
            return;

        // If enemy is dead, then disable healthbar
        if (health.isEmpty()) {
            canvas.enabled = false;
            return;
        }

        if (mv.getFacingDirection() == -1) {
            rectTransform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
        else {
            rectTransform.localRotation = Quaternion.identity;
        }
    }

    private void enableCanvasOnHit(GameObject attacker, GameObject hit, int damage) {
        if (hit == null || attacker == null || mv == null)
            return;
        if (hit.gameObject == mv.gameObject && damage > 0) {
            canvas.enabled = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private Movement mv;
    [SerializeField] private Health health;
    [SerializeField] private StatusEffectsUI statusEffectsUI;
    [SerializeField] private Healthbar healthbar;

    [SerializeField] private Image indicatorImage;
    [SerializeField] private float indicatorDuration = 1f;

    

    private Coroutine indicatorRoutine;

    // Start is called before the first frame update
    private void Awake() {
        enemyAI = GetComponentInParent<EnemyAI>();
        mv = GetComponentInParent<Movement>();
        health = GetComponentInParent<Health>();
        statusEffectsUI = GetComponentInChildren<StatusEffectsUI>();
        healthbar = GetComponentInChildren<Healthbar>();
        indicatorImage = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        // Subscribe
        GameEvents.instance.onHit += enableCanvasOnHit;
        // healthBarHolder.SetActive(false);
        indicatorImage.enabled = false;

        // Set up internal UI
        statusEffectsUI.setEntity(enemyAI.GetComponent<EffectableEntity>());
        healthbar.setEntity(health);

        // Disable UI
        healthbar.enabled = false;
        statusEffectsUI.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // If canvas is disabled, then don't do anything
        if (!healthbar.enabled)
            return;

        // Flip healthbar and Status effects
        if (mv.getFacingDirection() == -1) {
            healthbar.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
            statusEffectsUI.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
        else {
            healthbar.transform.localRotation = Quaternion.identity;
            statusEffectsUI.transform.localRotation = Quaternion.identity;
        }
    }

    private void enableCanvasOnHit(GameObject attacker, GameObject hit, int damage) {
        if (hit == null || attacker == null || mv == null)
            return;
        if (hit.gameObject == mv.gameObject && damage > 0) {
            // Enable UI
            healthbar.enabled = true;
            statusEffectsUI.enabled = true;
        }
    }

    public void enableIndicator(Sprite sprite) {
        if (indicatorRoutine != null) {
            StopCoroutine(indicatorRoutine);
        }
        indicatorRoutine = StartCoroutine(showIndicator(indicatorDuration, sprite));
    }

    private IEnumerator showIndicator(float duration, Sprite indicatorSprite) {
        indicatorImage.enabled = true;
        indicatorImage.sprite = indicatorSprite;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        indicatorImage.enabled = false;
    }
}

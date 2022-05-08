using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private GameObject healthBarHolder;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image indicatorImage;
    [SerializeField] private Movement mv;
    [SerializeField] private Health health;
    [SerializeField] private float indicatorDuration = 1f;

    private Coroutine indicatorRoutine;

    // Start is called before the first frame update
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        mv = GetComponentInParent<Movement>();
        health = GetComponentInParent<Health>();
        
        if (healthBarHolder == null) {
            print("ERROR SETTING HEALTHBAR: " + gameObject.name);
        }
    }

    private void Start()
    {
        GameEvents.instance.onHit += enableCanvasOnHit;
        healthBarHolder.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        // If canvas is disabled, then don't do anything
        if (!healthBarHolder.activeInHierarchy)
            return;

        // If enemy is dead, then disable healthbar
        if (health.isEmpty()) {
            healthBarHolder.SetActive(false);
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
            healthBarHolder.SetActive(true);
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

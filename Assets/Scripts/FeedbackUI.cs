using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour
{
    public static FeedbackUI instance;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Text feedbackShadow;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine fadeRoutine;

    private void Awake() {
        // Singleton logic
        if(FeedbackUI.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; // Sets this to itself
    }

    public void setMessage(string message, Color color, Vector3 position, float scale = 1f) {
        feedbackText.text = message;
        feedbackText.color = color;

        feedbackShadow.color = Color.black;
        feedbackShadow.transform.position = position + offset;
        feedbackShadow.text = message;

        // Start fading
        if (fadeRoutine != null) {
            StopCoroutine(fadeRoutine);
        }
        fadeRoutine = StartCoroutine(fade(duration, fadeDuration));
    }
    

    private IEnumerator fade(float duration, float fadeDuration) {
        // Wait a bit before fading
        yield return new WaitForSeconds(duration);

        // Then fade over time
        float elapsedTime = 0;
        
        Color color = feedbackText.color;
        Color shadowColor = feedbackShadow.color;
        // Smoothly fade text
        while (elapsedTime < fadeDuration)
        {
            // Lerp alpha
            color.a = 1 - elapsedTime / fadeDuration;
            shadowColor.a = 1 - elapsedTime / fadeDuration;
            feedbackText.color = color;
            feedbackShadow.color = shadowColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset
        color.a = 0;
        shadowColor.a = 0;
        feedbackText.color = color;
        feedbackShadow.color = shadowColor;
    }
}

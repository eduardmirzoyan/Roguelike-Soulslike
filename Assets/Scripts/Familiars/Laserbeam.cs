using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laserbeam : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Vector3 targetPosition;

    [SerializeField] private Color laserColor;
    [SerializeField] private float maxWidth;
    [SerializeField] private float minWidth;
    [SerializeField] private float duration;
    private float currentWidth;
    private float timer;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        targetPosition = Vector3.back;
    }

    public void fireAt(Transform transform) {
        lineRenderer.enabled = true;
        currentWidth = maxWidth;
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
        timer = 0;

        targetPosition = transform.position;
    }

    private void FixedUpdate ()
    {
        if (targetPosition != Vector3.back) {
            timer += Time.deltaTime;
            // Create a line between the two targets
            lineRenderer.SetPositions(new Vector3[] { transform.position, targetPosition });

            // Reduce width
            currentWidth = Mathf.Lerp(currentWidth, minWidth, timer / duration);
            lineRenderer.startWidth = currentWidth;

            // Reduce transparancy
            var color = lineRenderer.startColor;
            color.a = Mathf.Lerp(color.a, 0, timer / duration);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            if (timer > duration) {
                Destroy(gameObject);
            }
        }
        
        
    }
}

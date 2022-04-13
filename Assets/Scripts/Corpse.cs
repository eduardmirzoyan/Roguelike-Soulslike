using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D collider2d;

    // Start is called before the first frame update
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<BoxCollider2D>();
    }

    public void setHost(GameObject host) {
        spriteRenderer.sprite = host.GetComponent<SpriteRenderer>().sprite;
        transform.position = host.transform.position;
        transform.rotation = host.transform.rotation;
        collider2d.size = spriteRenderer.size;

        // Increase transparancy
        Color color = spriteRenderer.color;
        color.a /= 2;
        spriteRenderer.color = color;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [SerializeField] private Collider2D currentStandingPlatform;
    private Collider2D entityCollider;
    private int platformLayer;

    [SerializeField] private bool isDropping;

    // Start is called before the first frame update
    private void Start()
    {
        entityCollider = GetComponent<Collider2D>();
        platformLayer = LayerMask.NameToLayer("Platform");
    }

    public void dropFromPlatform() {
        if (currentStandingPlatform != null)
            StartCoroutine(disableCollision(0.35f));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == platformLayer) {
            currentStandingPlatform = collision.collider;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.layer == platformLayer) {
            currentStandingPlatform = null;
        }
    }

    private IEnumerator disableCollision(float time) {
        var platform = currentStandingPlatform;
        Physics2D.IgnoreCollision(entityCollider, platform, true);
        isDropping = true;
        yield return new WaitForSeconds(time);
        Physics2D.IgnoreCollision(entityCollider, platform, false);
        isDropping = false;
    }

    public bool IsDropping() {
        return isDropping;
    }
}

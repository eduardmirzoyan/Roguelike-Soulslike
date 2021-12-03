using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private GameObject platform;
    [SerializeField] private float phaseDuration;

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space))
        {
            if(platform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            platform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        CompositeCollider2D platfromCollider = platform.GetComponent<CompositeCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platfromCollider);
        yield return new WaitForSeconds(phaseDuration);
        Physics2D.IgnoreCollision(playerCollider, platfromCollider, false);
    }
}

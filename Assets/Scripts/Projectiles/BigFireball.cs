using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFireball : Fireball
{
    [SerializeField] private GameObject fireFloorZone;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (collision.gameObject != this.gameObject && damageable != null && collision.tag != GetComponent<Projectile>().creator.tag)
        {
            damageable.takeDamage(fireballDamage);
        }

        if (collision.tag == "Ground")
        {
            Instantiate(fireFloorZone, getGroundPosition(), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private Vector2 getGroundPosition()
    {
        // Intial destination is x distance infront of the entity in the chosen direction
        Vector2 destination = transform.position;

        RaycastHit2D hit; //= Physics2D.Linecast(transform.position, destination);

        // Then raycast from the current chosen destination downwards
        hit = Physics2D.Raycast(destination, -Vector2.up);
        if (hit)
        {
            // Set destination to be at the top of the contact point
            destination = hit.point;

            // Then give some buffer space
           destination.y -= 0.6f;
        }

        // Return destination
        return destination;
    }
}

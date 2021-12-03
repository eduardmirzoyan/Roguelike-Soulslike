using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] private EnemyAI enemy; // Enemy that holds this hitbox
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyAI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            if (!enemy.currentAttack?.damage.isAvoidable ?? false)
                spriteRenderer.material = GameManager.instance.getPerilousMaterial();
            else
                spriteRenderer.material = GameManager.instance.getDefaultMaterial();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<Damageable>();
        if (collision.gameObject != this.gameObject && damageable != null)
        {
            damageable.takeDamage(enemy.currentAttack.damage);
        }
    }
}

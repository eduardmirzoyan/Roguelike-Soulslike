using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Changes the gravity scale of entity
[CreateAssetMenu(menuName = "Enchantments/Featherfall")]
public class FeatherFallEnchantment : Enchantment
{
    private Rigidbody2D body;
    private FallDamage fallDamage;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        body = entity.GetComponent<Rigidbody2D>();
        fallDamage = entity.GetComponent<FallDamage>();

        if (body != null)
        {
            body.gravityScale = 1.5f;
            if (fallDamage != null)
                fallDamage.enableFallDamage = false;
        }
    }

    public override void unintialize()
    {
        body = entity.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.gravityScale = 2.5f;
            if (fallDamage != null)
                fallDamage.enableFallDamage = true;
        }
        base.unintialize();
    }
}

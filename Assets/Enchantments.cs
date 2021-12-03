using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Changes the gravity scale of entity
[CreateAssetMenu]
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
                fallDamage.enabled = false;
        }
    }

    public override void unintialize()
    {
        body = entity.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.gravityScale = 2.5f;
            if (fallDamage != null)
                fallDamage.enabled = true;
        }
        base.unintialize();
    }
}

[CreateAssetMenu(menuName = "Enchantments/")]
public class DebuffDamageReductionEnchantment : Enchantment
{
    private EffectableEntity effectableEntity;
    private CombatStats stats;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        stats = entity.GetComponent<CombatStats>();
        effectableEntity = entity.GetComponent<EffectableEntity>();
    }

    public override void onTick()
    {
        if(effectableEntity != null && stats != null)
        {
            stats.damageTakenMultiplier = effectableEntity.getNumberOfEffects() * 0.1f;
        }
    }

    public override void unintialize()
    {
        stats.damageTakenMultiplier = 0;
        stats = null;
        effectableEntity = null;
        base.unintialize();
    }
}

[CreateAssetMenu(menuName = "Enchantments/")]
public class BurnImmunityEnchantment : Enchantment
{
    private CombatStats stats;
    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        stats = entity.GetComponent<CombatStats>();
        if (stats != null)
            stats.percentFireResistance = 100;
    }

    public override void unintialize()
    {
        if (stats != null)
            stats.percentFireResistance = 0;
        stats = null;
        base.unintialize();
    }
}

/*
 * Examples:
 * Featherfall: change gameobject's rigidbody's gravity
 * Double Jump: jump logic should be in perk? "can double jump" variable
 * Familiar based perk: Just summon familiar and store reference, and despawn when perk is removed, logic is stored in familiar
 * Damage Aura: Outward expanding damage ring with particles every x seconds
 * Long range: Check if an enemy is in your line of sight AND x-distance away from you before triggering effect
 * Close range: Same logic as long range
 * On Ability Use: check for ability trigger event for utility and/or special
 * Timed Effects: When perk cooldown is done, a visual effect should appear or give status effect
 * Projectile modifier?: Should projectiles contain their owners? Need to check when the perk owner spawns a projectile
 *      Maybe add projectile spawn trigger with summoner and the projectile. Projectile modifer would sub to this event,
 *      and if the projectile summoner is the same gameobject as it's perk holder, then modify the projectile
 * Stat Modifier: Change entity values, pretty easy lol
 * Perma-death source protection: Add resistances to these values, or bools?
 * 
 * HOW to differentiate perilous attacks from boulder/spike? Make perilous and boulders the same idea (acts like a perilous attack)
 * Spikes, Boulders, Falling out of map deal 999 damage always (undodgeable, must physically avoid)
 * Fall damage deals % of hp based on contact speed? or distance?
 * 
 * But how to grant resistance to spikes/ boulders? Each source could have it's own handler and it's own immunity logic?
 */

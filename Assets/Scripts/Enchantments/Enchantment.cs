using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enchantment : ScriptableObject
{
    protected GameObject entity;

    // Basic intialize just takes in the gameobject it is attached to
    public virtual void intialize(GameObject gameObject)
    {
        entity = gameObject;
    }

    public virtual void onTick()
    {
        // Do nothing on tick originally
    }

    // Basic deinitalize just nullifies current attatched gameobject
    public virtual void unintialize()
    {
        entity = null;
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
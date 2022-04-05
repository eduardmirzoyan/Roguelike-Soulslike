using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Damage Aura")]
public class DamageAuraEnchantment : Enchantment
{
    [SerializeField] private GameObject particlesPrefab;
    [SerializeField] private float procTime = 3f;
    [SerializeField] private float auraRadius = 2f;
    [SerializeField] private int auraDamage = 1;
    private ParticleSystem particles;
    private float timer;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        particles = Instantiate(particlesPrefab, entity.transform).GetComponent<ParticleSystem>();
        timer = procTime;
        particles.Play();
    }

    public override void onTick()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            // Start particle Effect
            particles.Play();

            // At end of particles, deal damage to surrounding enemies
            var hits = Physics2D.OverlapCircleAll(entity.transform.position, auraRadius);
            foreach (var hit in hits)
            {
                if (hit.gameObject != entity && hit.TryGetComponent(out Damageable damageable))
                {
                    Damage dmg = new Damage {
                        damageAmount = auraDamage,
                        source = DamageSource.fromPlayer,
                        origin = entity.transform
                    };
                    damageable.takeDamage(dmg);
                }
            }

            timer = procTime;
        }
    }

    public override void unintialize()
    {
        particles.Stop();
        particles = null;
        Destroy(particlesPrefab);
        base.unintialize();
    }
}

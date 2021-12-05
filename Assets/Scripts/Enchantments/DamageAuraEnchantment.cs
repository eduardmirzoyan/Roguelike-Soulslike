using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Damage Aura")]
public class DamageAuraEnchantment : Enchantment
{
    [SerializeField] private GameObject particlesPrefab;
    [SerializeField] private float procTime = 3f;
    [SerializeField] private float auraRadius = 2f;
    [SerializeField] private Damage auraDamage;
    private ParticleSystem particle;
    private float timer;

    public override void intialize(GameObject gameObject)
    {
        base.intialize(gameObject);
        particle = Instantiate(particlesPrefab, entity.transform).GetComponent<ParticleSystem>();
        timer = procTime;
    }

    public override void onTick()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            // Start particle Effect
            particle.Play();

            // At end of particles, deal damage to surrounding enemies
            var hits = Physics2D.OverlapCircleAll(entity.transform.position, auraRadius);
            foreach (var hit in hits)
            {
                var damageable = hit.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.takeDamage(auraDamage);
                }
            }

            timer = procTime;
        }
    }

    public override void unintialize()
    {
        particle = null;
        Destroy(particlesPrefab);
        base.unintialize();
    }
}

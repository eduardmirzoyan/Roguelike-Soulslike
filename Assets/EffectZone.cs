using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(ParticleSystem))]
public class EffectZone : MonoBehaviour
{
    [SerializeField] private ParticleSystem zoneParticles;
    [SerializeField] private Color particleColor;
    [SerializeField] private BoxCollider2D boxCollider;

    [SerializeField] private BuildupEffect effect;
    [SerializeField] private int buildUpAmount;

    [SerializeField] private float expansionRate;
    [SerializeField] private float expansionAmountX;
    [SerializeField] private float expansionAmountY;
    [SerializeField] private int amountOfExpansions;
    private float expansionTimer;

    [SerializeField] private float maxExpansionDuration;

    [Tooltip("-1 for left, 1 for right")]
    [SerializeField] private int xDirection;

    [Tooltip("-1 for down, 1 for up")]
    [SerializeField] private int yDirection;

    [SerializeField] private float tickRate;
    private float timer;

    // Start is called before the first frame update
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        zoneParticles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (amountOfExpansions > 0)
        {
            if (expansionTimer > 0)
                expansionTimer -= Time.deltaTime;
            else
            {
                // Increase size of box
                var oldArea = boxCollider.size.x * boxCollider.size.y;
                boxCollider.size += new Vector2(expansionAmountX, expansionAmountY);
                var newArea = boxCollider.size.x * boxCollider.size.y;

                // Offset box
                transform.position += new Vector3(xDirection * (expansionAmountX / 2), yDirection * (expansionAmountY / 2), 0);

                // Set particle system bounds to the bounds of the new boxcollider size
                var shape = zoneParticles.shape;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(boxCollider.size.x, boxCollider.size.y, 0);

                // Maintain particle density
                var emission = zoneParticles.emission;
                emission.rateOverTime = (int)(emission.rateOverTime.constant * (newArea / oldArea));

                // Reduce amount of expansions
                amountOfExpansions -= 1;

                // Reset rate
                expansionTimer = expansionRate;
            }
        }
        else
        {
            // After the zone has expanded to it's max, destroy it in x-seconds
            if (maxExpansionDuration > 0)
                maxExpansionDuration -= Time.deltaTime;
            else
            {
                zoneParticles.Stop();
                boxCollider.enabled = false;
                Destroy(gameObject, 0.2f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        timer -= Time.deltaTime;

        // Apply effect every x seconds
        if (timer < 0)
        {
            var buildupHandler = collision.GetComponent<EffectBuildupHandler>();
            if (buildupHandler != null)
                buildupHandler.addEffectBuildUp(effect);
            timer = tickRate;
        }
    }

    public void setBuildUpEffect(BuildupEffect effect, int amount)
    {
        this.effect = effect;
        buildUpAmount = amount;

        // Set color of zone to indicate effect
        var color = zoneParticles.colorOverLifetime.color;
        color.color = effect.color;
    }
}

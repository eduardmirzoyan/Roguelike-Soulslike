using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;

public class ImpAI : EnemyAI
{
    [Header("Imp Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private CompositeCollider2D platformCollider;
    [SerializeField] private Image interactingCircle;
    [SerializeField] private ParticleSystem rallyParticles;

    [Header("Imp Settings")]
    [SerializeField] private int goldPerGeneration;
    [SerializeField] private float goldGenerationRate;
    [SerializeField] private float interactionDuration;
    [SerializeField] private float interactionOffset;
    [SerializeField] private float rallyRadius;
    [SerializeField] private float rallyDuration;

    [Header("Debugging")]
    [SerializeField] private int heldGold;
    [SerializeField] private List<Shrine> shrinesToPatrol;
    [SerializeField] private int currentShrineIndex;
    [SerializeField] private bool hasRallied;

    [Header("Animation")]
    [SerializeField] private string flyingAnimation = "Fly";
    [SerializeField] private string attackAnimation = "Attack";
    [SerializeField] private string rallyAnimation = "Rally";
    [SerializeField] private string deadAnimation = "Dead";
    [SerializeField] private string stunnedAnimation = "Stunned";

    // Private helper variables
    private float interactionTimer;
    private float rallyTimer;
    private float goldTimer;
    private Vector2 attackDirection;

    private enum ImpState {
        Idle,
        Searching,
        Rallying,
        Attacking,
        Interacting,
        Stunned,
        Dead
    }
    [SerializeField] private ImpState impState;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        collider2d = GetComponent<Collider2D>();
        rallyParticles = GetComponent<ParticleSystem>();

        // Don't interact with platforms
        platformCollider = GameObject.Find("Platform").GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(platformCollider, collider2d);
        
        goldTimer = goldGenerationRate;
        interactingCircle.fillAmount = 0;
        heldGold = 0;

        // Generate a list of shrines to visit
        generatePatrolPath();

        // Set starting state
        impState = ImpState.Idle;
    }

    // Update is called once per frame
    private void Update()
    {
        if (agent.velocity.x > 0.1f) {
            mv.setFacingDirection(1);
        }
        else if (agent.velocity.x < -0.1f) {
            mv.setFacingDirection(-1);
        }
        
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        

        // Check if entity has no health
        if (impState != ImpState.Dead && health.isEmpty())
        {
            // Call it's death method
            Die();

            // Prevent movement
            mv.Walk(0);

            // Stop nav agent
            agent.isStopped = true;

            // Give gravity
            body.gravityScale = 2.5f;

            // Destroy in 2 seconds
            Destroy(gameObject, 2f);

            // Add knockback to corpse
            if (target != null)
                displacable.triggerKnockback(400f, 2f, target.position);

            // Change state to dead
            impState = ImpState.Dead;
        }
    }

    private void FixedUpdate() {

        // Generate gold over time to give to shrines
        if (goldTimer > 0) {
            goldTimer -= Time.deltaTime;
        }
        else {
            heldGold += goldPerGeneration;
            goldTimer = goldGenerationRate;
        }

        switch(impState) {
            case ImpState.Idle:
                animationHandler.changeAnimationState(flyingAnimation);

                // Stop moving
                agent.isStopped = true;

                handleRetaliation();
                
                // If any shrines exist, go to them
                if (shrinesToPatrol.Count > 0) {
                    agent.isStopped = false;
                    impState = ImpState.Searching;
                }

                // Handle displacement
                if (displacable.isDisplaced()) {
                    if (displacable.isStunned())
                        resetValues();
                    animationHandler.changeAnimationState(stunnedAnimation);
                    impState = ImpState.Stunned;
                    break;
                }

            break;
            case ImpState.Searching:
                animationHandler.changeAnimationState(flyingAnimation);

                handleRetaliation();
                
                // If the shrine, you are going to is gone, IE destroyed
                if (shrinesToPatrol[currentShrineIndex] == null) {
                    // Remove that shrine from patrol route
                    shrinesToPatrol.RemoveAt(currentShrineIndex);
                    // Then move to next index
                    nextShrineIndex();
                }

                // If there are no shines left, be idle
                if (shrinesToPatrol.Count <= 0) {
                    impState = ImpState.Idle;
                    return;
                }

                // Move towards target
                var destination = shrinesToPatrol[currentShrineIndex].transform.position + Vector3.up * interactionOffset;
                agent.SetDestination(destination);
                if (Vector2.Distance(transform.position, destination) < attackRange) {
                    // Begin interacting
                    interactionTimer = interactionDuration;
                    impState = ImpState.Interacting;
                }

                // Handle displacement
                if (displacable.isDisplaced()) {
                    if (displacable.isStunned())
                        resetValues();
                        
                    rallyParticles.Stop();
                    animationHandler.changeAnimationState(stunnedAnimation);
                    impState = ImpState.Stunned;
                    break;
                }

            break;
            case ImpState.Interacting:
                animationHandler.changeAnimationState(flyingAnimation);

                // Fill Circle
                if (interactionTimer > 0) {
                    interactionTimer -= Time.deltaTime;
                    interactingCircle.fillAmount = 1 - interactionTimer / interactionDuration;
                }
                else {
                    interactionTimer = 0;
                    interactingCircle.fillAmount = 0;

                    // Interact with shrine
                    interactWithNearbyShrine();

                    // Move to next shrine
                    nextShrineIndex();

                    // Change state
                    impState = ImpState.Idle;
                }

                handleRetaliation();

                // Handle displacement
                if (displacable.isDisplaced()) {
                    if (displacable.isStunned())
                        resetValues();

                    rallyParticles.Stop();
                    animationHandler.changeAnimationState(stunnedAnimation);
                    impState = ImpState.Stunned;
                    break;
                }

            break;
            case ImpState.Rallying:
                animationHandler.changeAnimationState(rallyAnimation);

                // Don't move while rallying
                agent.isStopped = true;

                if (rallyTimer > 0) {
                    rallyTimer -= Time.deltaTime;
                }
                else {
                    rally();
                }

                // Handle displacement
                if (displacable.isDisplaced()) {
                    if (displacable.isStunned())
                        resetValues();

                    rallyParticles.Stop();
                    animationHandler.changeAnimationState(stunnedAnimation);
                    impState = ImpState.Stunned;
                    break;
                }

            break;
            case ImpState.Attacking:
                // If you are in the middle of an attack
                if (attackTimer > 0) {
                    animationHandler.changeAnimationState(attackAnimation);

                    attackTimer -= Time.deltaTime;
                    if (attackTimer < 0.45f) {
                        body.velocity = attackDirection * attackDashSpeed;
                    }
                }
                else {
                    // Reset body velocity
                    body.velocity = Vector2.zero;

                    // If target is dead, or un-aggro'd then go back to idle
                    if (target == null) {
                        impState = ImpState.Idle;
                        return;
                    }

                    // If you get farther than aggro range, remove target
                    if (Vector2.Distance(transform.position, target.position) > aggroRange) {
                        target = null;
                        impState = ImpState.Idle;
                        return;
                    }

                    // Regular animation
                    animationHandler.changeAnimationState(flyingAnimation);

                    // Face target
                    faceTarget();

                    // Cooldown between attacks
                    if (attackCooldownTimer > 0) {
                        attackCooldownTimer -= Time.deltaTime;
                    }

                    // Check if target is in range for attack
                    if (Vector2.Distance(transform.position, target.position) < attackRange) {
                        agent.isStopped = true;

                        // If the cooldown between attacks is over, then start new attack
                        if (attackCooldownTimer <= 0) {
                            // Start the attack
                            attack();
                        }
                    }
                    else {
                        // Else keep pathings towards target
                        agent.isStopped = false;
                        agent.SetDestination(target.position);
                    }

                    // Handle displacement
                    if (displacable.isDisplaced()) {
                        if (displacable.isStunned())
                            resetValues();
                        animationHandler.changeAnimationState(stunnedAnimation);
                        impState = ImpState.Stunned;
                        break;
                    }
                }
            break;
            case ImpState.Stunned:
                displacable.performDisplacement();

                if (!displacable.isDisplaced()) {
                    
                    impState = ImpState.Attacking;
                }

            break;
            case ImpState.Dead:
                animationHandler.changeAnimationState(deadAnimation);
                displacable.performDisplacement();
            break;
        }
    }

    private void nextShrineIndex() {
        // Increment to next shrine
        currentShrineIndex++;
        if (currentShrineIndex >= shrinesToPatrol.Count) {
            currentShrineIndex = 0;
        }
    }

    private void generatePatrolPath() {
        // Find all gold shrine and false shrine objects
        var goldShrines = (GoldShrine[])FindObjectsOfType(typeof(GoldShrine));
        var falseShrines = (FalseShrine[])FindObjectsOfType(typeof(FalseShrine));
        shrinesToPatrol.AddRange(goldShrines);
        shrinesToPatrol.AddRange(falseShrines);

        // One-liner randomize list
        shrinesToPatrol = shrinesToPatrol.OrderBy(x => Random.value).ToList();

        // Set target to first shrine
        currentShrineIndex = 0;
    }

    private void interactWithNearbyShrine() {
        // Check to see if entity is touching an interactable
        var hit = Physics2D.OverlapBox(collider2d.bounds.center, collider2d.bounds.size, 0, 1 << LayerMask.NameToLayer("Interactables"));
        if (hit) {
            if (hit.TryGetComponent(out GoldShrine goldShrine)) {
                goldShrine.addGold(heldGold);
                heldGold = 0;
            }
            else if (hit.TryGetComponent(out FalseShrine falseShrine)) {
                // Do nothing
            }
        }
    }

    private void handleRetaliation() {
        // If entity was attacked...
        if (attacker != null) {
            interactingCircle.fillAmount = 0; // Reset circle
            target = attacker;
            if(!hasRallied) {
                // Set has rallied to true
                hasRallied = true;

                // Start particle effects
                rallyParticles.Play();

                // Set timer and change state
                rallyTimer = rallyDuration;
                impState = ImpState.Rallying;
            }
            else {
                aggroOn(target);
            }
            attacker = null;
        }
    }

    private void attack() {
        attackTimer = attackDuration;
        attackCooldownTimer = attackCooldown;
        attackDirection = (target.position - transform.position).normalized;
    }

    private void rally() {
        // Stop particle effects
        rallyParticles.Stop();

        // Check for all nearby Imps and make them aggro on the target
        var hits = Physics2D.OverlapCircleAll(transform.position, rallyRadius, 1 << LayerMask.NameToLayer("Enemies"));
        foreach (var hit in hits) {
            if (hit.TryGetComponent(out ImpAI impAI)) {
                impAI.aggroOn(target);
            }
        }
    }

    public void aggroOn(Transform entity) {
        interactingCircle.fillAmount = 0; // Reset circle
        target = entity;
        impState = ImpState.Attacking;
    }

    protected override void resetValues()
    {
        base.resetValues();
        rallyTimer = rallyDuration;
        rallyParticles.Stop();
    }

    private void OnDestroy() {
        // Create corpse
        if (health.currentHealth <= 0)
            GameManager.instance.spawnCorpse(gameObject);
    }

    protected override void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rallyRadius);
    }
}

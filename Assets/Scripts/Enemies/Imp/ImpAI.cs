using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;

public class ImpAI : EnemyAI
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Health health;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private CompositeCollider2D platformCollider;
    [SerializeField] private int heldGold;
    [SerializeField] private float goldGenerationRate;
    [SerializeField] private List<Shrine> shrinesToPatrol;
    [SerializeField] private int currentShrineIndex;
    [SerializeField] private Image interactingCircle;
    [SerializeField] private float interactionDuration;
    [SerializeField] private float interactionOffset;
    private float interactionTimer;
    private bool hasRallied;
    [SerializeField] private float rallyRadius;
    [SerializeField] private float rallyDuration;
    private float rallyTimer;

    private enum ImpState {
        Idle,
        Searching,
        Rallying,
        Attacking,
        Interacting,
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

        health = GetComponent<Health>();
        collider2d = GetComponent<Collider2D>();

        // Don't interact with platforms
        platformCollider = GameObject.Find("Platform").GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(platformCollider, collider2d);
        
        interactingCircle.fillAmount = 0;
        heldGold = 0;

        generatePatrolPath();

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

        // Check if entity has no health
        if (health.isEmpty()) {
            impState = ImpState.Dead;
        }
    }

    private void FixedUpdate() {
        switch(impState) {
            case ImpState.Idle:
                // If the shrine at this index is not null, then go to it
                if (shrinesToPatrol[currentShrineIndex] != null) {
                    impState = ImpState.Searching;
                }

            break;
            case ImpState.Searching:
                // Move towards target
                var destination = shrinesToPatrol[currentShrineIndex].transform.position + Vector3.up * interactionOffset;
                agent.SetDestination(destination);
                if (Vector2.Distance(transform.position, destination) < minAttackRange) {
                    // Begin interacting
                    interactionTimer = interactionDuration;
                    impState = ImpState.Interacting;
                }

            break;
            case ImpState.Interacting:
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

                    // Increment to next shrine
                    currentShrineIndex++;
                    if (currentShrineIndex >= shrinesToPatrol.Count) {
                        currentShrineIndex = 0;
                    }

                    // Change state
                    impState = ImpState.Idle;
                }

            break;
            case ImpState.Rallying:
                if (rallyTimer > 0) {
                    rallyTimer -= Time.deltaTime;
                }
                else {
                    rally();
                    impState = ImpState.Attacking;
                }
            break;
            case ImpState.Attacking:

                if (attackTimer > 0) {
                    attackTimer -= Time.deltaTime;
                }
                else {
                    if (Vector2.Distance(transform.position, target.position) < minAttackRange) {
                        attack();
                    }
                    else {
                        agent.SetDestination(target.position);
                    }
                }
            break;
            case ImpState.Dead:
            
            break;
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
            target = attacker;
            if(!hasRallied) {
                rallyTimer = rallyDuration;
                impState = ImpState.Rallying;
            }
            else {
                impState = ImpState.Attacking;
            }
            attacker = null;
        }
    }

    private void rally() {
        var hits = Physics2D.OverlapCircleAll(transform.position, rallyRadius, 1 << LayerMask.NameToLayer("Enemies"));
        foreach (var hit in hits) {
            if (hit.TryGetComponent(out ImpAI impAI)) {
                impAI.isAttacked(attacker);
            }
        }
    }

    public void attack() {
        animationHandler.changeAnimationState("Attack");
        attackTimer = animationHandler.getAnimationDuration();
    }

    private void OnDrawGizmosSelected() {
        if (shrinesToPatrol != null) {
            foreach (var shrine in shrinesToPatrol) {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(shrine.transform.position, 0.4f);
            }
        }
    }
}

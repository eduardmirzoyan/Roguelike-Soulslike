using UnityEngine;
using UnityEngine.AI;

public abstract class Familiar : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Rigidbody2D body;
    [SerializeField] protected Movement mv;


    [Header("Familiar Settings")]
    [SerializeField] protected Transform homePosition;
    [SerializeField] protected GameObject owner;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float spacing;

    // Personal vars
    protected int position;
    protected Vector2 movement;

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        mv = GetComponent<Movement>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected virtual void Update()
    {
        agent.SetDestination(homePosition.position);

        if (agent.velocity.x > 0.1f) {
            mv.setFacingDirection(1);
        }
        else if (agent.velocity.x < -0.1f) {
            mv.setFacingDirection(-1);
        }

        // var targetPosition = (Vector2)homePosition.position + Vector2.right * spacing * (position + 1) * -owner.GetComponent<Movement>().getFacingDirection();

        // var hit = Physics2D.Raycast(homePosition.position, Vector2.right * -owner.GetComponent<Movement>().getFacingDirection(), spacing * position);
        // if (hit)
        //     targetPosition = hit.point;

        // Vector3 direction = targetPosition - (Vector2)transform.position;
        // if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
        //     direction = Vector2.zero;
        // direction.Normalize();
        // movement = direction;
    }

    public void setHome(Transform newHome) => homePosition = newHome;

    public void setOwner(GameObject newOwner) => owner = newOwner;

    public void setPosition(int newPos) => position = newPos;

    public int getPosition() => position;

    public void Despawn()
    {
        Destroy(gameObject);
    }
}

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

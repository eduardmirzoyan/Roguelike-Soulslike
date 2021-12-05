using UnityEngine;

public abstract class Familiar : MonoBehaviour
{
    [Header("Familiar Settings")]
    [SerializeField] protected Transform homePosition;
    [SerializeField] protected GameObject owner;
    [SerializeField] protected float movespeed;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float spacing;
    protected int position;
    protected Rigidbody2D body;
    protected Movement mv;
    protected Vector2 movement;

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        mv = GetComponent<Movement>();
    }

    protected virtual void FixedUpdate()
    {
        moveFamiliar(movement);
    }

    protected virtual void Update()
    {
        var targetPosition = (Vector2)homePosition.position + Vector2.right * spacing * (position + 1) * -owner.GetComponent<Movement>().getFacingDirection();

        var hit = Physics2D.Raycast(homePosition.position, Vector2.right * -owner.GetComponent<Movement>().getFacingDirection(), spacing * position);
        if (hit)
            targetPosition = hit.point;

        Vector3 direction = targetPosition - (Vector2)transform.position;
        if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
            direction = Vector2.zero;
        direction.Normalize();
        movement = direction;
    }

    protected void moveFamiliar(Vector2 direction)
    {
        body.MovePosition((Vector2)transform.position + (direction * movespeed * Time.deltaTime));
        mv.setFacingDirection(direction.x);
    }

    public void setHome(Transform newHome) => homePosition = newHome;

    public void setOwner(GameObject newOwner) => owner = newOwner;

    public void setPosition(int newPos) => position = newPos;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody2D body;

    [Header("Movement")]
    [SerializeField] protected float movespeed = 10;
    [SerializeField] protected float jumpPower = 10;

    [Header("Collision Detection")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform wallCheck;

    [Header("Settings")]
    [SerializeField] protected Vector2 groundCheckBoxSize;
    [SerializeField] protected float wallcheckRadius;
    private float externalSpeedBonuses;

    [Header("Custom Settings")]
    [SerializeField] protected bool isImmobile = false;
    [SerializeField] protected bool flying;
   
    // Helper variables
    protected bool isFacingRight = true;

    protected void Start()
    {
        body = GetComponent<Rigidbody2D>(); // Gets body physics handler
    }

    public void Walk(float direction) // From 0 to 1
    {
        if (isImmobile)
            return;

        body.velocity = new Vector2(direction * movespeed * Time.deltaTime, body.velocity.y); // Actually moves the character

        if (direction > 0.1f && !isFacingRight || direction < -0.1f && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
            if (TryGetComponent(out DustTrail dustTrail))
                dustTrail.createDust();
        }
    }

    public void WalkNoTurn(float direction) {
        if (isImmobile)
            return;

        body.velocity = new Vector2(direction * movespeed * Time.deltaTime, body.velocity.y); // Actually moves the character
    }

    public void WalkAtSpeed(float direction, float movespeed) // From 0 to 1
    {
        if (isImmobile)
            return;

        body.velocity = new Vector2(direction * movespeed * Time.deltaTime, body.velocity.y); // Actually moves the character

        if (direction > 0.1f && !isFacingRight || direction < -0.1f && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
            if (TryGetComponent(out DustTrail dustTrail))
                dustTrail.createDust();
        }
    }

    public void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpPower); // Vector2.up * jumpPower;
        if (TryGetComponent(out DustTrail dustTrail))
            dustTrail.createDust();
    }

    public bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0, groundLayer) && body.velocity.y < 0.1f;
    }

    public bool onWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallcheckRadius, wallLayer);
    }

    public void setMoveSpeed(float newSpeed) {
        movespeed = newSpeed;
    }

    public float getMovespeed() {
        return movespeed;
    }

    // Zeros horizontal movement
    public void Stop()
    {
        body.velocity = new Vector2(0, body.velocity.y);
    }

    public void walkBackwards(float direction)
    {
        if (isImmobile)
            return;

        body.velocity = new Vector2(direction * (movespeed / 2) * Time.deltaTime , body.velocity.y); // Actually moves the character

        if (direction > 0.1f && isFacingRight || direction < -0.1f && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
            var dustTrail = GetComponent<DustTrail>();
            if (dustTrail != null)
                dustTrail.createDust();
        }
    }

    // Returns 1 or -1
    public int getFacingDirection()
    {
        return isFacingRight ? 1 : -1;
    }

    public void setFacingDirection(float direction)
    {
        if (direction < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.Rotate(0f, 180f, 0f);
        }
        else if (direction > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
    }
    public bool checkFalling()
    {
        return !isGrounded() && body.velocity.y < -0.01f;
    }

    public bool checkRising()
    {
        return !isGrounded() && body.velocity.y > 0.01f;
    }

    public void dash(float dashSpeed, float direction)
    {
        body.velocity = new Vector2(direction * Time.deltaTime * dashSpeed, body.velocity.y);
    }

    public void jumpReposition(float xVel, float jumpPower)
    {
        body.velocity = new Vector2(xVel, jumpPower);
    }

    private void OnDrawGizmosSelected()
    {
        if (!flying) {
            Gizmos.DrawWireCube(groundCheck.transform.position, groundCheckBoxSize);
            Gizmos.DrawWireSphere(wallCheck.transform.position, wallcheckRadius);
        }
    }
}

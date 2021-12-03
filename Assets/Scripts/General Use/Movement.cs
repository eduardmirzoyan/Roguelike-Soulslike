using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody2D body;
    [SerializeField] protected Animator animator;

    [Header("Movement")]
    [SerializeField] protected float movespeed = 10;
    [SerializeField] protected float jumpPower = 10;

    [Header("Collision Detection")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected Transform backWallCheck;
    [SerializeField] protected Transform dropCheck; // only for enemies

    [SerializeField] protected Vector2 groundCheckBoxSize;
    [SerializeField] protected float wallcheckRadius;
    [SerializeField] protected Vector2 dropCheckBoxSize;

    [SerializeField] public float speedConstant;
    private float externalSpeedBonuses;

    [Header("Custom Settings")]
    [SerializeField] protected bool isImmobile = false;
    [SerializeField] protected bool enableRunningAnimations = false;
    [SerializeField] protected bool enableFallingRisingAnimations = false;
   
    // Helper variables
    protected bool isFacingRight = true;

    protected void Start()
    {
        body = GetComponent<Rigidbody2D>(); // Gets body physics handler
        animator = GetComponent<Animator>();
        speedConstant = 0.02f * 35;
    }


    protected virtual void Update()
    {
        var stats = GetComponent<CombatStats>();
        if (stats != null)
            externalSpeedBonuses = 1 + stats.movespeedMultiplier;
        else
            externalSpeedBonuses = 1;

        if (enableRunningAnimations)
        {
            animator.SetBool("run", Mathf.Abs(body.velocity.x) > 0.1f); // set run to the answer of the logical check
        }
        if (enableFallingRisingAnimations)
        {
            if (checkFalling())
                animator.SetTrigger("falling");
            if (checkRising())
                animator.SetTrigger("rising");
        }
    }

    public void Walk(float direction) // From 0 to 1
    {
        if (isImmobile)
            return;


        body.velocity = new Vector2(direction * movespeed * speedConstant * externalSpeedBonuses, body.velocity.y); // Actually moves the character

        if (direction > 0.1f && !isFacingRight || direction < -0.1f && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
            var dustTrail = GetComponent<DustTrail>();
            if (dustTrail != null)
                dustTrail.createDust();
        }
    }


    // Temp walk
    public void WalkAtSpeed(float direction, float movespeed) // From 0 to 1
    {
        if (isImmobile)
            return;

        body.velocity = new Vector2(direction * movespeed * speedConstant * externalSpeedBonuses, body.velocity.y); // Actually moves the character

        if (direction > 0.1f && !isFacingRight || direction < -0.1f && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
            var dustTrail = GetComponent<DustTrail>();
            if (dustTrail != null)
                dustTrail.createDust();
        }
    }

    public void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpPower); // Vector2.up * jumpPower;
        var dustTrail = GetComponent<DustTrail>();
        if (dustTrail != null)
            dustTrail.createDust();
    }

    public bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0, groundLayer);
    }

    public bool onWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallcheckRadius, wallLayer);
    }

    public bool backAgainstWall()
    {
        return Physics2D.OverlapCircle(backWallCheck.position, wallcheckRadius, wallLayer);
    }

    // Needs rework
    public bool aboutToDrop()
    {
        return !onWall() && !Physics2D.OverlapBox(dropCheck.position, dropCheckBoxSize, 0, groundLayer);
    }

    // Zeros horizontal movement
    public void Stop()
    {
        body.velocity = new Vector2(0, body.velocity.y);
    }

    // Rework? NAH
    public void walkBackwards(float direction)
    {
        if (isImmobile)
            return;

        body.velocity = new Vector2(direction * (movespeed / 2) * speedConstant , body.velocity.y); // Actually moves the character

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
        body.MovePosition(new Vector2(body.position.x + 0.01f * direction * speedConstant * dashSpeed, body.position.y));
    }

    public void dashWithVelocity(float dashSpeed, float direction)
    {
        body.velocity = new Vector2(direction * speedConstant * dashSpeed, body.velocity.y);
    }

    public void angledDash(float dashSpeed, float direction)
    {
        body.velocity = new Vector2(direction * speedConstant * dashSpeed, dashSpeed / 4);
    }

    public void jumpReposition(float xVel)
    {
        body.velocity = new Vector2(xVel, jumpPower);
    }

    public Vector2 getPosition() => body.position;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(groundCheck.transform.position, groundCheckBoxSize);
        Gizmos.DrawWireSphere(wallCheck.transform.position, wallcheckRadius);
        //Gizmos.DrawWireCube(dropCheck.transform.position, dropCheckBoxSize);
    }

    public void setImmobile(bool newState) => isImmobile = newState;
}

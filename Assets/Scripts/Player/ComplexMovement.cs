using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexMovement : Movement
{
    [Header("Complex Movement Variables")]
    [SerializeField] protected float wallSlidingSpeed;
    [SerializeField] protected float xWallForce;
    [SerializeField] protected float yWallForce;
    [SerializeField] protected int wallJumpStaminaDrain;
    [SerializeField] public float wallJumpTime;
    [SerializeField] protected float sprintingMultiplier;
    [SerializeField] protected float crouchWalkingMultiplier;

    [SerializeField] private bool enableWallAnimations;

    public void sprint(float direction)
    {
        if (isImmobile)
            return;

        body.velocity = new Vector2(direction * movespeed * Time.deltaTime * sprintingMultiplier, body.velocity.y); // Actually moves the character

        if (direction > 0.1f && !isFacingRight || direction < -0.1f && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    public void crouchWalk(float direction)
    {
        if (isImmobile)
            return;

        body.velocity = new Vector2(direction * movespeed * Time.deltaTime * crouchWalkingMultiplier, body.velocity.y); // Actually moves the character

        if (direction > 0.1f && !isFacingRight || direction < -0.1f && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    public void wallSlide()
    {
        body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
    }

    public void wallJump()
    {
        body.velocity = new Vector2(xWallForce * movespeed * Time.deltaTime * -getFacingDirection(), yWallForce * jumpPower);
        setFacingDirection(-getFacingDirection());
    }

    public int getWallSlideStaminaDrain() => wallJumpStaminaDrain;
}

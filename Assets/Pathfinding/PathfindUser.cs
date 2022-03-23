using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindUser : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform target;
    [SerializeField] private PathfindingMap pathfinderAI;
    [SerializeField] private Movement mv; 

    [SerializeField] private float minTargetDistance;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckWidth;
    [SerializeField] private float waypointSize;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float padding;
    [SerializeField] private float movespeed;
    [SerializeField] private float jumpHeight;

    [SerializeField] private Queue<Vector3> currentPath;
    [SerializeField] private Vector3 currentTarget;

    // Relationship between jumpheight and y velocity is y = 1.67x + 3
    [SerializeField] private Dictionary<int, float> jumpHeightToVelocityPairs;
    private bool isFacingLeft = true;
    [SerializeField] private Animator animator;
    [SerializeField] private PlatformHandler platformHandler;

    [SerializeField] private bool manualControl;

    private bool isJump;
    private bool isDrop;
    private bool properjump;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        platformHandler = GetComponent<PlatformHandler>();
        mv = GetComponent<Movement>();
        currentPath = new Queue<Vector3>();
        currentTarget = Vector3.back;

        // Manually added the perfect jump velocities for a standing jump
        jumpHeightToVelocityPairs = new Dictionary<int, float>();
        jumpHeightToVelocityPairs.Add(1, 5f);
        jumpHeightToVelocityPairs.Add(2, 6.75f);
        jumpHeightToVelocityPairs.Add(3, 8.25f);
        jumpHeightToVelocityPairs.Add(4, 9.5f);
        jumpHeightToVelocityPairs.Add(5, 11.25f);
    }

    private void move(float direction) {
        mv.Walk(direction);
    }

    private void jump() {
        if (mv.isGrounded()) {
            if (Mathf.Abs(currentTarget.x) <= 1) {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeightToVelocityPairs[(int)currentTarget.z]);
            }
            else {
                Vector2 optimal = pathfinderAI.getOptimalIntialVelocity(currentTarget.x, currentTarget.y, movespeed * Time.deltaTime, Physics2D.gravity.y);
                rb.velocity = new Vector2(rb.velocity.x, optimal.y);
            }
        }
    }

    // Update is called once per frame
    // private void Update()
    // {
    //     if (!manualControl) {
    //         if (Input.GetKeyDown(KeyCode.Mouse0) && isGrounded()) {
    //             Vector3 mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //             var hit = Physics2D.Raycast(mousePosition, Vector2.down, 1000, groundMask);
    //             if (hit) {
    //                 // Shift point down
    //                 hit.point = hit.point - Vector2.up * 0.3f;
    //                 travelTo(hit.point);
    //             }
    //         }
    //     }
    //     else {
    //         // Used to test jumping height
    //         if (!Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space) && isGrounded()) {
    //             isJump = true;
    //         }
    //     }
    // }

    private void FixedUpdate() {
        // if (!manualControl && currentPath != null) {
        //     moveToLocation();
        // }
        // else {
        //     if (Input.GetKey(KeyCode.A)) {
        //         move(-1);
        //     }
        //     else if (Input.GetKey(KeyCode.D)) {
        //         move(1);
        //     }
        //     else {
        //         move(0);
        //     }

        //     if (isJump) {
        //         rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        //         isJump = false;
        //     }
        // }
        

    }

    public void setPathTo(Vector3 location) {
        // Raycast downward to ground
        var startHit = Physics2D.Raycast(transform.position, Vector2.down, 1000f, groundMask);
        var endHit = Physics2D.Raycast(location, Vector2.down, 1000f, groundMask);

        // If both hit, then find path
        if (startHit && endHit) {
            startHit.point += Vector2.up * 0.5f;
            endHit.point += Vector2.up * 0.5f;

            Debug.DrawLine(transform.position, startHit.point, Color.red, 100f);
            Debug.DrawLine(location, endHit.point, Color.red, 100f);

            currentPath = pathfinderAI.findPath(startHit.point, endHit.point);
            if (currentPath.Count == 0) {
                print("No path to location: " + location);
                return;
            }
            nextTarget();
        }
    }

    private void nextTarget() {
        if (currentPath.Count == 0) {
            currentTarget = Vector3Int.back;
            return;
        }

        // Get center of the cell instead of the bottom left corner
        currentTarget = pathfinderAI.getCellCenter(currentPath.Dequeue());

        // If currentTarget.z > 0, it indicates a jump
        if (currentTarget.z > 0 && !isJump) {
            isJump = true;
        }

        if (currentTarget.z == -2) {
            isDrop = true;
        }
    }

    public void moveToLocation() {
        // Check if currentTarget is set and you are not jumping
        if (currentTarget.z != -1  && !isJump && !isDrop) {
            // Check if the location that you need to go to is to the left or right of your current position
            if (currentTarget.x - padding > transform.position.x) {
                move(1);
            }
            else if(currentTarget.x + padding < transform.position.x) {
                move(-1);
            }
            else {
                move(0);
            }

            if (Vector2.Distance(transform.position, currentTarget) < minTargetDistance && mv.isGrounded()) {
                nextTarget();
            }
        }
        else {
            move(0);
        }

        if (isJump) {
            var bound = GetComponent<Collider2D>().bounds;
            // Make sure the block above you is open, so you can jump
            var hit = Physics2D.BoxCast(bound.center, bound.size, 0, Vector2.up, .5f, groundMask);
            if (false) {
                // If you do hit a wall, then move in the opposite direction until an open spot is found
                if (Mathf.Sign(currentTarget.x) >= 0) {
                    move(-1);
                }
                else {
                    move(1);
                }
                // Make sure you do a standing jump instead of a running one
                properjump = true;
            }
            else {
                jump();
                if(properjump) {
                    if (rb.velocity.y < 5.5f) {
                        properjump = false;
                    }
                }
                else {
                    nextTarget();
                    // Check for recalibration if you don't make the jump
                    //StartCoroutine(recalibrateIn(5f));
                    isJump = false;
                }
            }
        }

        if (isDrop) {
            platformHandler.dropFromPlatform();
            isDrop = false;
        }
    }

    private bool isGrounded() => Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, 0.2f), 0, groundMask) && Mathf.Abs(rb.velocity.y) <= 0.05f;
    
    // Checks to see if after a second, if the target has not changed, then generate new path to the end point
    private IEnumerator recalibrateIn(float waitTime) {
        var currentPoint = currentTarget;

        yield return new WaitForSeconds(waitTime);

        // Checks to see if the stored waypoint has changed in the wait time, if not, generate a new path
        if (currentPath.Count > 0 && currentTarget == currentPoint) {
            var endPoint = currentPath.Dequeue();
            while (currentPath.Count > 1) {
                currentPath.Dequeue();
            }

            print("Recalibrating...");
            setPathTo(endPoint - Vector3.up);
        }
    }

    public void stopTraveling() {
        currentPath = null;
        currentTarget = Vector3.back;
        mv.Walk(0);
    }
    public bool isDonePathing() => currentPath.Count == 0;

    private void OnDrawGizmos()
    { 
        Gizmos.color = Color.red;
        var col = GetComponent<Collider2D>();
        if (currentTarget.z >= 0) {
            Gizmos.DrawSphere(currentTarget, waypointSize);
        }
    }
}

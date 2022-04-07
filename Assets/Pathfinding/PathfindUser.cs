using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformHandler))]
public class PathfindUser : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PathfindingMap pathfindingMap;
    [SerializeField] private Movement mv;
    [SerializeField] private PlatformHandler platformHandler;
    [SerializeField] private Transform altTransform;

    [Header("Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float minTargetDistance = 0.3f;
    [SerializeField] private float padding = 0.05f;

    [Header("Debugging")]
    [SerializeField] private Vector3[] viewableQueue;
    [SerializeField] private Vector3 currentTarget;
    [SerializeField] private Dictionary<int, float> jumpHeightToVelocityPairs;

    // Private fields
    private Queue<Vector3> currentPath;
    private bool isJump;
    [SerializeField] private bool isDrop;
    private bool properjump;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platformHandler = GetComponent<PlatformHandler>();
        mv = GetComponent<Movement>();
        currentPath = new Queue<Vector3>();
        currentTarget = Vector3.back;
        pathfindingMap = GameObject.Find("Pathfinder Map").GetComponent<PathfindingMap>();
        // If alt transform is not set, then set it to the connected component's transform
        if (altTransform == null) {
            altTransform = transform;
        }

        groundMask = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Platform");

        // Manually added the perfect jump velocities for a standing jump
        jumpHeightToVelocityPairs = new Dictionary<int, float>();
        jumpHeightToVelocityPairs.Add(1, 5f);
        jumpHeightToVelocityPairs.Add(2, 6.75f);
        jumpHeightToVelocityPairs.Add(3, 8.25f);
        jumpHeightToVelocityPairs.Add(4, 9.5f);
        jumpHeightToVelocityPairs.Add(5, 11.25f);
    }

    private void jump() {
        if (mv.isGrounded()) {
            if (Mathf.Abs(currentTarget.x) <= 1) {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeightToVelocityPairs[(int)currentTarget.z]);
            }
            else {
                Vector2 optimal = pathfindingMap.getOptimalIntialVelocity(currentTarget.x, currentTarget.y, mv.getMovespeed() * Time.deltaTime, Physics2D.gravity.y);
                rb.velocity = new Vector2(rb.velocity.x, optimal.y);
            }
        }
    }

    public void setPathTo(Vector3 location) {
        // Raycast downward to ground
        var startHit = Physics2D.Raycast(altTransform.position, Vector2.down, 1000f, groundMask);
        var endHit = Physics2D.Raycast(location, Vector2.down, 1000f, groundMask);

        // If both hit, then find path
        if (startHit && endHit) {
            startHit.point += Vector2.up * 0.5f;
            endHit.point += Vector2.up * 0.5f;

            currentPath = pathfindingMap.findPath(startHit.point, endHit.point);
            if (currentPath.Count == 0) {
                print("No path to location: " + location);
                Debug.DrawRay(location, Vector3.up, Color.green, 10f);
            }
            nextTarget();
        }
    }

    public bool isPointValid(Vector3 location) {
        // Raycast downward to ground
        var endHit = Physics2D.Raycast(location, Vector2.down, 1000f, groundMask);

        // If ground has been found
        if (endHit) {
            // Should be 0.5f off the ground
            endHit.point += Vector2.up * 0.5f;

            // Make sure there is no wall within half a cell to the left or right
            var leftEndSideCheck = Physics2D.Raycast(endHit.point, Vector2.left, 0.5f, groundMask);
            var rightEndSideCheck = Physics2D.Raycast(endHit.point, Vector2.right, 0.5f, groundMask);

            if (isPointInsideMap(endHit.point) || leftEndSideCheck || rightEndSideCheck) {
                return false;
            }

            return true;
        }
        return false;
    }

    public List<Vector3> getAllOpenTiles(Vector3 center, int radius) {
        return pathfindingMap.getAllOpenCells(center, radius);
    }

    public bool isPointInsideMap(Vector3 point) {
        return pathfindingMap.isPointInsideMap(point);
    }

    private void nextTarget() {
        if (currentPath.Count == 0) {
            currentTarget = Vector3Int.back;
            return;
        }

        currentTarget = currentPath.Dequeue();

        viewableQueue = currentPath.ToArray();

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
        if (currentTarget != Vector3.back  && !isJump && !isDrop) {
            // Check if the location that you need to go to is to the left or right of your current position
            if (currentTarget.x - padding > altTransform.position.x) {
                
                mv.Walk(1);
            }
            else if(currentTarget.x + padding < altTransform.position.x) {
                
                mv.Walk(-1);
            }
            else {
                mv.Walk(0);
            }

            if (Vector2.Distance(altTransform.position, currentTarget) < minTargetDistance && mv.isGrounded()) {
                nextTarget();
            }
        }
        else {
            mv.Walk(0);
        }

        if (isJump) {
            jump();
            nextTarget();
            // Check for recalibration if you don't make the jump
            //StartCoroutine(recalibrateIn(5f));
            isJump = false;
        }

        if (isDrop) {
            platformHandler.dropFromPlatform();
            nextTarget();
            isDrop = false;
        }
    }
    
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
    public bool isDonePathing() => currentTarget == Vector3Int.back;

    private void OnDrawGizmosSelected()
    { 
        Gizmos.color = Color.green;
        if (currentPath != null && currentPath.Count > 0) {
            foreach (var path in currentPath) {
                Gizmos.DrawSphere(path, minTargetDistance);
            }
        }
    }
}

using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GoblinCamp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float timeAtCamp;
    [SerializeField] private Queue<GoblinAI> goblinsAtCamp;

    [Header("Debugging")]
    [SerializeField] private List<Transform> patrolRoute;
    [SerializeField] private GoblinCamp nextCamp;
    [SerializeField] private bool main;
    [SerializeField] private GoblinAI sentGoblin;

    // Start is called before the first frame update
    private void Start()
    {  
        // Intialize
        goblinsAtCamp = new Queue<GoblinAI>();

        // If this is the main camp, set all the other camps based on this one
        if (main) {
            // Create the patrol route
            patrolRoute = new List<Transform>();

            // Get all the goblin camps in the map
            var goblinCamps = GameObject.FindObjectsOfType<GoblinCamp>();

            // Get the transforms of all the camps
            Transform[] goblinCampTransforms = new Transform[goblinCamps.Length];
            for (int i = 0; i < goblinCamps.Length; i++)
                goblinCampTransforms[i] = goblinCamps[i].transform;

            // Sort the transforms in clockwise order
            Array.Sort(goblinCampTransforms, new ClockwiseComparer(transform.position));

            // Add the camps as the patrol route
            patrolRoute.AddRange(goblinCampTransforms);

            // Set all camp's next campsite
            for (int i = 0; i < goblinCampTransforms.Length; i++)
            {
                // Set camp
                goblinCamps[i].setPatrolRoute(goblinCampTransforms);
            }
        }
    }

    private void Update() {

        // If there is no currently patrolling goblin from this camp, or he has been killed, then send a new one
        if (sentGoblin == null) {
            if (goblinsAtCamp.Count < 1) {
                //  Do nothing if there are no goblins at camp
                return;
            }
            // Remove goblin from queue and track
            sentGoblin = goblinsAtCamp.Dequeue();
            
            // Issue command to goblin in 5 seconds
            StartCoroutine(sendGoblinIn(timeAtCamp));
        }
    }

    public void setPatrolRoute(Transform[] goblinCampTransforms) {
        for (int i = 0; i < goblinCampTransforms.Length; i++)
        {
            // If the camp is this one
            if (goblinCampTransforms[i].Equals(transform)) {
                // Set the next camp in loop as the next camp
                i++;
                if (i >= goblinCampTransforms.Length) {
                    i = 0;
                }
                nextCamp = goblinCampTransforms[i].GetComponent<GoblinCamp>();
                return;
            }
        }
    }

    public void joinCamp(GoblinAI goblin) {
        // print(goblin.name + " joined camp: " + name);
        // Add goblin to queue
        goblinsAtCamp.Enqueue(goblin);
    }

    private IEnumerator sendGoblinIn(float time) {  
        yield return new WaitForSeconds(time);
        if (sentGoblin == null) {
            print("Goblin that was supposed to be sent is gone!");
        }

        // Send goblin to next camp
        sentGoblin.patrolTo(nextCamp.transform);
    }

    public void leaveCamp(GoblinAI goblin) {
        if (sentGoblin = goblin) {
            sentGoblin = null;
        }
    }
}

public class ClockwiseComparer : IComparer<Transform>
{
    private Vector2 m_Origin;

    #region Properties

    /// <summary>
    ///     Gets or sets the origin.
    /// </summary>
    /// <value>The origin.</value>
    public Vector2 origin { get { return m_Origin; } set { m_Origin = value; } }

    #endregion

    /// <summary>
    ///     Initializes a new instance of the ClockwiseComparer class.
    /// </summary>
    /// <param name="origin">Origin.</param>
    public ClockwiseComparer(Vector2 origin)
    {
        m_Origin = origin;
    }

    #region IComparer Methods

    /// <summary>
    ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="first">First.</param>
    /// <param name="second">Second.</param>
    public int Compare(Transform first, Transform second)
    {
        return IsClockwise(first.position, second.position, m_Origin);
    }

    #endregion

    /// <summary>
    ///     Returns 1 if first comes before second in clockwise order.
    ///     Returns -1 if second comes before first.
    ///     Returns 0 if the points are identical.
    /// </summary>
    /// <param name="first">First.</param>
    /// <param name="second">Second.</param>
    /// <param name="origin">Origin.</param>
    public static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
    {
        Vector2 dir1 = first - origin;
        Vector2 dir2 = second - origin;
        float angle1 = Mathf.Atan2(dir1.x, dir1.y) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan2(dir2.x, dir2.y) * Mathf.Rad2Deg;

        if (angle1 < 0f) angle1 += 360;
        if (angle2 < 0f) angle2 += 360;

        return angle1.CompareTo(angle2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowThePath : MonoBehaviour
{

    // Array of waypoints to walk from one to the next one
    [SerializeField]
    private Transform[] Circle;

    // Walk speed that can be set in Inspector
    [SerializeField]
    private float moveSpeed = 2f;

    // Index of current waypoint from which Enemy walks
    // to the next one
    private int CircleIndex = 0;

    // Use this for initialization
    private void Start()
    {

        // Set position of Enemy as position of the first waypoint
        transform.position = Circle[CircleIndex].transform.position;
    }

    // Update is called once per frame
    private void Update()
    {

        // Move Enemy
        Move();
    }

    // Method that actually make Enemy walk
    private void Move()
    {
        // If Enemy didn't reach last waypoint it can move
        // If enemy reached last waypoint then it stops
        if (CircleIndex <= Circle.Length - 1)
        {

            // Move Enemy from current waypoint to the next one
            // using MoveTowards method
            transform.position = Vector2.MoveTowards(transform.position,
               Circle[CircleIndex].transform.position,
               moveSpeed * Time.deltaTime);

            // If Enemy reaches position of waypoint he walked towards
            // then waypointIndex is increased by 1
            // and Enemy starts to walk to the next waypoint
            if (transform.position == Circle[CircleIndex].transform.position)
            {
                CircleIndex += 1;
            }
        }
    }
}

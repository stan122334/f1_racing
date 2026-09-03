using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowThePath : MonoBehaviour
{
    [SerializeField]
    private Transform[] Circle;

    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float acceleration = 5f;

    [SerializeField]
    private float braking = 10f;

    // How far ahead we look to see if a corner is coming
    [SerializeField]
    private int lookAhead = 5;

    // Maximum speed
    [SerializeField]
    private float maxSpeed = 15f;

    // Minimum corner speed
    [SerializeField]
    private float minSpeed = 4f;

    private int CircleIndex = 0;

    private void Start()
    {
        transform.position = Circle[CircleIndex].position;
    }

    private void Update()
    {
        Move();
        ChangeSpeed();
    }

    private void Move()
    {
        // Move towards the current waypoint
        transform.position = Vector2.MoveTowards(
            transform.position,
            Circle[CircleIndex].position,
            moveSpeed * Time.deltaTime
        );

        // When we reach the waypoint
        if (transform.position == Circle[CircleIndex].position)
        {
            CircleIndex++;

            // If we reach the end of the track,
            // go back to the first waypoint
            if (CircleIndex >= Circle.Length)
            {
                CircleIndex = 0;
            }
        }
    }

    private void ChangeSpeed()
    {
        // Make sure we have enough waypoints
        if (Circle.Length < 2)
        {
            return;
        }

        // Get the current waypoint and wrap around the track
        int currentIndex = CircleIndex;
        int nextIndex = (CircleIndex + 1) % Circle.Length;

        // Look ahead and wrap around the track
        int futureIndex = (CircleIndex + lookAhead) % Circle.Length;
        int futurePreviousIndex =
            (CircleIndex + lookAhead - 1) % Circle.Length;

        // Direction from current waypoint to the next waypoint
        Vector2 currentDirection =
            Circle[nextIndex].position -
            Circle[currentIndex].position;

        // Direction further ahead
        Vector2 futureDirection =
            Circle[futureIndex].position -
            Circle[futurePreviousIndex].position;

        // Calculate how much the track is turning
        float cornerAngle =
            Vector2.Angle(currentDirection, futureDirection);

        // Convert corner angle into a speed
        float targetSpeed = Mathf.Lerp(
            maxSpeed,
            minSpeed,
            cornerAngle / 90f
        );

        // Keep the speed inside our limits
        targetSpeed = Mathf.Clamp(
            targetSpeed,
            minSpeed,
            maxSpeed
        );

        // Accelerate
        if (moveSpeed < targetSpeed)
        {
            moveSpeed = Mathf.MoveTowards(
                moveSpeed,
                targetSpeed,
                acceleration * Time.deltaTime
            );
        }

        // Brake
        else if (moveSpeed > targetSpeed)
        {
            moveSpeed = Mathf.MoveTowards(
                moveSpeed,
                targetSpeed,
                braking * Time.deltaTime
            );
        }
    }
}
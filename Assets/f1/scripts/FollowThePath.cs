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
        if (CircleIndex <= Circle.Length - 1)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                Circle[CircleIndex].position,
                moveSpeed * Time.deltaTime
            );

            if (transform.position == Circle[CircleIndex].position)
            {
                CircleIndex++;
            }
        }
    }

    private void ChangeSpeed()
    {
        // We need enough waypoints to look ahead
        if (CircleIndex + lookAhead >= Circle.Length)
        {
            return;
        }

        // Direction from current waypoint to the next waypoint
        Vector2 currentDirection =
            Circle[CircleIndex + 1].position -
            Circle[CircleIndex].position;

        // Direction further ahead
        Vector2 futureDirection =
            Circle[CircleIndex + lookAhead].position -
            Circle[CircleIndex + lookAhead - 1].position;

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
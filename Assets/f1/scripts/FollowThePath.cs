using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowThePath : MonoBehaviour
{
    [SerializeField]
    private Transform[] Circle;

    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float acceleration = 5f;

    [SerializeField]
    private float braking = 8f;

    [SerializeField]
    private float[] targetSpeeds;

    private int CircleIndex = 0;

    private void Start()
    {
        transform.position = Circle[CircleIndex].position;

        // Start at the first target speed
        if (targetSpeeds.Length > 0)
        {
            moveSpeed = targetSpeeds[0];
        }
    }

    private void Update()
    {
        Move();
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

            // Reached waypoint
            if (transform.position == Circle[CircleIndex].position)
            {
                CircleIndex++;

                // Get the target speed for the next waypoint
                if (CircleIndex < targetSpeeds.Length)
                {
                    float targetSpeed = targetSpeeds[CircleIndex];

                    // Accelerate or brake
                    if (targetSpeed > moveSpeed)
                    {
                        moveSpeed = Mathf.MoveTowards(
                            moveSpeed,
                            targetSpeed,
                            acceleration * Time.deltaTime
                        );
                    }
                    else
                    {
                        moveSpeed = Mathf.MoveTowards(
                            moveSpeed,
                            targetSpeed,
                            braking * Time.deltaTime
                        );
                    }
                }
            }
        }
    }
}

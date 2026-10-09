
using UnityEngine;

public class FollowThePath : MonoBehaviour
{
    [Header("Trasa")]
    [SerializeField] private Transform[] Circle;

    [Header("Prêdkoœæ")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float braking = 10f;
    [SerializeField] private int lookAhead = 5;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float minSpeed = 4f;

    [Header("Losowoœæ (dla botów)")]
    [SerializeField] private bool randomize = true;
    [SerializeField] private Vector2 skillRange = new Vector2(0.92f, 1.0f);
    [SerializeField] private float paceJitter = 0.03f;
    [SerializeField] private float mistakeChance = 0.02f;
    [SerializeField] private float mistakeSpeedMultiplier = 0.6f;
    [SerializeField] private Vector2 mistakeDurationRange = new Vector2(1f, 2.5f);
    [SerializeField] private float maxLateralOffset = 0.5f;

    [Header("Race Start")]
    [SerializeField] private bool waitForStart = false;

    private int CircleIndex = 0;
    private int totalWaypointsPassed = 0;

    private float skill = 1f;
    private float pace = 1f;
    private float lateralOffset = 0f;
    private float mistakeTimer = 0f;

    private bool racing = true;

    // Progress is measured in waypoints, including partial progress
    // towards the next waypoint. The leaderboard uses this to rank cars.
    public float RaceProgress
    {
        get
        {
            if (Circle == null || Circle.Length < 2)
                return totalWaypointsPassed;

            int previousIndex =
                (CircleIndex - 1 + Circle.Length) % Circle.Length;

            Vector2 previousPoint = GetTargetPosition(previousIndex);
            Vector2 targetPoint = GetTargetPosition(CircleIndex);

            float segmentLength =
                Vector2.Distance(previousPoint, targetPoint);

            if (segmentLength < 0.0001f)
                return totalWaypointsPassed;

            float distanceTravelled =
                Vector2.Distance(transform.position, previousPoint);

            float segmentProgress =
                Mathf.Clamp01(distanceTravelled / segmentLength);

            return totalWaypointsPassed + segmentProgress;
        }
    }

    public void Setup(Transform[] path, int startIndex, float sideOffset)
    {
        Circle = path;

        if (Circle == null || Circle.Length < 2)
        {
            Debug.LogError("FollowThePath: Path needs at least 2 waypoints!", this);
            return;
        }

        CircleIndex = Mathf.Clamp(startIndex, 0, Circle.Length - 1);
        totalWaypointsPassed = 0;
        lateralOffset = sideOffset;
        racing = false;

        transform.position = GetTargetPosition(CircleIndex);
    }

    public void StartRace()
    {
        racing = true;
    }

    private void Start()
    {
        if (waitForStart)
            racing = false;

        if (Circle == null || Circle.Length < 2)
        {
            Debug.LogError(name + ": Assign at least 2 waypoints!", this);
            enabled = false;
            return;
        }

        CircleIndex = Mathf.Clamp(CircleIndex, 0, Circle.Length - 1);

        if (randomize)
        {
            skill = Random.Range(skillRange.x, skillRange.y);

            if (Mathf.Approximately(lateralOffset, 0f))
            {
                lateralOffset = Random.Range(
                    -maxLateralOffset, maxLateralOffset);
            }
        }

        transform.position = GetTargetPosition(CircleIndex);
    }

    private void Update()
    {
        if (!racing)
            return;

        Move();
        ChangeSpeed();
    }

    private Vector2 GetTargetPosition(int index)
    {
        Vector2 point = Circle[index].position;

        if (Mathf.Approximately(lateralOffset, 0f))
            return point;

        int nextIndex = (index + 1) % Circle.Length;

        Vector2 direction =
            ((Vector2)Circle[nextIndex].position - point).normalized;

        Vector2 side = new Vector2(-direction.y, direction.x);

        return point + side * lateralOffset;
    }

    private void Move()
    {
        Vector2 target = GetTargetPosition(CircleIndex);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        if (((Vector2)transform.position - target).sqrMagnitude < 0.0001f)
        {
            totalWaypointsPassed++;

            CircleIndex++;

            if (CircleIndex >= Circle.Length)
                CircleIndex = 0;

            if (randomize)
                OnWaypointReached();
        }
    }

    private void OnWaypointReached()
    {
        pace = 1f + Random.Range(-paceJitter, paceJitter);

        if (mistakeTimer <= 0f && Random.value < mistakeChance)
        {
            mistakeTimer = Random.Range(
                mistakeDurationRange.x,
                mistakeDurationRange.y
            );
        }
    }

    private void ChangeSpeed()
    {
        if (Circle == null || Circle.Length < 2)
            return;

        int currentIndex = CircleIndex;
        int nextIndex = (CircleIndex + 1) % Circle.Length;
        int futureIndex = (CircleIndex + lookAhead) % Circle.Length;
        int futurePreviousIndex =
            (CircleIndex + lookAhead - 1 + Circle.Length) % Circle.Length;

        Vector2 currentDirection =
            Circle[nextIndex].position - Circle[currentIndex].position;

        Vector2 futureDirection =
            Circle[futureIndex].position -
            Circle[futurePreviousIndex].position;

        float cornerAngle =
            Vector2.Angle(currentDirection, futureDirection);

        float targetSpeed =
            Mathf.Lerp(maxSpeed, minSpeed, cornerAngle / 90f);

        if (randomize)
        {
            float mistake = 1f;

            if (mistakeTimer > 0f)
            {
                mistakeTimer -= Time.deltaTime;
                mistake = mistakeSpeedMultiplier;
            }

            targetSpeed *= skill * pace * mistake;
        }

        targetSpeed = Mathf.Clamp(
            targetSpeed,
            minSpeed * 0.5f,
            maxSpeed
        );

        if (moveSpeed < targetSpeed)
        {
            moveSpeed = Mathf.MoveTowards(
                moveSpeed,
                targetSpeed,
                acceleration * Time.deltaTime
            );
        }
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
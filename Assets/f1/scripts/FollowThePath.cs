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

    // Umiejêtnoœæ bota: mno¿nik prêdkoœci przez ca³y wyœcig (1 = pe³na prêdkoœæ)
    [SerializeField] private Vector2 skillRange = new Vector2(0.92f, 1.0f);

    // Wahania tempa na ka¿dym waypoincie (np. 0.03 = +/- 3%)
    [SerializeField] private float paceJitter = 0.03f;

    // Szansa na b³¹d przy ka¿dym waypoincie (0.02 = 2%)
    [SerializeField] private float mistakeChance = 0.02f;
    [SerializeField] private float mistakeSpeedMultiplier = 0.6f;
    [SerializeField] private Vector2 mistakeDurationRange = new Vector2(1f, 2.5f);

    // Maksymalne przesuniêcie w bok od linii toru (¿eby boty siê nie nak³ada³y)
    [SerializeField] private float maxLateralOffset = 0.5f;

    private int CircleIndex = 0;

    private float skill = 1f;
    private float pace = 1f;
    private float lateralOffset = 0f;
    private float mistakeTimer = 0f;

    // Domyœlnie jedzie od razu (auto gracza); bot czeka na StartRace()
    private bool racing = true;

    // Wywo³uje BotSpawner zaraz po Instantiate (przed Start)
    public void Setup(Transform[] path, int startIndex, float sideOffset)
    {
        Circle = path;
        CircleIndex = startIndex;
        lateralOffset = sideOffset;
        racing = false;
    }

    // Wywo³aj dla wszystkich naraz, ¿eby ruszyli razem
    public void StartRace()
    {
        racing = true;
    }

    // Zaznacz u gracza, ¿eby czeka³ na wspólny start ze spawnera
    [SerializeField] private bool waitForStart = false;

    private void Start()
    {
        if (waitForStart)
        {
            racing = false;
        }

        if (randomize)
        {
            skill = Random.Range(skillRange.x, skillRange.y);

            // Jeœli spawner nie ustawi³ offsetu, losujemy go sami
            if (Mathf.Approximately(lateralOffset, 0f))
            {
                lateralOffset = Random.Range(-maxLateralOffset, maxLateralOffset);
            }
        }

        transform.position = GetTargetPosition(CircleIndex);
    }

    private void Update()
    {
        if (!racing)
        {
            return;
        }

        Move();
        ChangeSpeed();
    }

    // Pozycja waypointa + przesuniêcie w bok wzglêdem kierunku jazdy
    private Vector2 GetTargetPosition(int index)
    {
        Vector2 point = Circle[index].position;

        if (Mathf.Approximately(lateralOffset, 0f))
        {
            return point;
        }

        int nextIndex = (index + 1) % Circle.Length;
        Vector2 direction = ((Vector2)Circle[nextIndex].position - point).normalized;
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

        // Dotarliœmy do waypointa
        if (((Vector2)transform.position - target).sqrMagnitude < 0.0001f)
        {
            CircleIndex++;

            if (CircleIndex >= Circle.Length)
            {
                CircleIndex = 0;
                // TUTAJ póŸniej: koniec okr¹¿enia -> zapis czasu do tabeli
            }

            if (randomize)
            {
                OnWaypointReached();
            }
        }
    }

    private void OnWaypointReached()
    {
        // Lekkie losowe wahania tempa
        pace = 1f + Random.Range(-paceJitter, paceJitter);

        // Losowy b³¹d kierowcy (chwilowe zwolnienie)
        if (mistakeTimer <= 0f && Random.value < mistakeChance)
        {
            mistakeTimer = Random.Range(mistakeDurationRange.x, mistakeDurationRange.y);
        }
    }

    private void ChangeSpeed()
    {
        if (Circle.Length < 2)
        {
            return;
        }

        int currentIndex = CircleIndex;
        int nextIndex = (CircleIndex + 1) % Circle.Length;
        int futureIndex = (CircleIndex + lookAhead) % Circle.Length;
        int futurePreviousIndex = (CircleIndex + lookAhead - 1) % Circle.Length;

        Vector2 currentDirection =
            Circle[nextIndex].position - Circle[currentIndex].position;

        Vector2 futureDirection =
            Circle[futureIndex].position - Circle[futurePreviousIndex].position;

        float cornerAngle = Vector2.Angle(currentDirection, futureDirection);

        float targetSpeed = Mathf.Lerp(maxSpeed, minSpeed, cornerAngle / 90f);

        // Losowoœæ: umiejêtnoœæ * wahania tempa * ewentualny b³¹d
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

        targetSpeed = Mathf.Clamp(targetSpeed, minSpeed * 0.5f, maxSpeed);

        if (moveSpeed < targetSpeed)
        {
            moveSpeed = Mathf.MoveTowards(moveSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else if (moveSpeed > targetSpeed)
        {
            moveSpeed = Mathf.MoveTowards(moveSpeed, targetSpeed, braking * Time.deltaTime);
        }
    }
}
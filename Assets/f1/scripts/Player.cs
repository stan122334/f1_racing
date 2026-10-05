using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Current Lap Timer")]
    [SerializeField] private TextMeshProUGUI TimerText;

    [Header("Last 5 Lap Times")]
    [SerializeField] private TextMeshProUGUI Lap1Text;
    [SerializeField] private TextMeshProUGUI Lap2Text;
    [SerializeField] private TextMeshProUGUI Lap3Text;
    [SerializeField] private TextMeshProUGUI Lap4Text;
    [SerializeField] private TextMeshProUGUI Lap5Text;

    [Header("Timer Settings")]
    [SerializeField] private float timerSpeed = 2f;

    private float timer = 0f;
    private int lapNumber = 1;

    private bool canFinishLap = true;

    private void Start()
    {
        timer = 0f;
        lapNumber = 1;

        TimerText.text = "00:00.000";

        Lap1Text.text = "";
        Lap2Text.text = "";
        Lap3Text.text = "";
        Lap4Text.text = "";
        Lap5Text.text = "";
    }

    private void Update()
    {
        // Timer runs at 2x speed
        timer += Time.deltaTime * timerSpeed;

        TimerText.text = FormatTime(timer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER DETECTED: " + other.gameObject.name);

        if (other.CompareTag("Finish") && canFinishLap)
        {
            FinishLap();

            canFinishLap = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            canFinishLap = true;
        }
    }

    private void FinishLap()
    {
        Debug.Log("LAP " + lapNumber + " FINISHED!");

        // Save the completed lap time
        string lapTime = FormatTime(timer);

        // Move old lap times down
        Lap5Text.text = Lap4Text.text;
        Lap4Text.text = Lap3Text.text;
        Lap3Text.text = Lap2Text.text;
        Lap2Text.text = Lap1Text.text;

        // Save newest lap
        Lap1Text.text = "Lap " + lapNumber + ": " + lapTime;

        // Next lap
        lapNumber++;

        // RESET TIMER
        timer = 0f;

        TimerText.text = "00:00.000";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return minutes.ToString("00") + ":" +
               seconds.ToString("00") + "." +
               milliseconds.ToString("000");
    }
}
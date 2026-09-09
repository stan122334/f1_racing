
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

    private float timer = 0f;

    private int lapNumber = 1;

    private void Update()
    {
        // Keep the current lap timer running
        timer += Time.deltaTime * 2f;

        // Display the current lap time
        TimerText.text = FormatTime(timer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we have crossed the finish line
        if (other.CompareTag("Finish"))
        {
            CompleteLap();
        }
    }

    private void CompleteLap()
    {
        // Get the completed lap time
        string completedLapTime = FormatTime(timer);

        // Move all previous lap times down one position
        Lap5Text.text = Lap4Text.text;
        Lap4Text.text = Lap3Text.text;
        Lap3Text.text = Lap2Text.text;
        Lap2Text.text = Lap1Text.text;

        // Put the newest lap at the top
        Lap1Text.text = "Lap " + lapNumber + ": " + completedLapTime;

        // Increase the lap number
        lapNumber++;

        // Reset the current lap timer
        timer = 0f;

        // Immediately show 00:00.000
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


 
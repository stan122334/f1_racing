using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI TimerText;

    private float timer = 0f;

    private void Update()
    {
        // Add time every frame
        timer += Time.deltaTime;

        // Convert the time into minutes, seconds and milliseconds
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        int milliseconds = Mathf.FloorToInt((timer * 1000f) % 1000f);

        // Display the timer
        TimerText.text =
            minutes.ToString("00") + ":" +
            seconds.ToString("00") + "." +
            milliseconds.ToString("000");
    }
}

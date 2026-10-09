
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    [Header("Bot Settings")]
    [SerializeField] private FollowThePath botPrefab;
    [SerializeField] private Transform[] circle;
    [SerializeField] private int botCount = 5;

    [Header("Starting Grid")]
    [SerializeField] private int startIndex = 0;
    [SerializeField] private float gridWidth = 3f;
    [SerializeField] private bool reservePlayerSlot = true;

    [Header("Race Start")]
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private FollowThePath player;

    [Header("Leaderboard")]
    [SerializeField] private RaceLeaderboard leaderboard;

    private readonly List<FollowThePath> bots =
        new List<FollowThePath>();

    private void Start()
    {
        if (botPrefab == null)
        {
            Debug.LogError("BotSpawner: Assign the Bot Prefab!", this);
            return;
        }

        if (circle == null || circle.Length < 2)
        {
            Debug.LogError("BotSpawner: Assign at least 2 waypoints!", this);
            return;
        }

        if (leaderboard == null)
        {
            Debug.LogError("BotSpawner: Assign the RaceLeaderboard!", this);
            return;
        }

        // Register the player using the first existing name label.
        if (player != null)
        {
            string playerName = leaderboard.GetDriverName(0).Trim();

            if (string.IsNullOrEmpty(playerName))
                playerName = "Player";

            player.gameObject.name = playerName;
            leaderboard.RegisterDriver(playerName, player);
        }

        int slots = botCount + (reservePlayerSlot ? 1 : 0);
        int playerSlot = slots / 2;
        int slot = 0;

        for (int i = 0; i < botCount; i++)
        {
            if (reservePlayerSlot && slot == playerSlot)
                slot++;

            float t = slots > 1
                ? slot / (float)(slots - 1)
                : 0.5f;

            float side = Mathf.Lerp(
                -gridWidth / 2f,
                gridWidth / 2f,
                t
            );

            slot++;

            FollowThePath bot = Instantiate(botPrefab);

            // Read the name from your existing UI.
            string driverName = leaderboard.GetDriverName(i + 1).Trim();

            if (string.IsNullOrEmpty(driverName))
                driverName = "Bot_" + (i + 1);

            // Give the spawned bot its original name.
            bot.gameObject.name = driverName;

            // Set up its starting position and route.
            bot.Setup(circle, startIndex, side);

            bots.Add(bot);

            // Store the same name in the leaderboard driver entry.
            leaderboard.RegisterDriver(driverName, bot);
        }

        Invoke(nameof(StartRace), startDelay);
    }

    private void StartRace()
    {
        foreach (FollowThePath bot in bots)
        {
            if (bot != null)
                bot.StartRace();
        }

        if (player != null)
            player.StartRace();
    }
}
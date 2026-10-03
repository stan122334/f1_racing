using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    [SerializeField] private FollowThePath botPrefab;

    // Te same waypointy, których u¿ywa gracz
    [SerializeField] private Transform[] circle;

    [SerializeField] private int botCount = 5;

    // Waypoint, na którym stoi ca³a stawka (0 = linia startu)
    [SerializeField] private int startIndex = 0;

    // Ca³kowita szerokoœæ startu w poprzek toru
    [SerializeField] private float gridWidth = 3f;

    // Zostaw wolne miejsce na œrodku dla auta gracza
    [SerializeField] private bool reservePlayerSlot = true;

    // Czas do startu (np. odliczanie 3 sekundy)
    [SerializeField] private float startDelay = 3f;

    // Gracz te¿ mo¿e czekaæ na ten sam start (podepnij jego FollowThePath)
    [SerializeField] private FollowThePath player;

    private readonly List<FollowThePath> bots = new List<FollowThePath>();

    private void Start()
    {
        int slots = botCount + (reservePlayerSlot ? 1 : 0);
        int playerSlot = slots / 2;
        int slot = 0;

        for (int i = 0; i < botCount; i++)
        {
            if (reservePlayerSlot && slot == playerSlot)
            {
                slot++;
            }

            // Równo roz³o¿one miejsca od lewej do prawej krawêdzi
            float t = slots > 1 ? slot / (float)(slots - 1) : 0.5f;
            float side = Mathf.Lerp(-gridWidth / 2f, gridWidth / 2f, t);
            slot++;

            FollowThePath bot = Instantiate(botPrefab);
            bot.name = "Bot_" + (i + 1);
            bot.Setup(circle, startIndex, side);
            bots.Add(bot);
        }

        Invoke(nameof(StartRace), startDelay);
    }

    private void StartRace()
    {
        foreach (FollowThePath bot in bots)
        {
            bot.StartRace();
        }

        if (player != null)
        {
            player.StartRace();
        }
    }
}


using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceLeaderboard : MonoBehaviour
{
    [System.Serializable]
    public class Driver
    {
        public string driverName;
        public FollowThePath car;
    }

    [Header("Race Drivers")]
    public List<Driver> drivers = new List<Driver>();

    [Header("Existing Position Texts")]
    [SerializeField] private TMP_Text[] positionTexts;

    [Header("Existing Driver Name Texts")]
    [SerializeField] private TMP_Text[] nameTexts;

    [Header("Update Settings")]
    [SerializeField] private float updateInterval = 0.2f;

    private float updateTimer;

    // Gets the driver's name from your existing UI.
    public string GetDriverName(int index)
    {
        if (nameTexts == null ||
            index < 0 ||
            index >= nameTexts.Length ||
            nameTexts[index] == null)
        {
            return "Driver " + (index + 1);
        }

        return nameTexts[index].text;
    }

    // Registers a car without changing your existing UI names.
    public void RegisterDriver(string driverName, FollowThePath car)
    {
        if (car == null)
            return;

        foreach (Driver driver in drivers)
        {
            if (driver != null && driver.car == car)
                return;
        }

        drivers.Add(new Driver
        {
            driverName = driverName,
            car = car
        });
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateInterval <= 0f || updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateLeaderboard();
        }
    }


    private void UpdateLeaderboard()
    {
        if (positionTexts == null || nameTexts == null)
            return;

        // Remove cars that no longer exist.
        drivers.RemoveAll(d => d == null || d.car == null);

        // Sort cars from furthest to least progress.
        drivers.Sort((a, b) =>
            b.car.RaceProgress.CompareTo(a.car.RaceProgress));

        // Update positions AND names in the visible leaderboard.
        for (int i = 0; i < positionTexts.Length; i++)
        {
            if (i < drivers.Count)
            {
                if (positionTexts[i] != null)
                    positionTexts[i].text = (i + 1) + ".";

                if (i < nameTexts.Length && nameTexts[i] != null)
                    nameTexts[i].text = drivers[i].driverName;
            }
            else
            {
                if (positionTexts[i] != null)
                    positionTexts[i].text = "";

                if (i < nameTexts.Length && nameTexts[i] != null)
                    nameTexts[i].text = "";
            }
        }
    }
}
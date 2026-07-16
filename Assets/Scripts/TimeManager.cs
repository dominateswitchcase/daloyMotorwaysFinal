using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("UI")]
    [Tooltip("Drag your on-screen UI Text here")]
    public TextMeshProUGUI clockText;

    [Header("Time")]
    public float dayLength = 180f;
    public float startHour = 5f;
    public float endHour = 22f;

    [Header("Inventory & Progression")]
    public int availableJeepneys = 2;
    public int currentDay = 1;

    public float CurrentHour { get; private set; }

    private float timer;
    private List<DistrictBuilding> allBuildings = new List<DistrictBuilding>();
    private float demandTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterBuilding(DistrictBuilding building)
    {
        if (!allBuildings.Contains(building))
        {
            allBuildings.Add(building);
        }
    }

    void Update()
    {
        HandleTimeProgression();
        HandleBuildingDemands();
        UpdateUI();
    }

    void HandleTimeProgression()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / dayLength);
        CurrentHour = Mathf.Lerp(startHour, endHour, t);

        if (CurrentHour >= endHour)
        {
            EndDay();
        }
    }

    void HandleBuildingDemands()
    {
        demandTimer += Time.deltaTime;

        // Smooth difficulty curve: demands get 3 seconds faster every day, stopping at a minimum of 8 seconds.
        float timeBetweenDemands = Mathf.Max(8f, 25f - (currentDay * 3f));

        if (demandTimer >= timeBetweenDemands)
        {
            demandTimer = 0f;
            ActivateRandomBuilding();
        }
    }

    void ActivateRandomBuilding()
    {
        List<DistrictBuilding> chillBuildings = new List<DistrictBuilding>();

        foreach (DistrictBuilding building in allBuildings)
        {
            if (!building.isDemandingShed)
            {
                chillBuildings.Add(building);
            }
        }

        if (chillBuildings.Count > 0)
        {
            // Calculate how many buildings get angry at the same time based on the day
            int buildingsToActivate = Mathf.CeilToInt(currentDay / 2f);

            // Prevent an error if there aren't enough chill buildings left
            buildingsToActivate = Mathf.Min(buildingsToActivate, chillBuildings.Count);

            for (int i = 0; i < buildingsToActivate; i++)
            {
                int randomIndex = Random.Range(0, chillBuildings.Count);
                chillBuildings[randomIndex].ActivateDemand();
                chillBuildings.RemoveAt(randomIndex); // Remove so we don't pick it twice
            }
        }
    }

    void EndDay()
    {
        Debug.Log("Day " + currentDay + " Ended. Rewarding 1 Jeepney!");
        availableJeepneys++;
        currentDay++;
        timer = 0f;
    }

    void UpdateUI()
    {
        if (clockText != null)
        {
            int hour = Mathf.FloorToInt(CurrentHour);
            int minutes = Mathf.FloorToInt((CurrentHour - hour) * 60f);
            clockText.text = $"Day {currentDay} | {hour:00}:{minutes:00}";
        }
    }

    public float NormalizedTime()
    {
        return Mathf.InverseLerp(startHour, endHour, CurrentHour);
    }

    public bool TryUseJeepney()
    {
        if (availableJeepneys > 0)
        {
            availableJeepneys--;
            return true;
        }
        return false;
    }
}
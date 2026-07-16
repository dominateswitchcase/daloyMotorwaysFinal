using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Day / Night UI")]
    public List<Image> NightIcons = new List<Image>();
    public UIAutoAnimation TextToBeShown;

    public float CurrentHour { get; private set; }

    private float timer;
    private float demandTimer;

    private bool fiveAMTriggered = false;

    private List<DistrictBuilding> allBuildings = new List<DistrictBuilding>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        HandleTimeProgression();
        HandleBuildingDemands();
        UpdateUI();
    }

    #region Time

    void HandleTimeProgression()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / dayLength);

        CurrentHour = Mathf.Lerp(startHour, endHour, t);

        ChangeTimeIcon();

        if (!fiveAMTriggered && CurrentHour >= 5f)
        {
            fiveAMTriggered = true;
            StartCoroutine(PlayDayPopUpTransition());
        }


        if (CurrentHour >= endHour)
        {
            EndDay();
        }
    }

    public float HourToNormalized(float hour)
    {
        return Mathf.InverseLerp(startHour, endHour, hour);
    }

    public float NormalizedTime()
    {
        return Mathf.InverseLerp(startHour, endHour, CurrentHour);
    }

    public string GetFormattedTime()
    {
        int hour = Mathf.FloorToInt(CurrentHour);
        int minutes = Mathf.FloorToInt((CurrentHour - hour) * 60f);

        return $"{hour:00}:{minutes:00}";
    }

    #endregion

    #region Buildings

    public void RegisterBuilding(DistrictBuilding building)
    {
        if (!allBuildings.Contains(building))
            allBuildings.Add(building);
    }

    void HandleBuildingDemands()
    {
        demandTimer += Time.deltaTime;

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
                chillBuildings.Add(building);
        }

        if (chillBuildings.Count == 0)
            return;

        int buildingsToActivate = Mathf.CeilToInt(currentDay / 2f);
        buildingsToActivate = Mathf.Min(buildingsToActivate, chillBuildings.Count);

        for (int i = 0; i < buildingsToActivate; i++)
        {
            int randomIndex = Random.Range(0, chillBuildings.Count);

            chillBuildings[randomIndex].ActivateDemand();

            chillBuildings.RemoveAt(randomIndex);
        }
    }

    #endregion

    #region Day Night UI

    IEnumerator PlayDayPopUpTransition()
    {
        if (TextToBeShown != null)
        {
            TextToBeShown.EntranceAnimation();

            yield return new WaitForSeconds(1.6f);

            TextToBeShown.ExitAnimation();
        }
    }

    void ChangeTimeIcon()
    {
        int activeIndex = GetActiveIconIndex();

        for (int i = 0; i < NightIcons.Count; i++)
        {
            if (NightIcons[i] != null)
                NightIcons[i].enabled = (i == activeIndex);
        }
    }

    int GetActiveIconIndex()
    {
        if (CurrentHour >= 5f && CurrentHour < 7f)
            return 0;   // Sunrise

        if (CurrentHour >= 7f && CurrentHour < 17f)
            return 1;   // Day

        if (CurrentHour >= 17f && CurrentHour < 19f)
            return 2;   // Sunset

        return 3;       // Night
    }

    #endregion

    #region UI

    void UpdateUI()
    {
        if (clockText == null)
            return;

        int hour = Mathf.FloorToInt(CurrentHour);
        int minutes = Mathf.FloorToInt((CurrentHour - hour) * 60f);

        clockText.text = $"Day {currentDay} | {hour:00}:{minutes:00}";
    }

    #endregion

    #region Gameplay

    void EndDay()
    {
        Debug.Log($"Day {currentDay} Ended. Rewarding 1 Jeepney!");

        availableJeepneys++;
        currentDay++;

        timer = 0f;
        demandTimer = 0f;

        fiveAMTriggered = false;
    }

    public bool TryUseJeepney()
    {
        if (availableJeepneys <= 0)
            return false;

        availableJeepneys--;
        return true;
    }

    #endregion
}
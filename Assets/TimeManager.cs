using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Time")]
    public float dayLength = 720f;      // 12 real minutes
    public float startHour = 5f;
    public float endHour = 22f;

    public float CurrentHour { get; private set; }

    public float HourToNormalized(float hour)
    {
        return Mathf.InverseLerp(startHour, endHour, hour);
    }

    private float timer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / dayLength);

        CurrentHour = Mathf.Lerp(startHour, endHour, t);

        if (CurrentHour >= endHour)
        {
            EndDay();
        }
    }

    void EndDay()
    {
        Debug.Log("Day Ended");
    }

    public float NormalizedTime()
    {
        return Mathf.InverseLerp(startHour, endHour, CurrentHour);
    }
}
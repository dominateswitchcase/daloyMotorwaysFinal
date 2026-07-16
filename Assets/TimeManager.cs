using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    // For UI ***
    public List<Image> NightIcons = new List<Image>();
    public UIAutoAnimation TextToBeShown; 
    private bool fiveAMTriggered = false;

    // ****

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

    public System.Collections.IEnumerator PlayDayPopUpTransition(){
        Debug.Log("TRIYING TO PLYA THE ANIMATION");
        
        if (TextToBeShown != null){
            TextToBeShown.EntranceAnimation();
            yield return new WaitForSeconds(1.6f);
            TextToBeShown.ExitAnimation();
        }
        
    }

    public void ChangeTimeIcon()
    {
         Debug.Log($"Hour: {CurrentHour}");
        int activeIndex = GetActiveIconIndex();
         Debug.Log($"Active Index: {activeIndex}");

        for (int i = 0; i < NightIcons.Count; i++)
        {
            if (NightIcons[i] != null)
                NightIcons[i].enabled = (i == activeIndex);
        }
    }

    private int GetActiveIconIndex()
    {
        if (CurrentHour >= 5f && CurrentHour < 7f)
            return 0; // Sunrise
        else if (CurrentHour >= 7f && CurrentHour < 17f)
            return 1; // Morning/Day
        else if (CurrentHour >= 17f && CurrentHour < 19f)
            return 0; // Sunset
        else
            return 3; // Night
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
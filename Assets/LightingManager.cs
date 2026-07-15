using UnityEngine;

public class LightingManager : MonoBehaviour
{

    [Header("References")]
    public Light sun;

    [Header("Lighting")]
    public Gradient lightColor;
    public AnimationCurve lightIntensity;

    [Header("Sun angle")]
    public float sunriseAngle = 15f;
    public float sunsetAngle = 195f;

    void Update()
    {
        float t = TimeManager.Instance.NormalizedTime();

        sun.color = lightColor.Evaluate(t);
        sun.intensity = lightIntensity.Evaluate(t);


        float angle = Mathf.Lerp(sunriseAngle, sunsetAngle, t);
        sun.transform.rotation = Quaternion.Euler(angle, -30f, 0f);
    }
}
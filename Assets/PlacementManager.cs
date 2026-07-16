using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.InputSystem;

public class PlacementManager : MonoBehaviour
{
    [Header("References")]
    public Camera cam;

    public GameObject waitingShedPrefab;

    public Transform preview;

    [Header("Settings")]
    public LayerMask roadMask;

    public int splineSamples = 30;

    private RoadSegment currentRoad;

    private float currentT;

    private void Update()
    {
        UpdatePreview();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            PlaceWaitingShed();
    }

    void UpdatePreview()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, roadMask))
            return;

        RoadSegment road = hit.collider.GetComponent<RoadSegment>();

        if (road == null)
            return;

        currentRoad = road;

        SplineContainer spline = road.spline;

        float bestDistance = float.MaxValue;
        float bestT = 0;

        for (int i = 0; i <= splineSamples; i++)
        {
            float t = i / (float)splineSamples;

            Vector3 p =
                spline.transform.TransformPoint(
                    spline.EvaluatePosition(t));

            float d = Vector3.Distance(hit.point, p);

            if (d < bestDistance)
            {
                bestDistance = d;
                bestT = t;
            }
        }

        currentT = bestT;

        preview.position =
            spline.transform.TransformPoint(
                spline.EvaluatePosition(bestT));
    }

    void PlaceWaitingShed()
    {
        if (currentRoad == null)
            return;

        GameObject obj =
            Instantiate(waitingShedPrefab,
                        preview.position,
                        Quaternion.identity);

        WaitingShed shed = obj.GetComponent<WaitingShed>();

        shed.road = currentRoad;
        shed.t = currentT;

        Debug.Log($"Placed shed on {currentRoad.name} @ {currentT:F2}");
    }
}
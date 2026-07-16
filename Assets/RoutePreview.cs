using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(LineRenderer))]
public class RoutePreview : MonoBehaviour
{
    private LineRenderer line;

    [Header("Rendering")]
    [SerializeField] private int samplesPerRoad = 20;
    [SerializeField] private float heightOffset = 0.1f;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 0;
    }

    public void DrawSingleRoad(RoadSegment road)
    {
        DrawPath(new List<RoadSegment>() { road });
    }

    /// <summary>
    /// Draw the route by following the road splines.
    /// </summary>
    public void DrawPath(List<RoadSegment> path)
    {
        Debug.Log("========== DRAW PATH ==========");

        if (path == null)
        {
            Debug.Log("Path is NULL");
            Clear();
            return;
        }

        Debug.Log("Road Count: " + path.Count);

        if (path.Count == 0)
        {
            Debug.Log("Path is EMPTY");
            Clear();
            return;
        }

        List<Vector3> points = new();

        foreach (RoadSegment road in path)
        {
            if (road == null)
            {
                Debug.Log("Road is NULL");
                continue;
            }

            Debug.Log("Drawing Road: " + road.name);

            if (road.spline == null)
            {
                Debug.LogError(road.name + " has NO spline!");
                continue;
            }

            for (int i = 0; i <= samplesPerRoad; i++)
            {
                float t = i / (float)samplesPerRoad;

                Vector3 pos = road.spline.EvaluatePosition(t);

                pos.y += heightOffset;

                if (points.Count > 0 &&
                    Vector3.Distance(points[points.Count - 1], pos) < 0.05f)
                    continue;

                points.Add(pos);
            }
        }

        Debug.Log("Generated Points: " + points.Count);

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());

        Debug.Log("LineRenderer Position Count: " + line.positionCount);
    }

    public void Clear()
    {
        line.positionCount = 0;
    }
}
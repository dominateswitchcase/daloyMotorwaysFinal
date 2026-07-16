using System.Collections.Generic;
using UnityEngine;

public class RouteBuilder : MonoBehaviour
{

    public static RouteBuilder Instance;

    public List<WaitingShed> currentStops = new();

    // We'll fill this after A* works
    public List<RoadSegment> currentPath = new();

    public RoutePreview preview;

    public bool isDrawing = false;

    private void Awake()
    {
        Instance = this;
    }

    public void BeginRoute()
    {
        currentStops.Clear();
        currentPath.Clear();

        preview.Clear();

        isDrawing = true;
    }

    public void AddStop(WaitingShed stop)
    {
        Debug.Log("Clicked: " + stop.name);

        if (!isDrawing)
        {
            Debug.Log("Not drawing!");
            return;
        }

        if (currentStops.Contains(stop))
        {
            Debug.Log("Stop already added.");
            return;
        }

        currentStops.Add(stop);

        Debug.Log("Current Stops: " + currentStops.Count);

        if (currentStops.Count >= 2)
        {
            WaitingShed previous = currentStops[currentStops.Count - 2];

            Debug.Log("Finding path from "
                + previous.roadNode.name
                + " to "
                + stop.roadNode.name);

            List<RoadSegment> section =
                Pathfinder.FindPath(previous.roadNode, stop.roadNode);

            Debug.Log("Roads Found: " + section.Count);

            foreach (RoadSegment road in section)
            {
                Debug.Log("Road: " + road.name);
            }

            currentPath.AddRange(section);

            Debug.Log("Current Path Count: " + currentPath.Count);

            preview.DrawPath(currentPath);
        }

        Debug.Log("Added Stop: " + stop.name);
    }

    public void FinishRoute()
    {
        isDrawing = false;

        Debug.Log("Route Complete!");

        foreach (WaitingShed stop in currentStops)
        {
            Debug.Log(stop.name);
        }

        // Later:
        // Create JeepRoute
        // Run Pathfinder
        // Spawn Jeep
    }

    public void CancelRoute()
    {
        currentStops.Clear();
        currentPath.Clear();

        preview.Clear();

        isDrawing = false;
    }
}
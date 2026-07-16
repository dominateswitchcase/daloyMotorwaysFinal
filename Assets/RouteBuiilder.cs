using System.Collections.Generic;
using UnityEngine;

public class RouteBuilder : MonoBehaviour
{
    public static RouteBuilder Instance;

    // Selected nodes
    public List<RoadNode> currentStops = new();

    // Final road path
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

        Debug.Log("Started Route");
    }

    public void AddStop(RoadNode stop)
    {
        Debug.Log("Clicked: " + stop.name);

        if (!isDrawing)
        {
            Debug.Log("Not drawing!");
            return;
        }

        if (currentStops.Contains(stop))
        {
            Debug.Log("Node already added.");
            return;
        }

        currentStops.Add(stop);

        Debug.Log("Current Nodes: " + currentStops.Count);

        // Once two nodes exist, find the road path between them
        if (currentStops.Count >= 2)
        {
            RoadNode previous = currentStops[currentStops.Count - 2];

            Debug.Log($"Finding path from {previous.name} to {stop.name}");

            List<RoadSegment> section =
                Pathfinder.FindPath(previous, stop);

            Debug.Log("Roads Found: " + section.Count);

            foreach (RoadSegment road in section)
            {
                Debug.Log("Road: " + road.name);
            }

            currentPath.AddRange(section);

            Debug.Log("Current Path Count: " + currentPath.Count);

            preview.DrawPath(currentPath);
        }

        Debug.Log("Added Node: " + stop.name);
    }

    public void FinishRoute()
    {
        isDrawing = false;

        Debug.Log("Route Complete!");

        foreach (RoadNode node in currentStops)
        {
            Debug.Log(node.name);
        }

        // Later:
        // Save Route
        // Spawn Jeep
    }

    public void CancelRoute()
    {
        currentStops.Clear();
        currentPath.Clear();

        preview.Clear();

        isDrawing = false;

        Debug.Log("Route Cancelled");
    }
}
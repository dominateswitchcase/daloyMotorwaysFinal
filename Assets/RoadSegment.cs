using UnityEngine;
using UnityEngine.Splines;

public class RoadSegment : MonoBehaviour
{
    [Header("Road Graph")]
    public RoadNode startNode;
    public RoadNode endNode;

    [Header("Road")]
    public SplineContainer spline;

    [Header("Traffic")]
    public float speedLimit = 30f;

    private void Awake()
    {
        // Automatically get the spline if it's on the same GameObject
        if (spline == null)
            spline = GetComponent<SplineContainer>();
    }

    private void OnValidate()
    {
        // Keep RoadNodes updated in the editor
        if (startNode != null && !startNode.connectedRoads.Contains(this))
            startNode.connectedRoads.Add(this);

        if (endNode != null && !endNode.connectedRoads.Contains(this))
            endNode.connectedRoads.Add(this);
    }

    public RoadNode GetOtherNode(RoadNode current)
    {
        if (current == startNode)
            return endNode;

        if (current == endNode)
            return startNode;

        return null;
    }
}
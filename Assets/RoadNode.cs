using System.Collections.Generic;
using UnityEngine;

public class RoadNode : MonoBehaviour
{
    public List<RoadSegment> connectedRoads = new();

    private void Start()
    {
        Debug.Log(name + " Connected Roads: " + connectedRoads.Count);

        foreach (RoadSegment road in connectedRoads)
        {
            if (road != null)
                Debug.Log(" -> " + road.name);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the node
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.6f);

        // Draw connections
        Gizmos.color = Color.cyan;

        foreach (RoadSegment road in connectedRoads)
        {
            if (road == null)
                continue;

            RoadNode other = road.GetOtherNode(this);

            if (other == null)
                continue;

            Gizmos.DrawLine(transform.position, other.transform.position);
        }
    }
}
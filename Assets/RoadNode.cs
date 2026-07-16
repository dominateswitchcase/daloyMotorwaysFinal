using System.Collections.Generic;
using UnityEngine;

public class RoadNode : MonoBehaviour
{
    public List<RoadSegment> connectedRoads = new();

    [Header("Visual")]
    public GameObject visual;

    public void SetVisible(bool visible)
    {
        if (visual != null)
            visual.SetActive(visible);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.6f);

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
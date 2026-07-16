using UnityEngine;

public class RoadNodeClick : MonoBehaviour
{
    private RoadNode node;

    private void Awake()
    {
        node = GetComponent<RoadNode>();
    }

    private void OnMouseDown()
    {
        if (RouteBuilder.Instance != null)
        {
            RouteBuilder.Instance.AddStop(node);
        }
    }
}
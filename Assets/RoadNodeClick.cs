using UnityEngine;

public class RoadNodeClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Clicked Node: " + gameObject.name);

        if (RouteBuilder.Instance != null)
        {
            RouteBuilder.Instance.AddStop(GetComponent<RoadNode>());
        }
        else
        {
            Debug.LogError("RouteBuilder.Instance is NULL");
        }
    }
}
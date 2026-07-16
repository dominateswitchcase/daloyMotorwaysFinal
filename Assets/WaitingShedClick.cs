using UnityEngine;

public class WaitingShedClick : MonoBehaviour
{
    private WaitingShed shed;

    private void Awake()
    {
        shed = GetComponent<WaitingShed>();
    }

    private void OnMouseDown()
    {
        RouteBuilder.Instance.AddStop(shed);
    }
}
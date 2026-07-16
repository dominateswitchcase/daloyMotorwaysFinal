using UnityEngine;

public class ShedPlacementManager : MonoBehaviour
{
    [Header("Placement Settings")]
    [Tooltip("Drag your Waiting Shed Prefab here.")]
    public GameObject shedPrefab;

    [Tooltip("Set this to 'Ground' so sheds only place on the baseplate.")]
    public LayerMask groundLayer;

    void Update()
    {
        // Check if the player presses the Left Mouse Button
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceShed();
        }
    }

    void TryPlaceShed()
    {
        // Create a ray from the mouse cursor position on the screen into the 3D world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Shoot the ray. It will only register a hit if it touches the 'groundLayer'
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // Spawn the shed exactly where the ray hit the ground
            Instantiate(shedPrefab, hit.point, Quaternion.identity);

            Debug.Log("Player placed a Waiting Shed at: " + hit.point);
        }
    }
}
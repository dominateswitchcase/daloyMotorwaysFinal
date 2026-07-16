using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDebugger : MonoBehaviour
{
    void Update()
    {
        // Check if the left mouse button was clicked
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            // Shoot a raycast that hits absolutely EVERYTHING (no layer restrictions)
            if (Physics.Raycast(ray, out hit))
            {
                // Print the exact name and layer of whatever we just touched
                string objectName = hit.collider.gameObject.name;
                string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);

                Debug.Log($"DEBUG LASER HIT: [{objectName}] on Layer: [{layerName}]");
            }
            else
            {
                Debug.Log("DEBUG LASER HIT: Absolutely nothing. The laser shot into the void.");
            }
        }
    }
}
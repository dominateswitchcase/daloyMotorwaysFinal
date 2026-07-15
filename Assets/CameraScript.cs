using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetMover : MonoBehaviour
{
    [Header("Movement Speed")]
    public float moveSpeed = 20f;

    [Header("Rotation Speed")]
    [Tooltip("How fast the camera rotates when pressing Q or E")]
    public float rotationSpeed = 100f;

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 inputVector = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) inputVector.y = 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) inputVector.y = -1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) inputVector.x = -1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) inputVector.x = 1;
        }

        if (inputVector != Vector2.zero)
        {
            // Calculate movement directions relative to the target's current rotation
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // Keep movement completely flat on the ground plane
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = (right * inputVector.x) + (forward * inputVector.y);
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        float rotationInput = 0f;

        if (Keyboard.current != null)
        {
            // Q rotates Left, E rotates Right
            if (Keyboard.current.qKey.isPressed) rotationInput = -1f;
            if (Keyboard.current.eKey.isPressed) rotationInput = 1f;
        }

        if (rotationInput != 0f)
        {
            // Rotate the target object around the Y axis
            transform.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Required to change camera priorities
using System.Collections; // Required for Coroutines fading timers

public class CameraTargetMover : MonoBehaviour
{
    [Header("Movement Speed")]
    public float moveSpeed = 20f;

    [Header("Rotation Speed")]
    [Tooltip("How fast the camera rotates when pressing Q or E")]
    public float rotationSpeed = 100f;

    [Header("Cinemachine Setup")]
    public CinemachineCamera freeformCamera;
    public CinemachineCamera topDownCamera;

    [Header("Top Down Zoom Control")]
    public float topDownZoomSpeed = 15f;
    public float minHeight = 10f;
    public float maxHeight = 60f;

    [Header("Fade UI Settings")]
    [Tooltip("Drag the CanvasGroup component from your full-screen black panel here")]
    public CanvasGroup fadeCanvasGroup;
    [Tooltip("How fast the screen fades to black and back to gameplay")]
    public float fadeSpeed = 4f;

    private bool isTopDownMode = false;
    private bool isTransitioning = false; // Locks inputs entirely during the visual fade sequence

    void Start()
    {
        if (freeformCamera != null) freeformCamera.Priority = 10;
        if (topDownCamera != null) topDownCamera.Priority = 0;

        // Ensure the black fade overlay screen is fully visible at project launch
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. Process spacebar input to trigger fade sequence
        HandleToggleInput();

        // 2. Freeze ALL player controls completely if the screen is currently fading
        if (isTransitioning) return;

        // 3. Normal gameplay loops running only when the camera frame settles
        if (isTopDownMode)
        {
            HandleTopDownZoom();
            HandleMovement();
        }
        else
        {
            HandleRotation();
            HandleMovement();
        }
    }

    void HandleToggleInput()
    {
        // Block consecutive spacebar inputs if the coroutine sequence is running
        if (isTransitioning) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(ExecuteCameraFadeTransition());
        }
    }

    // Smooth Coroutine managing the visual fade & snap sequence
    IEnumerator ExecuteCameraFadeTransition()
    {
        isTransitioning = true;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);

            // --- FADE TO BLACK ---
            while (fadeCanvasGroup.alpha < 1f)
            {
                fadeCanvasGroup.alpha += fadeSpeed * Time.deltaTime;
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        // --- INSTANT BEHIND-THE-SCENES CAMERA SNAP ---
        isTopDownMode = !isTopDownMode;

        if (freeformCamera != null && topDownCamera != null)
        {
            freeformCamera.Priority = isTopDownMode ? 0 : 10;
            topDownCamera.Priority = isTopDownMode ? 10 : 0;
        }

        // Brief artificial pause to let Cinemachine finish updating its render frame
        yield return new WaitForSeconds(0.05f);

        // --- FADE BACK IN TO GAMEPLAY ---
        if (fadeCanvasGroup != null)
        {
            while (fadeCanvasGroup.alpha > 0f)
            {
                fadeCanvasGroup.alpha -= fadeSpeed * Time.deltaTime;
                yield return null;
            }
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        isTransitioning = false; // Restore movement controls to player
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
            Vector3 moveDirection;

            if (isTopDownMode)
            {
                moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
            }
            else
            {
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;

                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();

                moveDirection = (right * inputVector.x) + (forward * inputVector.y);
            }

            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        float rotationInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.isPressed) rotationInput = -1f;
            if (Keyboard.current.eKey.isPressed) rotationInput = 1f;
        }

        if (rotationInput != 0f)
        {
            transform.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime);
        }
    }

    void HandleTopDownZoom()
    {
        if (Mouse.current != null && topDownCamera != null)
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;

            if (scrollValue != 0)
            {
                float scrollDir = scrollValue > 0 ? -1f : 1f;
                var followComponent = topDownCamera.GetComponent<CinemachineFollow>();

                if (followComponent != null)
                {
                    Vector3 currentOffset = followComponent.FollowOffset;
                    currentOffset.y += scrollDir * topDownZoomSpeed * Time.deltaTime * 10f;
                    currentOffset.y = Mathf.Clamp(currentOffset.y, minHeight, maxHeight);
                    followComponent.FollowOffset = currentOffset;
                }
            }
        }
    }
}

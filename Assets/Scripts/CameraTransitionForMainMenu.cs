using UnityEngine;
using System.Collections;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;
// using UnityEngine.SceneManagement;
// [ExecuteInEditMode()]
public class CameraTopViewSwitcher : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;
    public Transform carTransform;     // The car the camera follows closely
    public Transform topViewPoint;     // Empty GameObject positioned for the full pulled-back top view

    [Header("Follow Settings ")]
    public float followHeight = 0f;       // How high above the car the camera sits while following
    public float followOffsetX = 0f;      // Left/right offset from the car
    public float followOffsetZ = 0f;      // Forward/back offset from the car
    public bool matchCarRotation = true;  // If true, screen rotates with the car's heading
    public float Smoothness = 0f;

    [Header("Transition Settings")]
    public float transitionDuration = 0f;
    public float delayBeforeTransition = 1f;   // <-- new: wait this long before transition starts
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isFollowingCar = true;
    private bool isTransitioning = false;
    private Coroutine activeTransition;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (carTransform != null)
        {
            targetCamera.transform.position = GetFollowPosition();
            targetCamera.transform.rotation = GetFollowRotation();
        }
    }

    private void LateUpdate()
    {
        if (isFollowingCar && !isTransitioning && carTransform != null)
        {
            targetCamera.transform.position = Vector3.Lerp(
                targetCamera.transform.position, GetFollowPosition(), 9999 * Time.deltaTime);

            targetCamera.transform.rotation = Quaternion.Slerp(
                targetCamera.transform.rotation, GetFollowRotation(), Smoothness * Time.deltaTime);

                
            // targetCamera.transform.position = Vector3.Lerp(
            //     targetCamera.transform.position, GetFollowPosition(), Time.deltaTime);

            // targetCamera.transform.rotation = Quaternion.Slerp(
            //     targetCamera.transform.rotation, GetFollowRotation(), Time.deltaTime);
        }
    }

    // Camera sits above/around the car based on the offset values
    private Vector3 GetFollowPosition()
    {
        Vector3 localOffset = new Vector3(followOffsetX, followHeight, followOffsetZ);
        return carTransform.TransformPoint(localOffset);
    }

    // Straight-down look; optionally rotates to match the car's heading (Y axis only)
    private Quaternion GetFollowRotation()
    {
        float yaw;
        if (matchCarRotation)
        {
            yaw = carTransform.eulerAngles.y;
        }
        else
        {
            yaw = 0f;
        }
        // rotation or san naka tutok camera
        Debug.Log(yaw);
        return Quaternion.Euler(0f, yaw, 0f);
    }

    // Hook this to the Button's OnClick()
    public void SwitchToTopView()
    {
        if (!isFollowingCar) return;

        if (activeTransition != null)
            StopCoroutine(activeTransition);

        activeTransition = StartCoroutine(MoveCamera(topViewPoint.position, topViewPoint.rotation));
        
        
    }

    // public void SwitchToCarView()
    // {
    //     if (activeTransition != null)
    //         StopCoroutine(activeTransition);

    //     activeTransition = StartCoroutine(ReturnToCar());
    // }

    private IEnumerator MoveCamera(Vector3 endPos, Quaternion endRot)
    {
        yield return new WaitForSeconds(delayBeforeTransition);   //  wait before transition starts, camera keeps following car during this time

        isFollowingCar = false;
        isTransitioning = true;

        Vector3 fromPos = targetCamera.transform.position;
        Quaternion fromRot = targetCamera.transform.rotation;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsed / transitionDuration);

            targetCamera.transform.position = Vector3.Lerp(fromPos, endPos, t);
            targetCamera.transform.rotation = Quaternion.Slerp(fromRot, endRot, t);

            yield return null;
        }

        targetCamera.transform.position = endPos;
        targetCamera.transform.rotation = endRot;
        isTransitioning = false;

        if (elapsed >= transitionDuration)
        {
            // SCENE CHANGE
            SceneManager.LoadScene("Main Game");
        }
    }

    // private IEnumerator ReturnToCar()
    // {
    //     isTransitioning = true;

    //     Vector3 fromPos = targetCamera.transform.position;
    //     Quaternion fromRot = targetCamera.transform.rotation;
    //     float elapsed = 0f;

    //     while (elapsed < transitionDuration)
    //     {
    //         elapsed += Time.deltaTime;
    //         float t = easeCurve.Evaluate(elapsed / transitionDuration);

    //         targetCamera.transform.position = Vector3.Lerp(fromPos, GetFollowPosition(), t);
    //         targetCamera.transform.rotation = Quaternion.Slerp(fromRot, GetFollowRotation(), t);

    //         yield return null;
    //     }

    //     isTransitioning = false;
    //     isFollowingCar = true;
    // }
}
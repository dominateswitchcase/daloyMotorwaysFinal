using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuUITransition : MonoBehaviour
{
    [System.Serializable]
    public class UIElementTransition
    {
        [Header("Target & Final Destination")]
        public RectTransform target;        // The text or button to move
        public Vector2 endPosition;         // Final resting position (anchoredPosition)
        public bool fadeOut = true;

        [Header("Anticipation (quick bounce before main move)")]
        public Vector2 anticipationOffset;  // e.g. (0, -30) = dips down briefly before going up
        public float anticipationRotation;  // degrees to tilt during the dip, e.g. -10
        [Range(0f, 0.5f)]
        public float anticipationFraction = 0.15f; // % of total duration spent on the anticipation dip

        [HideInInspector] public Vector2 startPosition;
        [HideInInspector] public CanvasGroup canvasGroup;
    }

    [Header("Elements that move UP (e.g. title text)")]
    public List<UIElementTransition> moveUpElements = new List<UIElementTransition>();

    [Header("Elements that move DOWN (e.g. buttons)")]
    public List<UIElementTransition> moveDownElements = new List<UIElementTransition>();

    [Header("Transition Settings")]
    public float transitionDuration = 1.5f;
    public AnimationCurve anticipationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // dip out
    public AnimationCurve mainCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);         // fly to final spot

    public bool IsTransitionFinished { get; private set; } = false;
    private Coroutine activeTransition;

    private void Awake()
    {
        PrepareElements(moveUpElements);
        PrepareElements(moveDownElements);
    }

    private void PrepareElements(List<UIElementTransition> elements)
    {
        foreach (var el in elements)
        {
            if (el.target == null) continue;

            el.startPosition = el.target.anchoredPosition;

            el.canvasGroup = el.target.GetComponent<CanvasGroup>();
            if (el.canvasGroup == null)
                el.canvasGroup = el.target.gameObject.AddComponent<CanvasGroup>();
        }
    }

    // Hook this to the MAGLARO (Play) button's OnClick()
    public void TriggerMenuTransition()
    {
        if (activeTransition != null)
            StopCoroutine(activeTransition);

        activeTransition = StartCoroutine(AnimateElements());
    }

    private IEnumerator AnimateElements()
    {
        IsTransitionFinished = false;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float overallT = elapsed / transitionDuration;

            AnimateGroup(moveUpElements, overallT);
            AnimateGroup(moveDownElements, overallT);

            yield return null;
        }

        AnimateGroup(moveUpElements, 1f);
        AnimateGroup(moveDownElements, 1f);

        IsTransitionFinished = true;
    }

    private void AnimateGroup(List<UIElementTransition> elements, float overallT)
    {
        foreach (var el in elements)
        {
            if (el.target == null) continue;

            Vector2 pos;
            float rot;

            if (overallT <= el.anticipationFraction)
            {
                // Phase 1: quick dip toward the anticipation offset/rotation
                float localT = overallT / el.anticipationFraction;
                float eased = anticipationCurve.Evaluate(localT);

                pos = Vector2.Lerp(el.startPosition, el.startPosition + el.anticipationOffset, eased);
                rot = Mathf.Lerp(0f, el.anticipationRotation, eased);
            }
            else
            {
                // Phase 2: from the dipped position, fly to the final resting spot
                float localT = (overallT - el.anticipationFraction) / (1f - el.anticipationFraction);
                float eased = mainCurve.Evaluate(localT);

                Vector2 dippedPos = el.startPosition + el.anticipationOffset;
                pos = Vector2.Lerp(dippedPos, el.endPosition, eased);
                rot = Mathf.Lerp(el.anticipationRotation, 0f, eased);
            }

            el.target.anchoredPosition = pos;
            el.target.localRotation = Quaternion.Euler(0f, 0f, rot);

            if (el.fadeOut && el.canvasGroup != null)
                el.canvasGroup.alpha = Mathf.Lerp(1f, 0f, overallT);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_LerpMethods_Movement : GUI_LerpMethods
{
    [SerializeField] private AnimationCurve easeCurve;
    public RectTransform Rect { get; private set; }
    public Vector2 OriginalPos { get; private set; }


    public override void PanelConfig()
    {
        Rect = this.GetComponent<RectTransform>();
        OriginalPos = Rect.anchoredPosition;
    }



    public void InitialCall(Vector2 targetPos, float lerpSpeedModifier = 1, bool deactivateSelf = false, Action followingAction = null)
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }

        runningCoroutine = InitialCallRoutine(targetPos, lerpSpeedModifier, deactivateSelf, followingAction);
        StartCoroutine(runningCoroutine);
    }

    IEnumerator InitialCallRoutine(Vector2 targetPos, float lerpSpeedModifier, bool deactivateSelf, Action followingAction)
    {
        float elapsedTime = 0f;
        Vector2 currentPos = Rect.anchoredPosition;
        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (LerpDuration * lerpSpeedModifier);
            easeFactor = easeCurve.Evaluate(easeFactor);

            Rect.anchoredPosition = Vector2.LerpUnclamped(currentPos, targetPos, easeFactor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        Rect.anchoredPosition = targetPos;

        runningCoroutine = null;
        followingAction?.Invoke();
    }

    public void FinalCallDirect()
    {
        Rect.anchoredPosition = OriginalPos;
    }

    public void FinalCall(float lerpSpeedModifier = 1, bool deactivateSelf = false, Action followingAction = null)
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = FinalCallRoutine(lerpSpeedModifier, deactivateSelf, followingAction);
        StartCoroutine(runningCoroutine);
    }

    IEnumerator FinalCallRoutine(float lerpSpeedModifier, bool deactivateSelf, Action followingAction)
    {
        float elapsedTime = 0f;
        Vector2 currentPos = Rect.anchoredPosition;

        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (LerpDuration * lerpSpeedModifier);
            easeFactor = easeCurve.Evaluate(easeFactor);

            Rect.anchoredPosition = Vector2.LerpUnclamped(currentPos, OriginalPos, easeFactor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        Rect.anchoredPosition = OriginalPos;
        followingAction?.Invoke();

        if (deactivateSelf)
        {
            this.gameObject.SetActive(false);
        }

        runningCoroutine = null;
    }
}

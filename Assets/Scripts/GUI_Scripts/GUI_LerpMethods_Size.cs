using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_LerpMethods_Size : GUI_LerpMethods
{
    private RectTransform rt;
    protected override float LerpDuration => .115f;
    //private float lerpDuration = .115f;
    //public bool cr_running = false; // later to be make autoproperty;
    [SerializeField] private AnimationCurve easeCurveDownsize;
    [SerializeField] private AnimationCurve easeCurveUpsize;

    private void Start()
    {
       PanelConfig();
    }

    public override void PanelConfig()
    {
        if (rt == null)
        {
            rt = GetComponent<RectTransform>();
        }
    }

    public void Resize(Vector2 finalSize, float lerpSpeedModifier = 1)
    {
        if (runningCoroutine != null)
        {
            return;
        }

        runningCoroutine = ResizeRoutine(finalSize, lerpSpeedModifier);
        StartCoroutine(runningCoroutine);
        //StartCoroutine(ResizeRoutine(finalSize, lerpSpeedModifier));
    }

    IEnumerator ResizeRoutine(Vector2 finalSize, float lerpSpeedModifier)
    {
        //cr_running = true;

        float elapsedTime = 0f;
        Vector2 currentSize = rt.sizeDelta;

        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (LerpDuration * lerpSpeedModifier);
            easeFactor = finalSize==Vector2.zero? easeCurveDownsize.Evaluate(easeFactor) : easeCurveUpsize.Evaluate(easeFactor) ;

            rt.sizeDelta = Vector2.LerpUnclamped(currentSize, finalSize, easeFactor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rt.sizeDelta = finalSize;

        runningCoroutine = null;
        //cr_running = false;
    }
}

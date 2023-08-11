using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_TintScale : GUI_LerpMethods
{
    [SerializeField] private AnimationCurve easeCurve;
    private RectTransform rt;
    private Vector2 originalScale = Vector2.one;
    protected override float LerpDuration => .15f;
    //private float lerpDuration = .15f;

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

    //private IEnumerator runningCoroutine = null;


    public void TintSize()
    {
        if(runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = TintSizeRoutine();
        StartCoroutine(runningCoroutine);
    }

    IEnumerator TintSizeRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < LerpDuration)
        {
            

            float easeFactor = elapsedTime / LerpDuration;
            easeFactor = easeCurve.Evaluate(easeFactor);

            Vector2 downScale = Vector2.LerpUnclamped(originalScale, originalScale*.8f, easeFactor);
            Vector2 upScale = Vector2.LerpUnclamped(originalScale * .85f, originalScale, easeFactor);

            rt.localScale = Vector2.LerpUnclamped(downScale, upScale, easeFactor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rt.localScale = originalScale;
        runningCoroutine = null;
    }
}

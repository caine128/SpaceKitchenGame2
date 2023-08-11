using System;
using System.Collections;
using UnityEngine;

public class GUI_LerpMethods_Scale : GUI_LerpMethods
{
    [SerializeField] private AnimationCurve easeCurve;
    [SerializeField] private RectTransform rt;

    //private override float  = .1f;
    protected override float LerpDuration => .1f;
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

    public void RescaleDirect(Vector3 finalScale,(Func<RectTransform,bool>finalValueChecker, Action<RectTransform> finalValueSetter)? finalValueOperations)
    {
        if (rt.localScale == finalScale 
            && finalValueOperations.HasValue 
            && finalValueOperations.Value.finalValueChecker(rt))
        {
            return;
        }
        else
        {
            rt.localScale = finalScale;
            if(finalValueOperations.HasValue) finalValueOperations.Value.finalValueSetter(rt);
        }
    }

    public void Rescale(Vector3? customInitialValue,
                       (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation,
                       Vector3 finalScale,                     
                       float lerpSpeedModifier = 1,                      
                       Action followingAction_IN = null)
    {
        /// coroutine check added to avoid situations where scale is not changed yet but couroutine has started
        /// this is applicable when another scale order is starting immediately after the previous which leaves no time for couroutine to 
        /// complete therefore localstate doesnt change which prevents from new rescale command to function.
        if ( rt.localScale == finalScale && runningCoroutine is null)
        {
            return;
        }
        /// if coroutine has started but scale is not changed yet this is to change the scale directly to be able to have the following animation
        /// ternary added as parameter inversement cehck to assure if scale corutine started but couldnt finised 
        /// to finish it directly so that the next animation can play immediately  after.
        else if (rt.localScale == finalScale && runningCoroutine is not null)
        {
            RescaleDirect(finalScale == Vector3.zero 
                                ? finalScale 
                                : Vector3.zero,
                          finalValueOperations: null);
        }

        else if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = RescaleRoutine(customInitialValue, secondaryInterpolation, finalScale, lerpSpeedModifier, followingAction_IN);
        StartCoroutine(runningCoroutine);
    }

    IEnumerator RescaleRoutine(Vector3? customInitialValue,
                               (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation,
                               Vector3 finalScale,                              
                               float lerpSpeedModifier,
                               Action followingAction_IN)
    {
        float elapsedTime = 0f;

        if (secondaryInterpolation.HasValue) secondaryInterpolation.Value.setValues(rt, true);
        if (customInitialValue.HasValue) rt.localScale = customInitialValue.Value;       
        Vector3 initialScale = rt.localScale;

        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (LerpDuration * lerpSpeedModifier);
            easeFactor = easeCurve.Evaluate(easeFactor);

            rt.localScale = Vector3.LerpUnclamped(initialScale, finalScale, easeFactor);
            secondaryInterpolation?.interpolator.Invoke(easeFactor, rt);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rt.localScale = finalScale;
        if (secondaryInterpolation.HasValue) secondaryInterpolation.Value.setValues(rt, false);
        followingAction_IN?.Invoke();

        runningCoroutine = null;
    }

    public void StopRescale()
    {
        StopCoroutine(runningCoroutine);
        runningCoroutine = null;
    }

}


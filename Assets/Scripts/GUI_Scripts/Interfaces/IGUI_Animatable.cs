using System;
using UnityEngine;

public interface IGUI_Animatable
{
    //public GUI_LerpMethods_Scale GUI_LerpMethods_Scale { protected get; protected set }

    void AnimateWithRoutine(Vector3? customInitialValue,
                            (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation, 
                            bool isVisible, 
                            float lerpSpeedModifier,
                            Action followingAction_IN);
    void ScaleDirect(bool isVisible,
                    (Func<RectTransform, bool> finalValueChecker, Action<RectTransform> finalValueSetter)? finalValueOperations);
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ContentDisplay_JustSpriteAndDoubleText : ContentDisplay_JustSpriteAndText
{
    [SerializeField] private TextMeshProUGUI textFieldAdditional;

    private RectTransform textFieldRt;
    private GUI_LerpMethods_Movement _textFieldLerper;
    private RectTransform textFieldAdditionalRt;
    private GUI_LerpMethods_Movement _textFieldAdditionalLerper;

    protected override void Awake()
    {
        base.Awake();
        textFieldRt = textField.GetComponent<RectTransform>();
        textFieldAdditionalRt = textFieldAdditional.GetComponent<RectTransform>();

        _textFieldLerper = textField.GetComponent<GUI_LerpMethods_Movement>();
        _textFieldAdditionalLerper = textFieldAdditional.GetComponent<GUI_LerpMethods_Movement>();
    }

    public sealed override void Load(ContentDisplayInfo info)
    {
        Load((ContentDisplayInfo_JustSpriteAndDoubleText)info);
    }

    private void Load(ContentDisplayInfo_JustSpriteAndDoubleText info)
    {
        textField.text = info.textVal;
        textFieldAdditional.text = info.textValAdditional;
        SelectAdressableSpritesToLoad(info.spriteRef);
    }

    public sealed override void AnimateWithRoutine(Vector3? customInitialValue, (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation, bool isVisible, float lerpSpeedModifier, Action followingAction_IN)
    {
        base.AnimateWithRoutine(customInitialValue: customInitialValue,
                               secondaryInterpolation: secondaryInterpolation,
                               isVisible: isVisible,
                               lerpSpeedModifier: lerpSpeedModifier,
                               followingAction_IN: followingAction_IN);

        textField.gameObject.SetActive(false);
        textFieldAdditional.gameObject.SetActive(false);

        textFieldRt.anchoredPosition = new Vector2(textFieldRt.rect.width / 2, textFieldRt.anchoredPosition.y);
        textFieldAdditionalRt.anchoredPosition = new Vector2(textFieldAdditionalRt.rect.width / 2, textFieldAdditionalRt.anchoredPosition.y);

        textField.gameObject.SetActive(true);
        textFieldAdditional.gameObject.SetActive(true);

        _textFieldLerper.FinalCall();
        _textFieldAdditionalLerper.FinalCall();
    }


    public Vector2 GetTotalSize()
    {
        return new Vector2(x: this.RT.rect.width + (Mathf.Max(textFieldRt.rect.width, textFieldAdditionalRt.rect.width)),
                           y: this.RT.rect.height);
    }
}

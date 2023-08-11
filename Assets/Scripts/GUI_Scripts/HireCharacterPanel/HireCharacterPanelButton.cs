using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(GUI_LerpMethods_Scale))]
public class HireCharacterPanelButton : MultiPurposeButton<ButtonFunctionType.HireCharactersPanel>, IGUI_Animatable
{
    [SerializeField] private TextMeshProUGUI buttonValueText;
    private GUI_LerpMethods_Scale gUI_LerpMethods_Scale;
    public bool IsAnimating
    {
        get => gUI_LerpMethods_Scale.RunningCoroutine is not null;
    }

    private void Awake()
    {
        gUI_LerpMethods_Scale = GetComponent<GUI_LerpMethods_Scale>();
    }

    public void AnimateWithRoutine(Vector3? customInitialValue, 
                                   (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation, 
                                   bool isVisible, 
                                   float lerpSpeedModifier, 
                                   Action followingAction_IN)
    {
        Vector3 scale = isVisible == true ? Vector3.one : Vector3.zero;
        gUI_LerpMethods_Scale.Rescale(customInitialValue: customInitialValue,
                                      secondaryInterpolation: secondaryInterpolation,
                                      finalScale: scale,
                                      lerpSpeedModifier: lerpSpeedModifier,
                                      followingAction_IN: followingAction_IN);
    }

    public void ScaleDirect(bool isVisible, 
                            (Func<RectTransform, bool> finalValueChecker, Action<RectTransform> finalValueSetter)? finalValueOperations)
    {
        if (!this.gameObject.activeSelf) gameObject.SetActive(true);
        gUI_LerpMethods_Scale.RescaleDirect(finalScale: isVisible == true
                                                ? Vector3.one
                                                : Vector3.zero,
                                            finalValueOperations: finalValueOperations);
    }

    public override void SetupButton(ButtonFunctionType.HireCharactersPanel buttonFunction_IN)
    {
        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.HireCharactersPanel.None:
                break;
            case ButtonFunctionType.HireCharactersPanel.RecruitWithGold:
                var requiredGoldToHire = ((Worker)HireCharacter_Panel.Instance.SelectedCharacter).workerspecs.goldCostForHire;
                
                buttonName.text = "Recruit With Gold";              
                buttonValueText.text = requiredGoldToHire.ToString();
                if (buttonImage_Adressable.color != Color.yellow) buttonImage_Adressable.color = Color.yellow;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("TokenIcon"));
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ()=> ((Worker)HireCharacter_Panel.Instance.SelectedCharacter).TryHireCharacter(new Gold(requiredGoldToHire));
                break;
            case ButtonFunctionType.HireCharactersPanel.RecruitWithGem:
                var requiredGemToHire = ((Worker)HireCharacter_Panel.Instance.SelectedCharacter).workerspecs.gemCostForHire;

                buttonName.text = "Recruit With Gem";
                buttonValueText.text = requiredGemToHire.ToString();
                if (buttonImage_Adressable.color != Color.blue) buttonImage_Adressable.color = Color.blue;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("GemIcon"));
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () => ((Worker)HireCharacter_Panel.Instance.SelectedCharacter).TryHireCharacter(new Gem(requiredGemToHire));
                break;
            case ButtonFunctionType.HireCharactersPanel.RecruitWithCommanderBadge:
                Debug.LogWarning(" TO BE IMPLEMENTED !");
                break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InformationModalPanelButton : MultiPurposeButton<ButtonFunctionType.InformationModalPanel>, IGUI_Animatable
{
    private ModalLoadData _modalLoadData;
    [SerializeField] TextMeshProUGUI buttonNameSecondary;
    [SerializeField ]private GUI_LerpMethods_Scale gUI_LerpMethods_Scale;


    public void AssignModalLoadData(ModalLoadData modalLoadData)
    {
        _modalLoadData = modalLoadData;
    }

    public override void SetupButton(ButtonFunctionType.InformationModalPanel buttonFunction_IN)
    {
        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.InformationModalPanel.None:
                buttonFunctionDelegate = DoNothing;
                if(buttonName.text != string.Empty) buttonName.text = string.Empty;
                if(buttonNameSecondary.text != string.Empty) buttonNameSecondary.text = string.Empty;
                buttonInnerImage_Adressable.UnloadSprite();
                break;
            case ButtonFunctionType.InformationModalPanel.Collect_Rarity:
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () => ExecuteEvents.Execute(BackgroudPanel.Instance.gameObject, new PointerEventData(EventSystem.current) , ExecuteEvents.pointerUpHandler);           
                buttonName.text = "Collect";
                if (buttonNameSecondary.text != string.Empty) buttonNameSecondary.text = string.Empty;
                buttonInnerImage_Adressable.UnloadSprite();
                break;
            case ButtonFunctionType.InformationModalPanel.Upgrade_Rarity:
                var rarityModalData = (ModalPanel_GameItemRarityUpgrade)_modalLoadData;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () =>
                {
                    var informationModalPanel = (Information_Modal_Panel)PanelManager.InvokablePanels[typeof(Information_Modal_Panel)].MainPanel;
                    informationModalPanel.HandleTask(true);                 
                };
                buttonName.text = $"Upgrade from {((IQualitative)rarityModalData.gameItem).GetQuality()}";
                buttonNameSecondary.text = "10 x";  // to change according to the formulae 
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("GemIcon"));
                break;
        }
    }

    public void StopAnimateWithRoutine()
    {
        if (gUI_LerpMethods_Scale.RunningCoroutine is not null) gUI_LerpMethods_Scale.StopRescale();
    }

    public void AnimateWithRoutine(Vector3? customInitialValue, (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation, bool isVisible, float lerpSpeedModifier, Action followingAction_IN)
    {
        ///Code to prevent disabled buttons from trying to start running coroutine and throw exception
        if (this.gameObject.activeInHierarchy != true)
            return;
        else
        {
            Vector3 scale = isVisible == true ? Vector3.one : Vector3.zero;
            gUI_LerpMethods_Scale.Rescale(customInitialValue: customInitialValue,
                                          secondaryInterpolation: secondaryInterpolation,
                                          finalScale: scale,
                                          lerpSpeedModifier: lerpSpeedModifier,
                                          followingAction_IN: followingAction_IN);
        }
    }
    public void ScaleDirect(bool isVisible, (Func<RectTransform, bool> finalValueChecker, Action<RectTransform> finalValueSetter)? finalValueOperations)
    {
        if (!this.gameObject.activeSelf) gameObject.SetActive(true);
        gUI_LerpMethods_Scale.RescaleDirect(finalScale: isVisible == true
                                                ? Vector3.one
                                                : Vector3.zero,
                                            finalValueOperations: finalValueOperations);
    }

    public override void UnloadButton()
    {
        base.UnloadButton();
        buttonNameSecondary.text = string.Empty;
        _modalLoadData = null;
    }

}

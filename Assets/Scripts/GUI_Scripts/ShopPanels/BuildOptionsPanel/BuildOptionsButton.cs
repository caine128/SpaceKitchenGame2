using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOptionsButton : MultiPurposeButton<ButtonFunctionType.BuildOptionsPanel>
{

    public override void SetupButton(ButtonFunctionType.BuildOptionsPanel buttonFunction_IN)
    {
        var selectedProp = PropManager.SelectedProp;


        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.BuildOptionsPanel.None:
                if (buttonImage_Adressable.color != Color.white) buttonImage_Adressable.color = Color.white;
                buttonFunctionDelegate = DoNothing;
                buttonName.text = string.Empty;
                buttonInnerImage_Adressable.UnloadSprite();
                break;
            case ButtonFunctionType.BuildOptionsPanel.Apply:
                if (buttonImage_Adressable.color != Color.green) buttonImage_Adressable.color = Color.green;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonName.text = $"Apply {Environment.NewLine} {selectedProp.ShopUpgradeBluePrint.GetValue()}";
                break;
            case ButtonFunctionType.BuildOptionsPanel.Rotate:
                if (buttonImage_Adressable.color != Color.red) buttonImage_Adressable.color = Color.red;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += PropManager.RotateProp;
                break;
            case ButtonFunctionType.BuildOptionsPanel.Archive:
                break;
            case ButtonFunctionType.BuildOptionsPanel.Exit:
                break;
            case ButtonFunctionType.BuildOptionsPanel.Move:
                break;
            case ButtonFunctionType.BuildOptionsPanel.Upgrade:
                break;
        }
    }
}

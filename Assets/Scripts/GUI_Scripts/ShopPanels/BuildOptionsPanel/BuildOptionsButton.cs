using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOptionsButton : MultiPurposeButton<ButtonFunctionType.BuildOptionsPanel>,IVerificationCallbackReceiver
{

    public override void SetupButton(ButtonFunctionType.BuildOptionsPanel buttonFunction_IN)
    {
        var selectedProp = PropManager.SelectedProp; // TODO : is this variable required ? ?


        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.BuildOptionsPanel.None:
                if (buttonImage_Adressable.color != Color.white) buttonImage_Adressable.color = Color.white;
                buttonFunctionDelegate = DoNothing;
                buttonName.text = string.Empty;
                buttonInnerImage_Adressable.UnloadSprite();
                break;
            case ButtonFunctionType.BuildOptionsPanel.Apply: // TODO : this might need an event listener on gold changed events !!
                if (buttonImage_Adressable.color != Color.green) buttonImage_Adressable.color = Color.green;
                buttonFunctionDelegate = gUI_TintScale.TintSize;

                var spendableRequired = selectedProp.ShopUpgradeBluePrint.PurchaseCost();

                buttonName.text = $"Apply {Environment.NewLine} {selectedProp.ShopUpgradeBluePrint.PurchaseCost()}";

                if(StatsData.IsSpendableAmountEnough(spendableRequired.Amount, spendableRequired))
                {
                    buttonName.color = Color.white;
                    buttonFunctionDelegate += () =>
                    {
                        PropManager.PurchaseProp(selectedProp);
                        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null, unloadAction:
                                () =>
                                {
                                    PanelManager.TopBarsController.ArrangeBarsFinal();
                                    PanelManager.BottomBarsController.PlaceBars();
                                    PanelManager.CraftWheelController.PlaceBars();
                                    PanelManager.ClearStackAndDeactivateElements();
                                });
                    };
                }
                else
                {
                    buttonName.color = Color.red;
                }

                break;
            case ButtonFunctionType.BuildOptionsPanel.Rotate:
                if (buttonImage_Adressable.color != Color.red) buttonImage_Adressable.color = Color.red;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += PropManager.RotateProp;
                buttonName.text = "Rotate";
                break;

            case ButtonFunctionType.BuildOptionsPanel.Archive:
                break;


            case ButtonFunctionType.BuildOptionsPanel.Exit:
                if (buttonImage_Adressable.color != Color.grey) buttonImage_Adressable.color = Color.grey;
                buttonName.text = "Back";
                
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () =>
                {

                    if (!selectedProp.IsPurchased)
                    {
                        PropManager.DestroyProp(selectedProp);
                    }
                    else
                    {
                        // TODO : Move Prop to the last valid position and release 
                        Debug.Log("Move Prop to the last valid position and release");
                    }

                    PanelManager.DeactivatePanel(invokablePanelIN: PanelManager.SelectedPanels.Peek(),
                                                 nextPanelLoadAction_IN: null);
                };
              
                break;
            case ButtonFunctionType.BuildOptionsPanel.Move:
                break;
            case ButtonFunctionType.BuildOptionsPanel.Upgrade:
                break;
        }
    }

    public void SubscribeToVerifivationCallback(bool shouldSubscribe)
    {
        /// 
    }

    public void VerificationCallback(bool isVerified)
    {
        ///
    }
}

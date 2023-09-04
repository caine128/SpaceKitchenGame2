using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class BuildOptionsButton : MultiPurposeButton<ButtonFunctionType.BuildOptionsPanel>,
                                  IVerificationCallbackReceiver,IAmountChangeCallbackReceiver
{

    public override void SetupButton(ButtonFunctionType.BuildOptionsPanel buttonFunction_IN)
    {
        SubscribeToVerifivationCallback(false);
        SubscribeToAmountChangeCallback(false);

        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.BuildOptionsPanel.None:
                if (buttonImage_Adressable.color != Color.white) buttonImage_Adressable.color = Color.white;
                buttonName.color = Color.white;
                buttonName.text = string.Empty;

                buttonFunctionDelegate = DoNothing;
                buttonInnerImage_Adressable.UnloadSprite();
                break;

            case ButtonFunctionType.BuildOptionsPanel.Purchase:

                SubscribeToVerifivationCallback(true);
                SubscribeToAmountChangeCallback(true);

                var spendableRequired = PropManager.SelectedProp.ShopUpgradeBluePrint.PurchaseCost();
                buttonName.text = $"Purchase {Environment.NewLine} {spendableRequired.Amount}";

                buttonImage_Adressable.color = PropManager.SelectedProp.HasValidPosition
                                                    ? Color.green : Color.gray;

                buttonName.color = StatsData.IsSpendableAmountEnough(spendableRequired.Amount, spendableRequired)
                                                    ? Color.white : Color.red;


                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    (bool hasValidPos, bool isSpendableAmountEnough) = (PropManager.SelectedProp.HasValidPosition, StatsData.IsSpendableAmountEnough(spendableRequired.Amount, spendableRequired));

                    if (hasValidPos && isSpendableAmountEnough)
                    {
                        PropManager.PurchaseProp(PropManager.SelectedProp);
                        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null, unloadAction:
                                () =>
                                {
                                    PanelManager.TopBarsController.ArrangeBarsFinal();
                                    PanelManager.BottomBarsController.PlaceBars();
                                    PanelManager.CraftWheelController.PlaceBars();
                                    PanelManager.ClearStackAndDeactivateElements();
                                });
                    }
                    else if (!hasValidPos)
                        Debug.Log($"{PropManager.SelectedProp.ShopUpgradeBluePrint.GetName()} doesnt have valid position");
                    else if (!isSpendableAmountEnough)
                        Debug.Log($"Not enough funds to purchase {PropManager.SelectedProp.ShopUpgradeBluePrint.GetName()}");

                };


                break;
            case ButtonFunctionType.BuildOptionsPanel.Rotate:
                if (buttonImage_Adressable.color != Color.red) buttonImage_Adressable.color = Color.red;
                buttonName.color = Color.white;
                buttonName.text = "Rotate";

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += PropManager.RotateProp;

                break;

            case ButtonFunctionType.BuildOptionsPanel.Archive:
                break;


            case ButtonFunctionType.BuildOptionsPanel.Exit:
                if (buttonImage_Adressable.color != Color.grey) buttonImage_Adressable.color = Color.grey;
                buttonName.color = Color.white;
                buttonName.text = "Back";
                
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () =>
                {

                    if (!PropManager.SelectedProp.IsPurchased)
                    {
                        PropManager.DestroyProp(PropManager.SelectedProp);
                    }
                    else
                    {
                        // Check if the prop has valid position and if not 
                        // TODO : Move Prop to the last valid position and release 
                        Debug.Log("Move Prop to the last valid position and release");
                    }

                    PanelManager.DeactivatePanel(invokablePanelIN: PanelManager.SelectedPanels.Peek(),
                                                 nextPanelLoadAction_IN: null);
                };
              
                break;
            case ButtonFunctionType.BuildOptionsPanel.Edit:
                break;
            case ButtonFunctionType.BuildOptionsPanel.Upgrade:
                break;
        }
    }

    public void SubscribeToVerifivationCallback(bool shouldSubscribe)
    {
        if (shouldSubscribe)
        {
            BuildingGrid.Instance.OnValidate += VerificationCallback;
        }
        else
        {
            BuildingGrid.Instance.OnValidate -= VerificationCallback;
        }
    }
    public void SubscribeToAmountChangeCallback(bool shouldSubscribe)
    {
        if (shouldSubscribe)
        {
            switch (PropManager.SelectedProp.ShopUpgradeBluePrint.PurchaseCost())
            {
                case Gold:
                    StatsData.OnGoldAmountChanged += AmountChangeCallback;
                    break;
                case Gem:
                    StatsData.OnGemAmountChanged += AmountChangeCallback;
                    break;
            }          
        }
        else
        {
            StatsData.OnGoldAmountChanged -= AmountChangeCallback;
            StatsData.OnGemAmountChanged -= AmountChangeCallback;
        }
    }

    public void VerificationCallback(bool isVerified)
    {
        buttonImage_Adressable.color = isVerified
                                            ? Color.green : Color.gray;
    }
    public void AmountChangeCallback(int newAmount)
    {
        buttonName.color = newAmount >= PropManager.SelectedProp.ShopUpgradeBluePrint.PurchaseCost().Amount 
                                            ? Color.white : Color.red;
    }
    public override void UnloadButton()
    {
        SubscribeToVerifivationCallback(false);
        SubscribeToAmountChangeCallback(false);
        base.UnloadButton();
    }




}

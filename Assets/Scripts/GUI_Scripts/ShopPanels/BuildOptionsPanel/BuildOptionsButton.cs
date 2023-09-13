using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class BuildOptionsButton : MultiPurposeButton<ButtonFunctionType.BuildOptionsPanel>,
                                  IVerificationCallbackReceiver, IAmountChangeCallbackReceiver, IPlacableRt
{
    public RectTransform RT { get; private set; }
    private Vector2 originalSize;

    private void Awake()
    {
        RT = GetComponent<RectTransform>();
        originalSize = RT.sizeDelta;
#if UNITY_EDITOR
        if (RT == null) throw new MissingComponentException($"missing component RT on : {this.name}");
#endif
    }

    public void SizeRevertToPriginal()
    {
        RT.sizeDelta = originalSize;
        RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, RT.sizeDelta.y / 2);
    }
    public void Resize(Vector2 sizeFactor)
    {
        RT.sizeDelta = originalSize * sizeFactor;
        RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, RT.sizeDelta.y / 2);
    }

    public void SetIsButtonRaycastTarget(bool isRaycastTarget)
        =>buttonImage_Adressable.raycastTarget = isRaycastTarget;
    

    public override void SetupButton(ButtonFunctionType.BuildOptionsPanel buttonFunction_IN)
    {
        SubscribeToVerifivationCallback(false);
        SubscribeToAmountChangeCallback(false);

        var parentPanel = (BuildOptionsPanel_Manager)PanelManager.SelectedPanels.Peek().MainPanel;

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
                    Debug.Log("purchase buton pressed");
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

                    PropManager.SelectedProp.ActivateColliders(false);
                    

                    PanelManager.DeactivatePanel(invokablePanelIN: PanelManager.SelectedPanels.Peek(),
                                                 nextPanelLoadAction_IN: null,
                                                 unloadAction: () =>
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
                                                 });
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

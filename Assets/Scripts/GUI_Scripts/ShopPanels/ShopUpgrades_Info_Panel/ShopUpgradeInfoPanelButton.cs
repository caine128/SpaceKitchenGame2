
using System;
using TMPro;
using UnityEngine;

public class ShopUpgradeInfoPanelButton : MultiPurposeButton<ButtonFunctionType.ShopUpgradeInfoPanel>
{
    [SerializeField] private TextMeshProUGUI buttonValueText;
    private (int tickAmount,int currentLevelCostPerTick) values;

    private void Awake()
    {
        buttonNames = new string[] { "Invest With Gold", "Invest With Gem", "Max Level" , "Check Progress" , "Reclaim"};
    }
    public override void SetupButton(ButtonFunctionType.ShopUpgradeInfoPanel buttonFunction_IN)
    {
        var selectedRecipe = ShopUpgradesInfoPanel_Manager.Instance.SelectedRecipe;
        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.ShopUpgradeInfoPanel.None:
                if (buttonImage_Adressable.color != Color.white) buttonImage_Adressable.color = Color.white;
                buttonFunctionDelegate = DoNothing;
                buttonName.text = buttonNames[2];
                buttonValueText.text = string.Empty;
                buttonInnerImage_Adressable.UnloadSprite();
                break;
            case ButtonFunctionType.ShopUpgradeInfoPanel.InvestWithGold:
                if (buttonImage_Adressable.color != Color.yellow) buttonImage_Adressable.color = Color.yellow;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () =>  ((IInvestable)selectedRecipe).TryInvest(
                                                      spendable: new Gold(values.tickAmount * values.currentLevelCostPerTick),
                                                      tickAmount: values.tickAmount,
                                                      tokensToReturn: out _);
                           
                buttonName.text = $"{buttonNames[0]}" ;
                buttonValueText.text = ISpendable.ToScreenFormat((values.tickAmount * values.currentLevelCostPerTick));//.ToString();
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("ValueIncrease"));
                break;
            case ButtonFunctionType.ShopUpgradeInfoPanel.InvestWithGem:
                if (buttonImage_Adressable.color != Color.blue) buttonImage_Adressable.color = Color.blue;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () => ((IInvestable)selectedRecipe).TryInvest(
                                                spendable: new Gem(values.tickAmount * values.currentLevelCostPerTick),
                                                tickAmount: values.tickAmount,
                                                tokensToReturn: out _);
               
                buttonName.text = $"{buttonNames[1]}";
                buttonValueText.text = ISpendable.ToScreenFormat((values.tickAmount * values.currentLevelCostPerTick));//.ToString();
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("GemIcon"));
                break;
            case ButtonFunctionType.ShopUpgradeInfoPanel.GoToProgressPanel:
                if (buttonImage_Adressable.color != Color.red) buttonImage_Adressable.color = Color.red;
                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    var panelToLoad = PanelManager.InvokablePanels[typeof(ProgressPopupPanel)];
                    var (ongoingUpgrades, clickedObjectIndex) = ShopData.GetOngoingUpgradesWithClickedIndex(selectedRecipe);
                    ProgressPanelLoadData panelLoadData = new(mainLoadInfo: null, panelHeader: "Current Upgrades", tcs_IN: null,
                                                                      rushableItemsData: ongoingUpgrades, clickedObjectIndex: clickedObjectIndex);

                    PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad, panelLoadAction_IN: () => ProgressPopupPanel.Instance.LoadPanel(panelLoadData));
                };

                buttonName.text = $"{buttonNames[3]}";
                buttonValueText.text = MethodHelper.GetValueStringPercent(selectedRecipe.CurrentProgress, inverseValue: false);
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("ValueIncrease"));  // TODO: To Change Sprite
                break;
            case ButtonFunctionType.ShopUpgradeInfoPanel.Collect:
                if (buttonImage_Adressable.color != Color.green) buttonImage_Adressable.color = Color.green;
                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(),
                                                 unloadAction: () => selectedRecipe.LevelUp(),
                                                 nextPanelLoadAction_IN: null);
                };

                buttonName.text = $"{buttonNames[4]}";
                buttonValueText.text = string.Empty;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("ValueIncrease"));  // TODO: To Change Sprite
                break;
            default:
                break;
        }
    }

    public void ModifyButtonValue((int newTickAmount,int costperClick) newValues)
    {
        values = newValues;
        buttonValueText.text = ISpendable.ToScreenFormat((values.tickAmount * values.currentLevelCostPerTick));//.ToString();
    }

    public void ModifyButtonValueText(float CurrentProgress)
    {
        buttonValueText.text= MethodHelper.GetValueStringPercent(CurrentProgress, inverseValue:false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TabPanel_ShopUpgradeInfo : TabPanel_Animated<Tab.ShopUpgradeInfoTabs>, IVariableButtonPanel
{
    public override Tab.ShopUpgradeInfoTabs TabType => _tabType;
    [SerializeField] private Tab.ShopUpgradeInfoTabs _tabType;

    [SerializeField] private RectTransform descriptionHolderArea_RT;
    [SerializeField] private TextMeshProUGUI shopUpgradeDescription;
    [SerializeField] private TextMeshProUGUI shopUpgradeDescriptionOnProgress;
    [SerializeField] private ContentDisplay_JustSpriteAndDoubleText[] contentDisplays;
    
    private RectTransform panel_RT;

    [SerializeField] private ShopUpgrade_CounterObjectScript counterObject;
    public RectTransform[] PopupButtons_RT { get; private set; }
    public Vector2[] PopupButtons_OriginalLocations { get; private set; }
    [SerializeField] private ShopUpgradeInfoPanelButton[] shopUpgradeInfoPanelButtons;

    ShopUpgrade selectedShopUpgrade = null;
    private void Awake()
    {
        co = new IEnumerator[1];
        panel_RT = this.GetComponent<RectTransform>();
    }

    public void ConfigureVariableButtons()
    {
        var arrayLength = shopUpgradeInfoPanelButtons.Length;
        PopupButtons_RT = new RectTransform[arrayLength];
        PopupButtons_OriginalLocations = new Vector2[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {
            PopupButtons_RT[i] = shopUpgradeInfoPanelButtons[i].GetComponent<RectTransform>();
            PopupButtons_OriginalLocations[i] = PopupButtons_RT[i].anchoredPosition;
        }
    }



    public override void LoadInfo()
    {
        Debug.Log("loading info");
        if (selectedShopUpgrade != null && selectedShopUpgrade is IInvestable investable)
        {
            investable.OnInvested -= ModifyOnInvested;
            selectedShopUpgrade.OnUpgradeTimerUpdate -= ModifyOnProgressUpdated;
        }

        selectedShopUpgrade = ShopUpgradesInfoPanel_Manager.Instance.SelectedRecipe;
        shopUpgradeDescription.text = selectedShopUpgrade.GetDescription();

        switch (selectedShopUpgrade)
        {
            case IRushable rushable when rushable.RemainingDuration > 0:
                if (shopUpgradeDescriptionOnProgress.gameObject.activeInHierarchy != true) shopUpgradeDescriptionOnProgress.gameObject.SetActive(true);
                if (counterObject.gameObject.activeInHierarchy != false) counterObject.gameObject.SetActive(false);

                shopUpgradeDescriptionOnProgress.text = NativeHelper.BuildString_Append(
                    MethodHelper.GiveRichTextString_Color(Color.green),
                    MethodHelper.GiveRichTextString_Size(125),
                    selectedShopUpgrade.GetName(), 
                    MethodHelper.GiveRichTextString_ClosingTagOf("color"),
                    MethodHelper.GiveRichTextString_ClosingTagOf("size"),
                    " is being upgraded to",
                    MethodHelper.GiveRichTextString_Color(Color.green),
                    MethodHelper.GiveRichTextString_Size(125),
                    " Level ",
                    (selectedShopUpgrade.GetLevel() + 1).ToString(),
                    MethodHelper.GiveRichTextString_ClosingTagOf("color"),
                    MethodHelper.GiveRichTextString_ClosingTagOf("size"),
                    " You can check the progress by clicking the button.");
                    //$"{selectedShopUpgrade.GetName()} is being upgraded to Level {selectedShopUpgrade.GetLevel() + 1}. You can check the progress by clicking the button.";
                GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplays);
                
                (this as IVariableButtonPanel).SetButtonLayout(buttonAmountToDisplay: 1);
                shopUpgradeInfoPanelButtons[1].SetupButton(ButtonFunctionType.ShopUpgradeInfoPanel.GoToProgressPanel);

                break;
            case IRushable rushable when rushable.IsReadyToReclaim:
                if (shopUpgradeDescriptionOnProgress.gameObject.activeInHierarchy != true) shopUpgradeDescriptionOnProgress.gameObject.SetActive(true);
                if (counterObject.gameObject.activeInHierarchy != false) counterObject.gameObject.SetActive(false);

                shopUpgradeDescriptionOnProgress.text = NativeHelper.BuildString_Append(
                    MethodHelper.GiveRichTextString_Color(Color.green),
                    MethodHelper.GiveRichTextString_Size(125),
                    selectedShopUpgrade.GetName(),
                    MethodHelper.GiveRichTextString_ClosingTagOf("color"),
                    MethodHelper.GiveRichTextString_ClosingTagOf("size"),
                    " is upgraded to",
                    MethodHelper.GiveRichTextString_Color(Color.green),
                    MethodHelper.GiveRichTextString_Size(125),
                    " Level ",
                    (selectedShopUpgrade.GetLevel() + 1).ToString(),
                    MethodHelper.GiveRichTextString_ClosingTagOf("color"),
                    MethodHelper.GiveRichTextString_ClosingTagOf("size"),
                    " Click Reclaim button to collect the item.");
                //$"{selectedShopUpgrade.GetName()} is upgraded to Level {selectedShopUpgrade.GetLevel() + 1}. Click Collect button to collect the upgraded {selectedShopUpgrade.shopUpgradeType}";
                GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplays);

                (this as IVariableButtonPanel).SetButtonLayout(buttonAmountToDisplay: 1);
                shopUpgradeInfoPanelButtons[1].SetupButton(ButtonFunctionType.ShopUpgradeInfoPanel.Collect);

                break;
            default:
                if (shopUpgradeDescriptionOnProgress.gameObject.activeInHierarchy != false) shopUpgradeDescriptionOnProgress.gameObject.SetActive(false);
                if (counterObject.gameObject.activeInHierarchy != true) counterObject.gameObject.SetActive(true);

                var isMaxLevel = selectedShopUpgrade.isAtMaxLevel;

                (this as IVariableButtonPanel).SetButtonLayout(buttonAmountToDisplay: 2);

                shopUpgradeInfoPanelButtons[0].SetupButton(isMaxLevel ? ButtonFunctionType.ShopUpgradeInfoPanel.None : ButtonFunctionType.ShopUpgradeInfoPanel.InvestWithGold);
                shopUpgradeInfoPanelButtons[1].SetupButton(isMaxLevel ? ButtonFunctionType.ShopUpgradeInfoPanel.None : ButtonFunctionType.ShopUpgradeInfoPanel.InvestWithGem);

                /*if (!isMaxLevel) counterObject.OncounterModified += ModifyOnCounterChanged;
                if (!isMaxLevel && selectedRecipe is WorkStationUpgrade workStationUpgrade) workStationUpgrade.OnWorkstationInvested += ModifyOnInvested;*/

                /* var contentDisplayInfos = selectedRecipe.GetDisplayableBenefits().ConvertEnumerable(tuple =>
                         new ContentDisplayInfo_JustSpriteAndDoubleText(
                                 textValAdditional_IN: tuple.benefitValue, textVal_IN: tuple.benefitName, spriteRef_IN: tuple.bnefitIcon));*/
                var contentDisplayInfos = selectedShopUpgrade.GetDisplayableBenefits().Select(tuple =>
                        new ContentDisplayInfo_JustSpriteAndDoubleText(
                                textValAdditional_IN: tuple.benefitValue, textVal_IN: tuple.benefitName, spriteRef_IN: tuple.bnefitIcon));

                var positionsAndcontainerHeight = GUI_CentralPlacement.MatrixPlacementCalculation(containingPanel_RT: panel_RT,
                                                                container_RT: contentDisplays[0].RT,
                                                                requiredAmount: contentDisplayInfos.Count(),
                                                                customContainerSize: contentDisplays[0].GetTotalSize());

                var offsetCorrectionX = (contentDisplays[0].RT.rect.width - contentDisplays[0].GetTotalSize().x) / 2;
                var offsetCorrectionY = descriptionHolderArea_RT.rect.height / 2;
                var correctedPositions = positionsAndcontainerHeight.positions.Select(pos => new Vector2(pos.x + offsetCorrectionX, pos.y - offsetCorrectionY));

                contentDisplays.PlaceContainersMatrix(correctedPositions.ToArray());
                contentDisplays.LoadContainers(contentDisplayInfos, hideAtInit: true);

                break;
        }
        /*var isMaxLevel = selectedRecipe.isAtMaxLevel;

        (this as IVariableButtonPanel).SetButtonLayout(buttonAmountToDisplay: 2);

        shopUpgradeInfoPanelButtons[0].SetupButton(isMaxLevel ? ButtonFunctionType.ShopUpgradeInfoPanel.None : ButtonFunctionType.ShopUpgradeInfoPanel.InvestWithGold);
        shopUpgradeInfoPanelButtons[1].SetupButton(isMaxLevel ? ButtonFunctionType.ShopUpgradeInfoPanel.None : ButtonFunctionType.ShopUpgradeInfoPanel.InvestWithGem);



    
        var contentDisplayInfos = selectedRecipe.GetDisplayableBenefits().Select(tuple =>
                new ContentDisplayInfo_JustSpriteAndDoubleText(
                        textValAdditional_IN: tuple.benefitValue, textVal_IN: tuple.benefitName, spriteRef_IN: tuple.bnefitIcon));

        var positionsAndcontainerHeight = GUI_CentralPlacement.MatrixPlacementCalculation(containingPanel_RT: panel_RT,
                                                        container_RT: contentDisplays[0].RT,
                                                        requiredAmount: contentDisplayInfos.Count(),
                                                        customContainerSize: contentDisplays[0].GetTotalSize());

        var offsetCorrectionX = (contentDisplays[0].RT.rect.width - contentDisplays[0].GetTotalSize().x) /2;
        var offsetCorrectionY = descriptionHolderArea_RT.rect.height / 2;
        var correctedPositions = positionsAndcontainerHeight.positions.Select(pos => new Vector2(pos.x + offsetCorrectionX, pos.y - offsetCorrectionY));
        
        contentDisplays.PlaceContainersMatrix(correctedPositions.ToArray());
        contentDisplays.LoadContainers(contentDisplayInfos, hideAtInit: true);   */ 
    }

    public override void DisplayContainers()
    {
        Debug.Log("displaying info");

        //var selectedRecipe = ShopUpgradesInfoPanel_Manager.Instance.SelectedRecipe;
        var rushable = (IRushable)selectedShopUpgrade;

        if(rushable.RemainingDuration > 0)
        {
            selectedShopUpgrade.OnUpgradeTimerUpdate += ModifyOnProgressUpdated;
        }

        if (rushable.RemainingDuration <= 0 && !rushable.IsReadyToReclaim)
        {
            var isMaxLevel = selectedShopUpgrade.isAtMaxLevel;

            if (!isMaxLevel) counterObject.OncounterModified += ModifyOnCounterChanged;
            if (!isMaxLevel && selectedShopUpgrade is IInvestable investable) investable.OnInvested += ModifyOnInvested;

            contentDisplays.SortContainers(customInitialValues: null,
                                           secondaryInterpolations: null,
                                           amountToSort_IN: selectedShopUpgrade.GetDisplayableBenefits().Count(),
                                           enumeratorIndex: 0,
                                           parentPanel_IN: this,
                                           lerpSpeedModifiers: null);
            counterObject.Initialize(selectedShopUpgrade);
        }        
    }

    private void ModifyOnProgressUpdated()
    {

        if (selectedShopUpgrade.RemainingDuration > 0)
        {
            shopUpgradeInfoPanelButtons[1].ModifyButtonValueText(selectedShopUpgrade.CurrentProgress);
        }
        else
        {
            selectedShopUpgrade.OnUpgradeTimerUpdate -= ModifyOnProgressUpdated;
            shopUpgradeDescriptionOnProgress.text = $"{selectedShopUpgrade.GetName()} is upgraded to Level {selectedShopUpgrade.GetLevel() + 1}. Click Collect button to collect the upgraded {selectedShopUpgrade.shopUpgradeType}";
            shopUpgradeInfoPanelButtons[1].SetupButton(ButtonFunctionType.ShopUpgradeInfoPanel.Collect);
        }
    }

    private void ModifyOnCounterChanged()
    {
        var costsPerTick = ((IInvestable)ShopUpgradesInfoPanel_Manager.Instance.SelectedRecipe).GetCostsPerTick();
        
        /// Modify Invest With Gold Button
        shopUpgradeInfoPanelButtons[0].ModifyButtonValue((counterObject.CurrentAmount, costsPerTick.gold));
        /// Modify Invest With Gem Button
        shopUpgradeInfoPanelButtons[1].ModifyButtonValue((counterObject.CurrentAmount, costsPerTick.gem));
    }

    private void ModifyOnInvested()
    {
        Debug.Log("on invested is called");
        counterObject.UpdateValuesOnInvested();

        ModifyOnCounterChanged();
    }

    public override void HideContainers()
    {
        Debug.Log("hiding containers");

        contentDisplays.HideContainers();
        counterObject.OncounterModified -= ModifyOnCounterChanged;
        selectedShopUpgrade.OnUpgradeTimerUpdate -= ModifyOnProgressUpdated;
        if (selectedShopUpgrade is IInvestable investable)
        {
            investable.OnInvested -= ModifyOnInvested;
        }
    }

    public override void UnloadInfo()
    {
        Debug.Log("unloading info");
        selectedShopUpgrade = null;

        foreach (var shopUpgradeBenefit in contentDisplays)
        {
            shopUpgradeBenefit.Unload();
        }

        foreach (var button in shopUpgradeInfoPanelButtons)
        {
            button.UnloadButton();
        }
    }

}

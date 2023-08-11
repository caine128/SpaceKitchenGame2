
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeInfoPanel_Manager : TabbedPanel<ProductRecipe, Tab.RecipeInfoTabs> //, IRefreshablePanel
{
    private static RecipeInfoPanel_Manager _instance;
    public static RecipeInfoPanel_Manager Instance { get { return _instance; } }
    public override List<ProductRecipe> ListToIterate => PanelManager.SelectedPanels.Any(sp => sp.MainPanel is CraftPanel_Manager)
                                                              ? CraftPanel_Manager.activeSelection_MainType switch
                                                                     {
                                                                         EquipmentType.Type.None => RecipeManager.RecipesAvailable_List,
                                                                         { } => RecipeManager.recipesAvailable_Dict[CraftPanel_Manager.activeSelection_MainType][CraftPanel_Manager.activeSelection_SubtType.GetDerivedType()],
                                                                     }
                                                              : RecipeManager.RecipesAvailable_List.Where(ra => ra == SelectedRecipe).ToList();
         /*//{
             true => CraftPanel_Manager.activeSelection_MainType switch
             {
                 EquipmentType.Type.None => RecipeManager.RecipesAvailable_List,
                 { } => RecipeManager.recipesAvailable_Dict[CraftPanel_Manager.activeSelection_MainType][CraftPanel_Manager.activeSelection_SubtType.GetDerivedType()],
             },
             false => RecipeManager.RecipesAvailable_List.Where(ra => ra == selectedRecipe).ToList()
         //};
                                    //== (EquipmentType.Type)((Worker)CharactersInfoPanel_Manager.Instance.selectedRecipe).workerspecs.workStationPrerequisites[0].type).ToList()
              

        CraftPanel_Manager.activeSelection_MainType switch //CraftPanel_Manager.activeSelection.active_MainType switch
        {
            EquipmentType.Type.None => RecipeManager.RecipesAvailable_List,
            { } => RecipeManager.recipesAvailable_Dict[CraftPanel_Manager.activeSelection_MainType][CraftPanel_Manager.activeSelection_SubtType.GetDerivedType()],
                   
        };*/
       

    private static readonly object _lock = new object();

    [SerializeField] private RecipeInfoPanelButton button;
    [SerializeField] private TextMeshProUGUI buttonDisplayInfo;
    [SerializeField] private TextMeshProUGUI amountInInventoryText;
    [SerializeField] private TextMeshProUGUI productValueText;
 
    protected void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                }
            }
        }
    }

    public void Ascend()
    {
        if (SelectedRecipe.ascensionLevel == AscensionLevel.Type.Avatar)
        {
            return;
        }
        else
        {
            var amountOfshardsNeeded = SelectedRecipe.recipeSpecs.ascensionUpgrades[(int)SelectedRecipe.ascensionLevel].shardsNeeded;
            var ascensionShard = new AscensionShard(SpecialItemType.Type.AscensionShard);

            if (Inventory.Instance.RemoveFromInventory(ascensionShard, amountOfshardsNeeded))
            {
                var ascensionUpgradeType = SelectedRecipe.UpdateAscensionLevel();
                //AscensionTreeManager.Instance.ProcessNewAscensionTreeStatus(productType_IN: selectedRecipe.recipeSpecs.productType,
                                                                           // amountToAdd_IN: 1);
                ReloadPanelInfo(ascensionUpgradeType);
                RefreshPanel();
            }
            else
            {
                Debug.Log("not enough shards!, Sorry");
            }
        }
    }

    public override void InitialConfigBrowserButtons()
    {
        foreach (var browserButton in _browserButtons)
        {
            browserButton.ButtonConfig(Instance); 
        }
    }

    protected sealed override void LoadInfo(ProductRecipe productRecipe)  // NEED TO STOP FROM BEING LOADED ON SAME PRODUCT
    {
        var comparer = new RecipeEqualityComparer();
        if (SelectedRecipe is null || !comparer.Equals(SelectedRecipe,productRecipe))  //(selectedRecipe != productRecipe)
        {
            SelectedRecipe = productRecipe;

            blueprintNameText.text = SelectedRecipe.GetName();
            bluePrintTypeLevelInfoText.text = string.Format("Level {0} {1} Recipe", SelectedRecipe.GetLevel(), SelectedRecipe.recipeSpecs.productType.ToString());

            blueprintRarityText.text = SelectedRecipe.masteryLevel.ToString();
        }

        bigImageContainer_Adressable.LoadSprite(SelectedRecipe.GetAdressableImage());
        thumbnailImageContainer_Adressable.LoadSprite(SelectedRecipe.GetAdressableImage());

        blueprintTypeText.text = string.Format("Ascension Type : {0} ", SelectedRecipe.ascensionLevel.ToString());
        productValueText.SetAsModifiableSpec(ISpendable.ToScreenFormat(SelectedRecipe.GetValue()), SelectedRecipe.IsValueModified);
        amountInInventoryText.text = Inventory.Instance.CheckAmountInInventory_Name(SelectedRecipe.GetName(), GameItemType.Type.Product).ToString();

        foreach (TabPanel<Tab.RecipeInfoTabs> tabPanel in tabPanels)
        {
            tabPanel.LoadInfo();
        }

        var browsablePanelInterface = ((IBrowsablePanel<ProductRecipe>)this);

        _currentIndice = browsablePanelInterface.SetCurrentIndice(SelectedRecipe);
        browsablePanelInterface.SetVisibilityBrowserButtons();

        ExecuteEvents.Execute(tabSelectorButtons[lastSelectionIndex].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        DefineButtonFunction(tabSelectorButtons[lastSelectionIndex].TabType);
    }

    private void DefineButtonFunction(Tab.RecipeInfoTabs tabType_IN)
    {
        if (tabType_IN == Tab.RecipeInfoTabs.AscensionTab)
        {
            if ((int)SelectedRecipe.ascensionLevel < SelectedRecipe.recipeSpecs.ascensionUpgrades.Length)
            {
                button.SetupButton(ButtonFunctionType.RecipeInfoPanel.AscendButton); //RecipeInfoPanelButton.Function.AscendButton
                buttonDisplayInfo.SetAsModifiableSpec_Comparaison(Inventory.Instance.CheckAmountInInventory_Name(SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.ascensionShardInfo.name, GameItemType.Type.SpecialItem), SelectedRecipe.recipeSpecs.ascensionUpgrades[(int)SelectedRecipe.ascensionLevel].shardsNeeded);

            }
            else
            {
                button.SetupButton(ButtonFunctionType.RecipeInfoPanel.None); // RecipeInfoPanelButton.Function.None);
                buttonDisplayInfo.text = null;
            }
        }
        else
        {
            button.SetupButton(ButtonFunctionType.RecipeInfoPanel.CraftButton); // RecipeInfoPanelButton.Function.CraftButton);
            buttonDisplayInfo.SetAsModifiableSpec(ConvertTime.ToHourMinSec(SelectedRecipe.GetCraftDuration()), SelectedRecipe.GetCraftDuration_Modification());
        }
    }

    public override void PlaceTabPanel(Tab.RecipeInfoTabs tabType_IN, TabSelectorButton<Tab.RecipeInfoTabs> button_IN)
    {
        base.PlaceTabPanel(tabType_IN, button_IN);

        DefineButtonFunction(tabType_IN);
    }

    public void ReloadPanelInfo(Recipes_SO.AscensionUpgradeType ascensionUpgradeType)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            if (tabPanels[i] is TabPanel_Ascension || tabPanels[i] is TabPanel_Stats)
            {
                tabPanels[i].LoadInfo();
            }
        }

        switch (ascensionUpgradeType)
        {
            case Recipes_SO.AscensionUpgradeType.RequiredProductReduction:
            case Recipes_SO.AscensionUpgradeType.IngredientReduction:
            case Recipes_SO.AscensionUpgradeType.ExtraComponentReduction:
                for (int i = 0; i < tabPanels.Length; i++)
                {
                    if(tabPanels[i] is TabPanel_Recipe)
                    {
                        tabPanels[i].LoadInfo();
                    }
                }
                break;
            case Recipes_SO.AscensionUpgradeType.CraftTimeReduction:
            case Recipes_SO.AscensionUpgradeType.QualityChanceIncrease:
            case Recipes_SO.AscensionUpgradeType.MultiCraftChance:
                break;
        }

        blueprintTypeText.text = string.Format("Ascension Type : {0} ", SelectedRecipe.ascensionLevel.ToString());
        DefineButtonFunction(activeTabType);
    }

    public void ReloadPanelInfo(AscensionTree_SO.AscensionTreeRewardType ascensionTreeRewardType)
    {
        switch (ascensionTreeRewardType)
        {
            case AscensionTree_SO.AscensionTreeRewardType.GoldReward:
            case AscensionTree_SO.AscensionTreeRewardType.GemReward:
            case AscensionTree_SO.AscensionTreeRewardType.WorkerXPIncreaseModifier:
            case AscensionTree_SO.AscensionTreeRewardType.ReduceSurchargeEnergyModifier:
                Debug.Log(ascensionTreeRewardType + " is received");
                break;
            case AscensionTree_SO.AscensionTreeRewardType.MultiCraftChance:
            case AscensionTree_SO.AscensionTreeRewardType.QualityChanceIncrease:
            case AscensionTree_SO.AscensionTreeRewardType.IngredientsReduction:
                for (int i = 0; i < tabPanels.Length; i++)
                {
                    if (tabPanels[i] is TabPanel_Recipe tabPanel_Recipe)
                    {
                        tabPanel_Recipe.LoadInfo();
                        Debug.Log(ascensionTreeRewardType + " is received ");
                    }
                }
                break;
            case AscensionTree_SO.AscensionTreeRewardType.CommanderBadge:
            case AscensionTree_SO.AscensionTreeRewardType.SurchargeValueIncreasemodifier:
                Debug.Log(ascensionTreeRewardType + " is received");
                break;
        }
    }

    public override void UnloadAndDeallocate()
    {
        button.UnloadButton();
        base.UnloadAndDeallocate();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class ProductRecipe : SortableBluePrint, IValueUpgradable, IRankable, IAmountable, IDatable, IToolTipDisplayable
{
    public readonly Recipes_SO recipeSpecs;
    public readonly DateTime dateUnlocked;
    public CraftMasteryLevel.Type masteryLevel { get; private set; }
    public AscensionLevel.Type ascensionLevel { get; private set; }
    public int amountCraftedGlobal { get; private set; }
    public int amountCraftedLocal { get; private set; }
    public int goldGenerated { get; private set; }
    public DateTime DateLastCrafted { get; private set; }

    private int[] requiredIngredientsReductions;
    private int[] requiredAdditionalItemReductions;


    private bool isUnlocked;
    private bool isResearched;
    private float craftTimeReductionModifier;
    public bool IsValueModified => _valueIncreaseModifiersFromCraftUpgrades.Any();
    public IEnumerable<(string bonusName, string bonusAmount)> ValueIncreaseModifierStrings 
    {
        get => _valueIncreaseModifiersFromCraftUpgrades.Select(vim => (vim.bonusName, $"x {vim.bonusAmount}"))
                                                       .Concat(Enumerable.Repeat(("Base Value", ISpendable.ToScreenFormat(recipeSpecs.productValue)), 1));
    }
    private List<(string bonusName, float bonusAmount)> _valueIncreaseModifiersFromCraftUpgrades;
    private List<int> qualityChanceModifiers; // { get; private set; }     
    private float multicraftChanceModifier; // { get; private set; }

    public bool[] AreWorkerRequirementsMet { get; private set; }

    public List<Single_CraftedItem> itemHolderSlotsList = new List<Single_CraftedItem>();  // WHY THIS LIST IS EXPOSED ??   

    public ProductRecipe
        (Recipes_SO recipeSpecsIN,
        CraftMasteryLevel.Type masteryLevelIN = CraftMasteryLevel.Type.Beginner,
        AscensionLevel.Type ascensionLevelIN = AscensionLevel.Type.None,
        int amountCraftedGlobalIN = 0,
        int amountCraftedLocallIN = 0,
        int goldGeneratedIN = 0,
        DateTime? dateUnlocked_IN = null,
        DateTime? dateLastCraftedIN = null,
        float craftTimeReductionModifierIN = 1f,
        float valueIncreaseModifierIN = 1f,
        List<(string,float)> valueIncreaseModifiers_IN = null,
        List<int> qualityChanceModifiersIN = null,
        float multicraftChanceModifierIN = 0,
        int[] requiredIngredientsReductions_IN = null,
        int[] requiredAdditionalItemReductions_IN = null,
        bool isUnlockedIN = false,    /// We updated this to default to false beucase it was possible to create unlocked recipes by mistake and subscribe to events resulting in memory leaks
        bool isResearchedIN = false)

    {
        this.recipeSpecs = recipeSpecsIN;
        this.masteryLevel = masteryLevelIN;
        this.ascensionLevel = ascensionLevelIN;
        this.amountCraftedGlobal = amountCraftedGlobalIN;
        this.amountCraftedLocal = amountCraftedLocallIN;
        this.goldGenerated = goldGeneratedIN;
        this.dateUnlocked = dateUnlocked_IN ?? DateTime.Today;
        this.DateLastCrafted = dateLastCraftedIN ?? DateTime.MinValue;
        this.craftTimeReductionModifier = craftTimeReductionModifierIN;
        //this.valueIncreaseModifier = valueIncreaseModifierIN;
        this._valueIncreaseModifiersFromCraftUpgrades = valueIncreaseModifiers_IN ?? new();
        this.qualityChanceModifiers = qualityChanceModifiersIN ?? new List<int>() { 1 };
        this.multicraftChanceModifier = multicraftChanceModifierIN;
        this.requiredIngredientsReductions = requiredIngredientsReductions_IN ?? new int[recipeSpecsIN.requiredIngredients.Length];
        this.requiredAdditionalItemReductions = requiredAdditionalItemReductions_IN ?? new int[recipeSpecsIN.requiredAdditionalItems.Length];
        this.isUnlocked = isUnlockedIN;
        this.isResearched = isResearchedIN;

        if (isUnlocked)
        {
            this.AreWorkerRequirementsMet = new bool[recipeSpecs.requiredworkers.Length];
            SubscribeToWorkerEvents();
            CheckWorkerRequirements();

            Radial_CraftSlots_Crafter.Instance.onStartCrafting += SetUpCurrentCraftedQueue;
            Radial_CraftSlots_Crafter.Instance.onReclaimCrafted += UpdateAmountCrafted;
            Radial_CraftSlots_Crafter.Instance.onReclaimCrafted += DequeueReclaimedItem;

            ModalPanel_DisplayBonuses_ProductRecipeUnlockOrResearch modalLoadData = new(productRecipe: this, modalState_IN: Information_Modal_Panel.ModalState.ProductRecipe_UnlockedOrResearched);
            var panelToInvoke = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];

            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                         panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                                    modalLoadDatas: Enumerable.Empty<ModalLoadData>()),
                                         alternativeLoadAction_IN: () =>
                                         {
                                             var panel = (Information_Modal_Panel)panelToInvoke.MainPanel;
                                             panel.ModalLoadDataQueue.Enqueue(modalLoadData);
                                         });
        }

        /// We moved this event subscriptions to the IF UNLOCED block becuase we dont' want memory leaks 
        /// caused by objects created which we assume gonna be garbage collected.
        //Radial_CraftSlots_Crafter.Instance.onStartCrafting += SetUpCurrentCraftedQueue;
        //Radial_CraftSlots_Crafter.Instance.onReclaimCrafted += UpdateAmountCrafted;
        //Radial_CraftSlots_Crafter.Instance.onReclaimCrafted += DequeueReclaimedItem;
    }

    public ToolTipInfo GetToolTipText()
    {
        var retStr1 = NativeHelper.BuildString_Append("Amount",
                                                      Environment.NewLine,
                                                      "Lvl.",
                                                      Environment.NewLine,
                                                      "Value");

        var retStr2 = NativeHelper.BuildString_Append(GetAmount().ToString(),
                                                       Environment.NewLine,
                                                       GetLevel().ToString(),
                                                       Environment.NewLine,
                                                       ISpendable.ToScreenFormat(GetValue()));//.ToString());

        return new ToolTipInfo(bodytextAsColumns: new string[] {retStr1.ToString(), retStr2.ToString()},
                               header:GetName(),
                               footer:GetDescription());
    }

   /* public string GetToolTipTextForModifiers_Value()
    {
        StringBuilder sb = new();
        var lastModifierItem = ValueIncreaseModifiers.Last();

        foreach (var modifier in ValueIncreaseModifiers)
        {
            sb.Append(modifier.bonusName).Append(' ', 10).Append(modifier.bonusAmount);

            if (!modifier.Equals(lastModifierItem))
                sb.AppendLine();
        }
        return sb.ToString();
    }*/

    private void SubscribeToWorkerEvents()
    {
        for (int i = 0; i < recipeSpecs.requiredworkers.Length; i++)
        {
            CharacterManager.WorkerEventsDict[recipeSpecs.requiredworkers[i].requiredWorker] += CheckWorkerRequirements;
        }
    }

    private void CheckWorkerRequirements()
    {
        for (int i = 0; i < AreWorkerRequirementsMet.Length; i++)
        {
            switch (AreWorkerRequirementsMet[i])
            {
                case (true):
                    continue;

                case (false):

                    var existingWorker = CharacterManager.CharactersAvailable_Dict[CharacterType.Type.Worker]
                                         .FirstOrDefault(chr => ((Worker)chr).workerspecs.workerType == recipeSpecs.requiredworkers[i].requiredWorker);

                    AreWorkerRequirementsMet[i] = (existingWorker, existingWorker?.isHired, existingWorker?.GetLevel()) switch
                    {
                        (Worker, true, var existingWorkerLevel) when existingWorkerLevel >= recipeSpecs.requiredworkers[i].requiredWorkerLevel => true,
                        _ => false,
                    };


                    if (AreWorkerRequirementsMet[i])
                    {
                        CharacterManager.WorkerEventsDict[recipeSpecs.requiredworkers[i].requiredWorker] -= CheckWorkerRequirements;
                    }

                    break;
            }
        }
    }


    public void UpdateAmountCrafted(object sender, Radial_CraftSlots_Crafter.OnCraftingEventArgs e)
    {
        if (e.productRecipe != this)
        {
            return;
        }
        else
        {
            amountCraftedGlobal += e.amountCrafted;
            amountCraftedLocal += e.amountCrafted;

            if (masteryLevel == CraftMasteryLevel.Type.Master)
            {
                return;
            }
            UpdateMasteryLevel();
            SetLastCraftedTime();
        }
    }

    private void UpdateMasteryLevel()
    {
        int currentMasteryLevel = (int)masteryLevel;
        if (amountCraftedLocal >= recipeSpecs.craftingUpgrades[currentMasteryLevel].craftsNeeded)
        {
            amountCraftedLocal = 0;

            var craftUpgradeType = recipeSpecs.craftingUpgrades[currentMasteryLevel].craftUpgradeType;
            ApplyMasteryLevelBonus(craftUpgradeType, currentMasteryLevel, out string bonusString, out AssetReferenceT<Sprite> bonusSprite, out Action actionToDelay);

            var panelToInvoke = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];
            //var isPanelAlreadyActive = PanelManager.SelectedPanels.TryPeek(out InvokablePanelController activePanel) && activePanel.MainPanel == invokablePanel_Modal.MainPanel;
            var modalLoadData = this.ConditionalyCreateFrom(condition: craftUpgradeType == Recipes_SO.CraftUpgradeType.UnlockRecipe,
                                                            ifTrueFunc: (@this) =>
                                                            {
                                                                var recipeToUnlock = recipeSpecs.craftingUpgrades[currentMasteryLevel].unlockRecipe;
                                                                return new ModalPanel_DisplayBonuses_LoadData(mainSprite_IN: recipeToUnlock.receipeImageRef,
                                                                                                          secondarySprite_IN: bonusSprite,
                                                                                                          bonusExplanationStringTuple_IN: (recipeToUnlock.recipeName, string.Empty),
                                                                                                          subheaderString_IN: $"You have unlocked {recipeToUnlock.recipeName}! Research it to start cooking it!",
                                                                                                          modalState_IN: Information_Modal_Panel.ModalState.RecipeUpgrade);

                                                            },
                                                            ifFalseFunc: (@this) =>
                                                            {
                                                                //var recipeUpgradeTypeName = craftUpgradeType.ToString();
                                                                return new ModalPanel_DisplayBonuses_LoadData(mainSprite_IN: recipeSpecs.receipeImageRef,
                                                                                                       secondarySprite_IN: bonusSprite,
                                                                                                       bonusExplanationStringTuple_IN: (craftUpgradeType.GetNameOfTheCraftingUpgradeType(), bonusString),
                                                                                                       subheaderString_IN: $"Milestone upgrade unlocked for {GetName()}",
                                                                                                       modalState_IN: Information_Modal_Panel.ModalState.RecipeUpgrade);
                                                            }
                                                            );

            masteryLevel = CraftMasteryLevel.GetNextCraftMasteryLevel(masteryLevel);

            //if (!isPanelAlreadyActive)
            //{

            /// TODO: MAYbe THIS MIGHT BE USEFUL OR TAKE IT AT THE END OF THE MODAL DISPLAY 
            /// /// PanelManager.ClearStackAndDeactivateElements();///

            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                         panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                             modalLoadDatas: Enumerable.Empty<ModalLoadData>()),
                                         alternativeLoadAction_IN: () =>
                                         {
                                             var panel = (Information_Modal_Panel)panelToInvoke.MainPanel;
                                             panel.ModalLoadDataQueue.Enqueue(modalLoadData);
                                         });

            actionToDelay?.Invoke();
            //}
            /*else
            {
                var panel = ((Information_Modal_Panel)activePanel.MainPanel);
                panel.ModalLoadDataQueue.Enqueue(modalLoadData);
            }*/

        }
    }

    private void ApplyMasteryLevelBonus(Recipes_SO.CraftUpgradeType craftUpgradeTypeIN, int currentMasteryLevelIN, out string bonusString, out AssetReferenceT<Sprite> bonusSprite, out Action actionToDelay)
    {
        actionToDelay = null;
        switch (craftUpgradeTypeIN)
        {
            case Recipes_SO.CraftUpgradeType.CraftTimeReduction:
                var timeRedTemp = recipeSpecs.craftingUpgrades[currentMasteryLevelIN].craftTimeReductionModifier;
                craftTimeReductionModifier *= timeRedTemp;

                bonusString = craftUpgradeTypeIN.AppendCraftingUpgradeBonusToSTring(timeRedTemp);
                bonusSprite = ImageManager.SelectSprite(craftUpgradeTypeIN.ToString());
                break;
            case Recipes_SO.CraftUpgradeType.IngredientReduction:
                var ingRedTemp = recipeSpecs.craftingUpgrades[currentMasteryLevelIN].ingredientReduction.reductionAmount;
                var ingTypeTemp = recipeSpecs.craftingUpgrades[currentMasteryLevelIN].ingredientReduction.ingredient;
                UpdateRequiredIngredients(ingTypeTemp, ingRedTemp);

                bonusString = craftUpgradeTypeIN.AppendCraftingUpgradeBonusToSTring(ingRedTemp);
                bonusSprite = ResourcesManager.Instance.Resources_SO.ingredients[(int)ingTypeTemp].spriteRef;
                break;
            case Recipes_SO.CraftUpgradeType.ExtraComponentReduction:
                var exCompRedTem = recipeSpecs.craftingUpgrades[currentMasteryLevelIN].extraComponentReduction.reductionAmount;
                var exCompTypeTemp = recipeSpecs.craftingUpgrades[currentMasteryLevelIN].extraComponentReduction.extraComponent;
                UpdateRequiredExtraComponents(exCompTypeTemp, exCompRedTem);

                bonusString = craftUpgradeTypeIN.AppendCraftingUpgradeBonusToSTring(exCompRedTem);
                bonusSprite = ResourcesManager.Instance.Resources_SO.extraComponentsList[ExtraComponentsType.GetNormalizedEnumIndex(exCompTypeTemp)].spriteRef;
                break;
            case Recipes_SO.CraftUpgradeType.ValueIncrease:
                var valIncTemp = recipeSpecs.craftingUpgrades[currentMasteryLevelIN].valueIncreaseModifier;
                //valueIncreaseModifier *= valIncTemp;

                _valueIncreaseModifiersFromCraftUpgrades.Add(("Craft Upgrade Bonus", valIncTemp));

                bonusString = craftUpgradeTypeIN.AppendCraftingUpgradeBonusToSTring(valIncTemp);
                bonusSprite = ImageManager.SelectSprite(craftUpgradeTypeIN.ToString());
                break;
            case Recipes_SO.CraftUpgradeType.QualityChanceIncrease:
                var qualIncTemp = recipeSpecs.craftingUpgrades[currentMasteryLevelIN].qualityChanceIncreaseModifier;
                qualityChanceModifiers.Add(qualIncTemp);

                bonusString = craftUpgradeTypeIN.AppendCraftingUpgradeBonusToSTring(qualIncTemp);
                bonusSprite = ImageManager.SelectSprite(craftUpgradeTypeIN.ToString());
                break;
            case Recipes_SO.CraftUpgradeType.UnlockRecipe:
                actionToDelay = () => RecipeManager.Instance.AddNewRecipe(recipeSpecs.craftingUpgrades[currentMasteryLevelIN].unlockRecipe);

                bonusString = String.Empty;
                bonusSprite = ImageManager.SelectSprite(craftUpgradeTypeIN.ToString());
                break;
            default:
                Debug.LogError("shouldn Come here !");
                bonusString = String.Empty;
                bonusSprite = null;
                break;
        }
    }

    public Recipes_SO.AscensionUpgradeType UpdateAscensionLevel()
    {
        int currentAscensionLevel = (int)ascensionLevel;
        var ascensionUpgradeType = recipeSpecs.ascensionUpgrades[currentAscensionLevel].ascensionUpgradeType;
        ApplyAscensionLevelBonus(ascensionUpgradeType, currentAscensionLevel, out string bonusString, out AssetReferenceT<Sprite> bonusSprite);

        AscensionTreeManager.Instance.ProcessNewAscensionTreeStatus(productType_IN: recipeSpecs.productType,
                                                                    amountToAdd_IN: 1);

        var panelToInvoke = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];
        // var isPanelAlreadyActive = PanelManager.SelectedPanels.TryPeek(out InvokablePanelController activePanel) && activePanel.MainPanel == invokablePanel_Modal.MainPanel;

        var modalLoadData = new ModalPanel_DisplayBonuses_Ascension_LoadData(ascensionLevel_IN: ascensionLevel,
                                                                             mainSprite_IN: GetAdressableImage(),
                                                                             secondarySprite_IN: bonusSprite,
                                                                             bonusExplanationStringTuple_IN: (ascensionUpgradeType.GetNameOfTheAscensionUpgradeType(), bonusString),
                                                                             subheaderString_IN: $"Your {GetName()} recipe has ascended!",
                                                                             modalState_IN: Information_Modal_Panel.ModalState.AscensionUpgrade); ;


        //PanelManager.ClearStackAndDeactivateElements();
        //PanelManager.SelectedPanels.Push(PanelManager.InvokablePanels[typeof(AscensionLadderPanel_Manager)]);
        //if (isPanelAlreadyActive) Debug.LogError("The panel shouldnt be recurring again here ");

        PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                     preLoadAction_IN: () =>
                                     {
                                         PanelManager.SelectedPanels.Push(PanelManager.InvokablePanels[typeof(AscensionLadderPanel_Manager)]);
                                         AscensionLadderPanel_Manager.activeSelection_MainType = recipeSpecs.productType;
                                     },
                                     panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                               modalLoadDatas: Enumerable.Empty<ModalLoadData>()),
                                     alternativeLoadAction_IN: () =>
                                     {
                                         var panel = (Information_Modal_Panel)panelToInvoke.MainPanel;
                                         panel.ModalLoadDataQueue.Enqueue(modalLoadData);
                                     });

        ascensionLevel = AscensionLevel.GetNextAscensionLevel(ascensionLevel);
        return ascensionUpgradeType;
    }

    private void ApplyAscensionLevelBonus(Recipes_SO.AscensionUpgradeType ascensionUpgradeTypeIn, int currentAscensionLevelIN, out string bonusString, out AssetReferenceT<Sprite> bonusSprite)
    {
        switch (ascensionUpgradeTypeIn)
        {
            case Recipes_SO.AscensionUpgradeType.CraftTimeReduction:
                var timeRedTemp = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].craftTimeReductionModifier;
                craftTimeReductionModifier *= timeRedTemp;

                bonusString = ascensionUpgradeTypeIn.AppendAscensionUpgradeBonusToSTring(timeRedTemp);
                bonusSprite = ImageManager.SelectSprite(ascensionUpgradeTypeIn.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.IngredientReduction:
                var ingRedTemp = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].ingredientReduction.reductionAmount;
                var ingTypeTemp = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].ingredientReduction.ingredient;
                UpdateRequiredIngredients(ingTypeTemp, ingRedTemp);

                bonusString = ascensionUpgradeTypeIn.AppendAscensionUpgradeBonusToSTring(ingRedTemp);
                bonusSprite = ResourcesManager.Instance.Resources_SO.ingredients[(int)ingTypeTemp].spriteRef;
                break;
            case Recipes_SO.AscensionUpgradeType.ExtraComponentReduction:
                var exCompRedTemp = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].extraComponentReduction.reductionAmount;
                var exCompTypeTemp = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].extraComponentReduction.extraComponent;
                UpdateRequiredExtraComponents(exCompTypeTemp, exCompRedTemp);
                //extraComponentReductions.Add(recipeSpecs.craftingUpgrades[currentMasteryLevelIN].extraComponentReduction);

                bonusString = ascensionUpgradeTypeIn.AppendAscensionUpgradeBonusToSTring(exCompRedTemp);
                bonusSprite = ResourcesManager.Instance.Resources_SO.extraComponentsList[ExtraComponentsType.GetNormalizedEnumIndex(exCompTypeTemp)].spriteRef;
                break;
            case Recipes_SO.AscensionUpgradeType.QualityChanceIncrease:
                var qualIncTemp = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].qualityChanceIncreaseModifier;
                qualityChanceModifiers.Add(qualIncTemp);
                //qualityChanceModifiers *= recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].qualityChanceIncreaseModifier;

                bonusString = ascensionUpgradeTypeIn.AppendAscensionUpgradeBonusToSTring(qualIncTemp);
                bonusSprite = ImageManager.SelectSprite(ascensionUpgradeTypeIn.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.MultiCraftChance:
                var multiIncTemp = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].multicraftChanceModifier;
                multicraftChanceModifier += multiIncTemp;

                bonusString = ascensionUpgradeTypeIn.AppendAscensionUpgradeBonusToSTring(multiIncTemp);
                bonusSprite = ImageManager.SelectSprite(ascensionUpgradeTypeIn.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.RequiredProductReduction:
                var reqProdRedTemp = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].requiredProductReduction.reductionAmount;
                var reqProdType = recipeSpecs.ascensionUpgrades[currentAscensionLevelIN].requiredProductReduction.requiredProduct_Name;
                UpdateRequiredProducts(reqProdType, reqProdRedTemp);
                //extraComponentReductions.Add(recipeSpecs.craftingUpgrades[currentMasteryLevelIN].extraComponentReduction);

                bonusString = ascensionUpgradeTypeIn.AppendAscensionUpgradeBonusToSTring(reqProdRedTemp);
                bonusSprite = reqProdType.receipeImageRef;
                break;
            default:
                Debug.LogError("shouldn Come here !");
                bonusString = String.Empty;
                bonusSprite = null;
                break;
        }
    }

    private void UpdateRequiredExtraComponents(ExtraComponentsType.Type extraComponentType, int reductionAmount)
    {
        for (int i = 0; i < recipeSpecs.requiredAdditionalItems.Length; i++)
        {
            if (recipeSpecs.requiredAdditionalItems[i].requiredExtraComponentsType == Recipes_SO.RequiredAdditionalItemType.ExtraComponents && recipeSpecs.requiredAdditionalItems[i].requiredExtraComponent.extraComponentType == extraComponentType)
            {
                requiredAdditionalItemReductions[i] += reductionAmount;
            }
        }
    }

    private void UpdateRequiredProducts(Recipes_SO product, int reductionAmount)
    {
        for (int i = 0; i < recipeSpecs.requiredAdditionalItems.Length; i++)
        {
            if (recipeSpecs.requiredAdditionalItems[i].requiredExtraComponentsType == Recipes_SO.RequiredAdditionalItemType.Product && recipeSpecs.requiredAdditionalItems[i].requiredProduct.requiredProduct_Name == product)
            {
                requiredAdditionalItemReductions[i] += reductionAmount;
            }
        }
    }

    private void UpdateRequiredIngredients(IngredientType.Type ingredientType, int reductionAmount)
    {
        for (int i = 0; i < recipeSpecs.requiredIngredients.Length; i++)
        {
            if (recipeSpecs.requiredIngredients[i].ingredient == ingredientType)
            {
                requiredIngredientsReductions[i] += reductionAmount;
            }
        }
    }

    private void SetUpCurrentCraftedQueue(object sender, Radial_CraftSlots_Crafter.OnCraftingEventArgs e)
    {
        if (e.productRecipe != this)
        {
            return;
        }
        else
        {
            itemHolderSlotsList.Add(e.itemHolderSlot);
        }
    }


    private void DequeueReclaimedItem(object sender, Radial_CraftSlots_Crafter.OnCraftingEventArgs e)
    {
        if (e.productRecipe != this)
        {
            return;
        }
        else if (itemHolderSlotsList.Count > 0)
        {
            foreach (Single_CraftedItem single_CraftedItem in itemHolderSlotsList)
            {
                if (e.itemHolderSlot == single_CraftedItem)
                {
                    itemHolderSlotsList.Remove(single_CraftedItem);
                    return;
                }
            }
            //itemHolderSlotQueue.Dequeue();
        }
    }



    public int GetLevel()
    {
        return recipeSpecs.productLevel;
    }

    public int GetValue()
        => Mathf.CeilToInt(recipeSpecs.productValue * _valueIncreaseModifiersFromCraftUpgrades.Select(vim => vim.bonusAmount).Aggregate(seed: 1f, (acc, next) => acc * next));

    /*public bool GetValue_Modification()
        => (_valueIncreaseModifiersFromCraftUpgrades.Select(vim => vim.bonusAmount).Aggregate(seed: 1f, (acc, next) => acc * next)) != 1 ;*/

    public override string GetName()
    {
        return recipeSpecs.recipeName;
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return recipeSpecs.receipeImageRef;
    }

    public int GetRequiredIngredients(int indexNo, out IngredientType.Type ingredientType, out bool isModified)
    {
        var typeOfIngreident = recipeSpecs.requiredIngredients[indexNo].ingredient;

        var ascensionTreeIngredientReductionBonuses = AscensionTreeManager.Instance.QueryAscensionRewardState(recipeSpecs.productType).rewardsAndStates //   AscensionTreeManager.AscensionTreeRewardsOfAllProducts.TryGetValue(recipeSpecs.productType, out AscensionTreeRewardState ascensionTreeRewardState)
                                                                                                                                                        //? ascensionTreeRewardState.rewardsAndStates
                                            .Where(rs => rs.IsClaimed && rs.reward.ascensionTreeRewardType == AscensionTree_SO.AscensionTreeRewardType.IngredientsReduction)
                                            //  reward => reward.ascensionTreeRewardType == AscensionTree_SO.AscensionTreeRewardType.IngredientsReduction)
                                            .SelectMany(rs => rs.reward.ingredientsReduction.ingredientReduction)  //  reward => reward.ingredientsReduction.ingredientReduction)
                                            .Where(reducedingredient => reducedingredient.ingredient == typeOfIngreident)
                                            .Aggregate(1.0f, (totalMultiplier, reducedIngredient) => totalMultiplier * reducedIngredient.reductionAmountPercent);
        //: 1.0f;

        ingredientType = typeOfIngreident; //recipeSpecs.requiredIngredients[indexNo].ingredient; // Had To Do this Because Out Paramenter Cannot be Used in LINQ

        isModified = requiredIngredientsReductions[indexNo] > 0 ? true : false || ascensionTreeIngredientReductionBonuses != 1 ? true : false;
        return Mathf.Max(Mathf.FloorToInt((recipeSpecs.requiredIngredients[indexNo].amountRequired - requiredIngredientsReductions[indexNo]) * ascensionTreeIngredientReductionBonuses), 1);
        //return requiredIngredients[indexNo].requiredAmount - requiredIngredients[indexNo].reductionAmount;                  

    }

    public bool GetRequiredIngredients_Modification(int indexNo)  // covered already in previous OUT eventually to delete !!! 
    {
        return requiredIngredientsReductions[indexNo] > 0 ? true : false;
        //return requiredIngredients[indexNo].reductionAmount > 0 ? true : false;      
    }

    public int GetRequiredAdditionalItems(int indexNo, out GameItemType.Type? additionalItemType, out bool? isModified)
    {
        switch (recipeSpecs.requiredAdditionalItems[indexNo].requiredExtraComponentsType)
        {
            case Recipes_SO.RequiredAdditionalItemType.ExtraComponents:
                additionalItemType = GameItemType.Type.ExtraComponents;
                isModified = requiredAdditionalItemReductions[indexNo] > 0 ? true : false;
                return recipeSpecs.requiredAdditionalItems[indexNo].requiredExtraComponent.amountRequired - requiredAdditionalItemReductions[indexNo];

            case Recipes_SO.RequiredAdditionalItemType.Product:
                additionalItemType = GameItemType.Type.Product;
                isModified = requiredAdditionalItemReductions[indexNo] > 0 ? true : false;
                return recipeSpecs.requiredAdditionalItems[indexNo].requiredProduct.amountRequired - requiredAdditionalItemReductions[indexNo];

            case Recipes_SO.RequiredAdditionalItemType.None:
            default:
                additionalItemType = null;
                isModified = null;
                return default(int);
        }
    }

    public bool GetRequiredAdditionalItems_Modification(int indexNO) // covered already in previous OUT eventually to delete !!! 
    {
        return requiredAdditionalItemReductions[indexNO] > 0 ? true : false;
    }

    public float GetCraftDuration()
    {
        return recipeSpecs.craftDuration * craftTimeReductionModifier;
    }
    public bool GetCraftDuration_Modification()
    {
        if (craftTimeReductionModifier != 1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public IEnumerable<int> GetQualityModifers()
    {
        var ascensionTreeQualityModifierBonuses = AscensionTreeManager.Instance.QueryAscensionRewardState(recipeSpecs.productType).rewardsAndStates //      AscensionTreeManager.AscensionTreeRewardsOfAllProducts.TryGetValue(recipeSpecs.productType, out AscensionTreeRewardState ascensionTreeRewardState)
                                                                                                                                                    // ? ascensionTreeRewardState.rewardsAndStates // claimedAscensionRewards
                                           .Where(rs => rs.IsClaimed && rs.reward.ascensionTreeRewardType == AscensionTree_SO.AscensionTreeRewardType.QualityChanceIncrease)  // reward => reward.ascensionTreeRewardType == AscensionTree_SO.AscensionTreeRewardType.QualityChanceIncrease)
                                           .Select(rs => rs.reward.qualityChanceIncreaseModifier); //  reward => reward.qualityChanceIncreaseModifier)
                                                                                                   // : Enumerable.Empty<int>();

        foreach (var qualityChanceModifier in qualityChanceModifiers)
        {
            yield return qualityChanceModifier;
        }
        foreach (var ascensionTreeQualityModifierBonus in ascensionTreeQualityModifierBonuses)
        {
            yield return ascensionTreeQualityModifierBonus;
        }

        //return qualityChanceModifiers;
    }

    public float GetMultiCraftChanceModifier()
    {
        var ascensionTreeMultiCrraftModifierBonusSum = AscensionTreeManager.Instance.QueryAscensionRewardState(recipeSpecs.productType).rewardsAndStates //  AscensionTreeManager.AscensionTreeRewardsOfAllProducts.TryGetValue(recipeSpecs.productType, out AscensionTreeRewardState ascensionTreeRewardState)
                                                                                                                                                         //? ascensionTreeRewardState.rewardsAndStates // claimedAscensionRewards
                                            .Where(rs => rs.IsClaimed && rs.reward.ascensionTreeRewardType == AscensionTree_SO.AscensionTreeRewardType.MultiCraftChance) //  reward => reward.ascensionTreeRewardType == AscensionTree_SO.AscensionTreeRewardType.MultiCraftChance)
                                            .Select(rs => rs.reward.multicraftChanceModifier) //  reward => reward.multicraftChanceModifier)
                                            .Sum();
        //: 0f;

        return this.multicraftChanceModifier + ascensionTreeMultiCrraftModifierBonusSum;
    }


    public bool IsUnlocked()
    {
        return isUnlocked;
    }

    public bool IsResearched()
    {
        return isResearched;
    }

    public void Research()
    {
        isResearched = true;

       /* ModalPanel_DisplayBonuses_ProductRecipeUnlockOrResearch modalLoadData = new(productRecipe: this, modalState_IN: Information_Modal_Panel.ModalState.ProductRecipe_UnlockedOrResearched);
        var panelToInvoke = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];

        PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                     panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                                modalLoadDatas: Enumerable.Empty<ModalLoadData>()),
                                     alternativeLoadAction_IN: () =>
                                     {
                                         var panel = (Information_Modal_Panel)panelToInvoke.MainPanel;
                                         panel.ModalLoadDataQueue.Enqueue(modalLoadData);
                                     });*/
    }
    public int GetResearchPointsRequired()
    {
        return recipeSpecs.researchPointsRequired;
    }

    public int GetTotalRequiredAmount(int indexNo)
    {
        int totalAmount = 0;
        for (int i = indexNo; i > -1; i--)
        {
            totalAmount += recipeSpecs.craftingUpgrades[i].craftsNeeded;
        }

        return totalAmount;
    }

    public override string GetDescription()
    {
        return recipeSpecs.recipeDescription;
    }

    public int GetAmount()
    {
        return Inventory.Instance.CheckAmountInInventory_ByNameDict(this.GetName(), out _);
    }

    public DateTime GetLastCraftedDate()
    {

        return DateLastCrafted;
    }

    public void SetLastCraftedTime()
    {
        DateLastCrafted = DateTime.Now;
    }
}

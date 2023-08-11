using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Radial_CraftSlots_Crafter : MonoBehaviour //, IMultiPanelInvokeButton
{
    #region Singleton Syntax
    private static Radial_CraftSlots_Crafter _instance;
    public static Radial_CraftSlots_Crafter Instance { get { return _instance; } }
    private static readonly object _lock = new object();
    #endregion

    [SerializeField] private RectTransform craftedItems_Parent;
    [SerializeField] private RectTransform craftSlots_Parent;
    [SerializeField] Single_CraftedItem[] craftedItems;
    [SerializeField] Single_Craftslot[] craftSlots;

    public int maxCraftSlotsForLevel { get; private set; } = 12; // LATER TO BE ARRANGED 
    [SerializeField] public int activeCraftAmount { get; private set; } = 0;

    //public InvokablePanelController PanelToInvoke { get { return panelToInvoke; } }           // this has to go 
    //[SerializeField] protected InvokablePanelController panelToInvoke;                  // this has to go 

    //public InvokablePanelController[] InvokablePanels { get { return _invokablePanels; } }
    //[SerializeField] InvokablePanelController[] _invokablePanels;

    public event Action<InvokablePanelController, Action> OnInvokeButtonPressed;

    private TaskCompletionSource<bool> tcs;

    private static Dictionary<bool, List<(GameObject itemToRemove, int amountToRemove)>> itemsToRemove_Dict = new Dictionary<bool, List<(GameObject itemToRemove, int amountToRemove)>>
            {
                {true, new List<(GameObject itemToRemove, int amountToRemove)>()},
                {false, new List<(GameObject itemToRemove, int amountToRemove)>()},
            };


    public event EventHandler<OnCraftingEventArgs> onStartCrafting;
    public event EventHandler<OnCraftingEventArgs> onReclaimCrafted;

    public class OnCraftingEventArgs
    {
        public int activeCraftAmount;
        public int remainingCraftAmount;
        public ProductRecipe productRecipe;
        public Single_CraftedItem itemHolderSlot;
        public int amountCrafted;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (Instance == null)
                {
                    _instance = this; // LATER TO ADD DONTDESTROY ON LOAD
                }
            }
        }
    }

    public async Task TryStartCraftingAsync(ProductRecipe productRecipeIN)
    {
        if (IsEnoughItemHolders() == false)
        {
            Debug.Log("There is not enough item Holders");
            Radial_CraftSlots_Controller.Instance.SetRotation(SpinType.Type.BackSpin);
            return;
        }

        else if (!productRecipeIN.IsUnlocked())
        {
            if (productRecipeIN.recipeSpecs.unlockPrerequisite.Length > 0)
            {
                var panelToLoad = PanelManager.InvokablePanels[typeof(UnlockRecipePopupPanel)];
                PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad,
                                             panelLoadAction_IN: () => UnlockRecipePopupPanel.Instance.LoadPanel(new PanelLoadData(mainLoadInfo: productRecipeIN,
                                                                                                                                  panelHeader: null,
                                                                                                                                  tcs_IN: null)));
            }
        }

        else if (!productRecipeIN.IsResearched())
        {
            var panelToLoad = PanelManager.InvokablePanels[typeof(ResearchPopupPanel)];
            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad,
                                         panelLoadAction_IN: () => ResearchPopupPanel.Instance.LoadPanel(new PanelLoadData(mainLoadInfo: productRecipeIN,
                                                                                                                           panelHeader: null,
                                                                                                                           tcs_IN: null)));

            Radial_CraftSlots_Controller.Instance.SetRotation(SpinType.Type.BackSpin);  // IS THIS NECESSARY ??
            return;
        }
        else
        {
            if(!AreWorkerRequirementsMet(productRecipeIN))
            {
                var panelToLoad = PanelManager.InvokablePanels[typeof(MissingRequirementsPopupPanel)];
                var missingWorkersList = GetMissingWorkers(productRecipeIN).ToList();
                PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad,
                                             panelLoadAction_IN:
                                             () => MissingRequirementsPopupPanel.Instance.LoadPanel(
                                                 new PanelLoadDatas(mainLoadInfo: productRecipeIN,
                                                                                             panelHeader: missingWorkersList.Count>1 
                                                                                                           ? "Missing Workers"
                                                                                                           : "Missing Worker",
                                                                                             tcs_IN: null,
                                                                                             bluePrintsToLoad: missingWorkersList)));

                Debug.Log("workers not completed");
            }
            else if (GetMissingResourceCabinets(productRecipeIN).Any())
            {
                var panelToLoad = PanelManager.InvokablePanels[typeof(MissingRequirementsPopupPanel)];
                var missingResourceCabinetsList = GetMissingResourceCabinets(productRecipeIN).ToList();
                PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad,
                                             panelLoadAction_IN:
                                             () => MissingRequirementsPopupPanel.Instance.LoadPanel(
                                                 new PanelLoadDatas(mainLoadInfo: productRecipeIN,
                                                                                             panelHeader: missingResourceCabinetsList.Count > 1
                                                                                                           ? "Missing Resource Cabinets"
                                                                                                           : "Missing Resource Cabinet",
                                                                                             tcs_IN: null,
                                                                                             bluePrintsToLoad: missingResourceCabinetsList)));
            }



            else if(!await IsEnoughResourcesAsync(productRecipeIN))
            {
                Debug.Log("not enough resources please wait");
            }

            else
            {
                Debug.Log("it is ok");
                Single_CraftedItem itemHolderToSelect = SelectItemHolder();
                PlaceItemHolderToCraftSlot(itemHolderToSelect, productRecipeIN);
            }
        }
    }

    private IEnumerable<(SortableBluePrint,int)> GetMissingResourceCabinets(ProductRecipe productRecipe_IN)
    {
        var requiredIngredients = productRecipe_IN.recipeSpecs.requiredIngredients;
        for (int i = 0; i < requiredIngredients.Length; i++)
        {
            var resourceCabinet = ShopUpgradesManager.Instance.GetRelevantResourceCabinet(ResourcesManager.FindIngredient(requiredIngredients[i].ingredient));
            if (!ShopData.CheckPresenceOfUpgrade(resourceCabinet, out _))
            {
                yield return (new ResourceCabinetUpgrade(indexNo_IN: ((ResourceCabinetUpgrade)resourceCabinet).IndexNo,
                                                        shopUpgradeType: ShopUpgradeType.Type.ResourceCabinetUpgrade,
                                                        tier_IN: ((ResourceCabinetUpgrade)resourceCabinet).Tier,
                                                        arbitraryLevel:0),0);
            }
        }
    }


    private bool AreWorkerRequirementsMet(ProductRecipe productRecipeIN) => productRecipeIN.AreWorkerRequirementsMet.All(awm => awm);
 
    private IEnumerable<(SortableBluePrint, int)> GetMissingWorkers(ProductRecipe productRecipeIN) // this is not necessary here 
    {
        for (int i = 0; i < productRecipeIN.AreWorkerRequirementsMet.Length; i++)
        {
           
            if (!productRecipeIN.AreWorkerRequirementsMet[i])          
            {
                var charToreturn = CharacterManager.CharactersAvailable_Dict[CharacterType.Type.Worker].FirstOrDefault(chr => ((Worker)chr).workerspecs.workerType == productRecipeIN.recipeSpecs.requiredworkers[i].requiredWorker);
                yield return charToreturn switch
                {
                    null => (new Worker(_workerSpecs_IN: CharacterManager.Instance.WorkerList_SO.listOfWorkers.First(chso => chso.workerType == productRecipeIN.recipeSpecs.requiredworkers[i].requiredWorker), arbitraryLevel: 0),
                                  productRecipeIN.recipeSpecs.requiredworkers[i].requiredWorkerLevel),
                    { } => (charToreturn, productRecipeIN.recipeSpecs.requiredworkers[i].requiredWorkerLevel)
                };
            }
        }
    }

    private async Task<bool> IsEnoughResourcesAsync(ProductRecipe productRecipeIN)
    {
        ClearItemsToRemoveDict();

        var maxMissingResourcesListLength = productRecipeIN.recipeSpecs.requiredIngredients.Length + productRecipeIN.recipeSpecs.requiredAdditionalItems.Length;
        List<(SortableBluePrint missingItem, int requiredAmount)> missingResources = new List<(SortableBluePrint, int)>(maxMissingResourcesListLength);

        for (int i = 0; i < productRecipeIN.recipeSpecs.requiredIngredients.Length; i++)
        {
            var requiredAmount = productRecipeIN.GetRequiredIngredients(i, out _, out _);
            if (ResourcesManager.CheckAmountOfIngredient(productRecipeIN.recipeSpecs.requiredIngredients[i].ingredient, out Ingredient ingredient_Out) >= requiredAmount) continue;

            else
            {
                missingResources.Add((ingredient_Out, requiredAmount));
            }
        }

        if (productRecipeIN.recipeSpecs.requiredAdditionalItems.Length > 0)
        {

            for (int i = 0; i < productRecipeIN.recipeSpecs.requiredAdditionalItems.Length; i++)
            {
                var requiredAmount = productRecipeIN.GetRequiredAdditionalItems(i, out GameItemType.Type? itemType, out _);
                int existingAmount;

                switch (itemType)
                {
                    case GameItemType.Type.Product:
                    case GameItemType.Type.Enhancement:

                        existingAmount = Inventory.Instance.CheckAmountInInventory_ByNameDict(productRecipeIN.recipeSpecs.requiredAdditionalItems[i].requiredProduct.requiredProduct_Name.recipeName, out List<GameObject> gameItemsToRemove);

                        if (existingAmount < requiredAmount && itemType == GameItemType.Type.Product)
                        {
                            var requiredProductRecipe_SO = productRecipeIN.recipeSpecs.requiredAdditionalItems[i].requiredProduct.requiredProduct_Name;
                            missingResources.Add((new Product(new ProductRecipe(requiredProductRecipe_SO), qualityLevel_IN: productRecipeIN.recipeSpecs.requiredAdditionalItems[i].requiredProduct.requiredProductQuality), requiredAmount));
                        }
                        else if (existingAmount < requiredAmount && itemType == GameItemType.Type.Enhancement)
                        {
                            var requiredProductRecipe_SO = productRecipeIN.recipeSpecs.requiredAdditionalItems[i].requiredProduct.requiredProduct_Name;
                            missingResources.Add((new Enhancement(new ProductRecipe(requiredProductRecipe_SO), qualityLevel_IN: productRecipeIN.recipeSpecs.requiredAdditionalItems[i].requiredProduct.requiredProductQuality), requiredAmount));
                        }
                        else if (existingAmount >= requiredAmount && missingResources.Count == 0)
                        {
                            MapFlaggedItemsBeforeConsume(gameItemsToRemove, requiredAmount);
                        }
                        break;

                    case GameItemType.Type.ExtraComponents:
                        existingAmount = Inventory.Instance.CheckAmountInInventory(productRecipeIN.recipeSpecs.requiredAdditionalItems[i].requiredExtraComponent.extraComponentType, out ExtraComponent extraComponentToRemove);

                        if (existingAmount < requiredAmount)
                        {
                            var extraComponent = extraComponentToRemove ?? new ExtraComponent(ExtraComponentsType.GetNormalizedEnumIndex(productRecipeIN.recipeSpecs.requiredAdditionalItems[i].requiredExtraComponent.extraComponentType));
                            missingResources.Add((extraComponent, requiredAmount));
                        }
                        else if (existingAmount >= requiredAmount && missingResources.Count == 0)
                        {
                            itemsToRemove_Dict[false].Add((extraComponentToRemove, requiredAmount));
                        }
                        break;

                    case GameItemType.Type.SpecialItem:
                        throw new NotImplementedException();
                }
            }

            if (itemsToRemove_Dict.TryGetValue(true, out List<(GameObject itemToRemove, int amountToRemove)> flaggedItemsList) && flaggedItemsList.Count > 0 && missingResources.Count == 0)
            {
                tcs = new TaskCompletionSource<bool>();
    
                /*if (_invokablePanels[1].MainPanel is ConfirmationPopupPanel)
                {*/

                    var panelLoadData = new PopupPanel_Confirmation_LoadData(mainLoadInfo: productRecipeIN, panelHeader: null, tcs_IN: tcs,
                        bluePrintsToLoad: flaggedItemsList,
                        extraDescription_IN : NativeHelper.BuildString_Append("Are you sure to dismantle those items to craft ",Environment.NewLine,
                        productRecipeIN.GetName(), " ?"));

                    var invokablePanel = PanelManager.InvokablePanels[typeof(ConfirmationPopupPanel)]; 

                    PanelManager.ActivateAndLoad(
                        invokablePanel_IN: invokablePanel, //_invokablePanels[1], 
                        panelLoadAction_IN: () => ((ConfirmationPopupPanel)invokablePanel.MainPanel).LoadPanel(panelLoadData));     
               /* }
                else
                {
                    Debug.Log("Wrong Panel Type Sent To Load");
                }*/


                /*foreach (var (itemToRemove, amountToRemove) in flaggedItemsList) // this line is for debug purpopses !! 
                {
                    Debug.Log(("There is {0} items in the list  ", flaggedItemsList.Count) + itemToRemove.GetName() + "you need to destroy of this item pcs : " + amountToRemove + "of quality : " + ((IQualitative)itemToRemove).GetQuality());
                }

                Debug.Log("tcs in the crafter" + tcs);*/

                await tcs.Task;
                ///Deactivate the confimation beucase it doesn't have self deactivation.
                if (PanelManager.SelectedPanels.Peek().MainPanel is ConfirmationPopupPanel confirmationPanel)
                {
                    PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null, unloadAction: null);
                }

                if (tcs.Task.Result == false)
                {
                    Debug.Log("doest want to spend important items");
                    return false;
                }

            }
        }


        if (missingResources.Count > 0)
        {
            tcs = new TaskCompletionSource<bool>();

                var panelToLoad = PanelManager.InvokablePanels[typeof(MissingRequirementsPopupPanel)];
                var panelLoadData = new PanelLoadDatas(mainLoadInfo: null, panelHeader: null, tcs_IN: tcs,
                    bluePrintsToLoad: missingResources);
                PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad, panelLoadAction_IN:
                                              () => MissingRequirementsPopupPanel.Instance.LoadPanel(panelLoadData));

            await tcs.Task;
                               
            if (tcs.Task.Result == false)
            {
                Debug.Log("Doesnt want to provide missing resources");
                return false;
            }
        }


        foreach (var kvPair in itemsToRemove_Dict)
        {
            if (itemsToRemove_Dict.Count > 0 && itemsToRemove_Dict.TryGetValue(kvPair.Key, out List<(GameObject itemToRemove, int amountToRemove)> innerList) && innerList.Count != 0)
            {
                for (int i = 0; i < innerList.Count; i++)
                {
                    Inventory.Instance.RemoveFromInventory(innerList[i].itemToRemove, innerList[i].amountToRemove);
                }
            }
        }

        for (int i = 0; i < productRecipeIN.recipeSpecs.requiredIngredients.Length; i++)
        {
            ResourcesManager.Instance.RemoveIngredient(productRecipeIN.recipeSpecs.requiredIngredients[i].ingredient, productRecipeIN.GetRequiredIngredients(i, out _, out _));
        }

        return true;
    }

    private void ClearItemsToRemoveDict()
    {
        foreach (var kvPair in itemsToRemove_Dict)
        {
            if (itemsToRemove_Dict.TryGetValue(kvPair.Key, out List<(GameObject itemToRemove, int amountToRemove)> innerList) && innerList.Count != 0)
            {
                innerList.Clear();
            }

        }
    }

    private void MapFlaggedItemsBeforeConsume(List<GameObject> candidateGameItemsList_IN, int requiredAmoutOfItems_IN)
    {
        candidateGameItemsList_IN?.SortByQuality();
        Debug.Log(candidateGameItemsList_IN[candidateGameItemsList_IN.Count - 1].GetName() + " of quality " + (candidateGameItemsList_IN[candidateGameItemsList_IN.Count - 1] as IQualitative).GetQuality() + "existing amount is : " + candidateGameItemsList_IN[candidateGameItemsList_IN.Count - 1].GetAmount() + " required amount is :" + requiredAmoutOfItems_IN);

        if (candidateGameItemsList_IN[candidateGameItemsList_IN.Count - 1].GetAmount() >= requiredAmoutOfItems_IN)
        {
            if (candidateGameItemsList_IN[candidateGameItemsList_IN.Count - 1] is IQualitative qualitativeItem && qualitativeItem.GetQuality() == Quality.Level.Normal)
            {
                bool isEnhanced = false;
                if (candidateGameItemsList_IN[candidateGameItemsList_IN.Count - 1] is IEnhanceable enhanceableItem)
                {
                    isEnhanced = enhanceableItem.isEnhanced;
                }

                itemsToRemove_Dict[isEnhanced].Add((candidateGameItemsList_IN[candidateGameItemsList_IN.Count - 1], requiredAmoutOfItems_IN));
                return;
            }
            else
            {
                itemsToRemove_Dict[true].Add((candidateGameItemsList_IN[candidateGameItemsList_IN.Count - 1], requiredAmoutOfItems_IN));
                return;
            }
        }
        else
        {     
            var remainingRequiredAmountOfItems = requiredAmoutOfItems_IN;
            for (int i = candidateGameItemsList_IN.Count - 1; i > -1; i--)
            {
                var sequentialRemoveAmount = candidateGameItemsList_IN[i].GetAmount() >= remainingRequiredAmountOfItems ? remainingRequiredAmountOfItems : candidateGameItemsList_IN[i].GetAmount();
                if (candidateGameItemsList_IN[i] is IQualitative qualitativeItem && qualitativeItem.GetQuality() == Quality.Level.Normal)
                {
                    bool isEnhanced = false;
                    if (candidateGameItemsList_IN[i] is IEnhanceable enhanceableItem)
                    {
                        isEnhanced = enhanceableItem.isEnhanced;
                    }

                    itemsToRemove_Dict[isEnhanced].Add((candidateGameItemsList_IN[i], sequentialRemoveAmount));
                    remainingRequiredAmountOfItems -= sequentialRemoveAmount;
                }
                else
                {
                    itemsToRemove_Dict[true].Add((candidateGameItemsList_IN[i], sequentialRemoveAmount));
                    remainingRequiredAmountOfItems -= sequentialRemoveAmount;
                }

                if (remainingRequiredAmountOfItems <= 0) break;
            }
            return;
        }
    }


    private bool IsEnoughItemHolders()
    {
        if (maxCraftSlotsForLevel <= activeCraftAmount)
        {
            Debug.Log("not enough slots buy more");
            return false;
        }
        else
        {
            return true;
        }
    }

    private Single_CraftedItem SelectItemHolder()
    {
        for (int i = 0; i < craftedItems.Length; i++)
        {
            if (craftedItems[i].isOccupied == false)
            {
                return craftedItems[i];
            }
        }
        Debug.Log("all slots are occupied");
        Radial_CraftSlots_Controller.Instance.SetRotation(SpinType.Type.BackSpin);
        return null;
    }


    public void ReclaimCrafted(Single_CraftedItem craftedItem)
    {
        Debug.Log("stimulated " + " is ready ? :" + craftedItem.IsReadyToReclaim);
        if (!craftedItem.IsReadyToReclaim)
        {
            Radial_CraftSlots_Controller.Instance.SetRotation(SpinType.Type.BackSpin);

            var panelToLoad = PanelManager.InvokablePanels[typeof(ProgressPopupPanel)];
            var activeCrafts = craftSlots.Where(cs => cs.containedItem != null).Select(cs => cs.containedItem);
            //var activeCrafts = craftedItems.Where(ci => ci.isOccupied); //&& index < activeCraftAmount);
            var clickedObjectIndex = activeCrafts.Select((ac, i) => (ac, i)).Where(aci => aci.ac.Equals(craftedItem)).Select(aci => aci.i).DefaultIfEmpty(0).FirstOrDefault();
                //.Select<Single_CraftedItem,(SortableBluePrint bluePrint,Action<float,float> onCraftTicked ,Func<float> currentProgress)>(ac => (ac.productRecipe,ac.OnCraftTicked,() => ac.ProgressImage.fillAmount));
            ProgressPanelLoadData panelLoadData = new (mainLoadInfo: null, panelHeader: "Current Crafts",tcs_IN:null,
                                                                           rushableItemsData: activeCrafts, clickedObjectIndex: clickedObjectIndex);

            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad, panelLoadAction_IN:
                                          () => ProgressPopupPanel.Instance.LoadPanel(panelLoadData));

            /*foreach (var activeCraft in activeCrafts)
            {
                Debug.Log(activeCraft.currentProgress() + "  " + activeCraft.bluePrint.GetName());
            } 

            Debug.Log($"item is not ready , need {craftedItem.productRecipe.recipeSpecs.SpeedUpEnergy} energy");*/
            return;
        }
        else
        {
            do
            {
                switch (craftedItem.craftedAmount)
                {
                    case null:
                        if (craftedItem.productRecipe.GetMultiCraftChanceModifier() <= 0)
                        {
                            craftedItem.craftedAmount = 1;
                            RemoveItemHolderFromCraftSlot(craftedItem);
                            return;
                        }
                        else
                        {
                            craftedItem.craftedAmount = MultiCraft.IsMultiCraft(craftedItem.productRecipe.GetMultiCraftChanceModifier()) ? 2 : 1;
                            craftedItem.SetFGColor();
                            continue;
                        }
                    case 1:
                    case 2:
                        RemoveItemHolderFromCraftSlot(craftedItem);
                        return;
                }
            }
            while (craftedItem.craftedAmount == 1);

        }
    }

    private void PlaceItemHolderToCraftSlot(Single_CraftedItem itemHolderToPlace, ProductRecipe productRecipeIN)
    {
        for (int i = 0; i < craftSlots.Length; i++)
        {
            if (craftSlots[i].containedItem == null)
            {
                craftSlots[i].DropItem(itemHolderToPlace);
                craftSlots[i].containedItem.StartCrafting(productRecipeIN);
                activeCraftAmount++;

                onStartCrafting?.Invoke(this, new OnCraftingEventArgs { activeCraftAmount = activeCraftAmount, remainingCraftAmount = maxCraftSlotsForLevel - activeCraftAmount, productRecipe = productRecipeIN, itemHolderSlot = itemHolderToPlace });

                if (craftSlots[i].containedItem.isVisible == false)
                {
                    Radial_CraftSlots_Controller.Instance.SetRotationTargeted(SpinType.Type.TargetedSpin, craftSlots[i]);
                    return;
                }
                else if (craftSlots_Parent.localEulerAngles.z % 30f != 0f)
                {
                    Radial_CraftSlots_Controller.Instance.SetRotation(SpinType.Type.BackSpin);
                    return;
                }

                return;
            }
        }
    }

    private void RemoveItemHolderFromCraftSlot(Single_CraftedItem itemHolderToRemove)
    {
        if (Inventory.HasFreeSlots((int)itemHolderToRemove.craftedAmount))
        {
            for (int i = 0; i < craftSlots.Length; i++)
            {
                if (craftSlots[i].containedItem == itemHolderToRemove)
                {
                    activeCraftAmount--;
                    onReclaimCrafted?.Invoke(this, new OnCraftingEventArgs { activeCraftAmount = activeCraftAmount, 
                                                                             remainingCraftAmount = maxCraftSlotsForLevel - activeCraftAmount, 
                                                                             productRecipe = itemHolderToRemove.productRecipe, 
                                                                             itemHolderSlot = itemHolderToRemove, 
                                                                             amountCrafted = (int)itemHolderToRemove.craftedAmount });
                    craftSlots[i].containedItem.ReclaimCrafted();
                    craftSlots[i].RemoveItem();

                    Radial_CraftSlots_Controller.Instance.RearrangeCraftSlots(slotNo: i, itemHolderToAwait: itemHolderToRemove);

                    itemHolderToRemove.transform.SetParent(craftedItems_Parent);
                    itemHolderToRemove.transform.SetAsLastSibling();

                    return;
                }
            }
        }
        else
        {
            Radial_CraftSlots_Controller.Instance.SetRotation(SpinType.Type.BackSpin);
            return;
        }
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static ModalPanel_DisplayBonuses_ProductRecipeUnlockOrResearch;

public class Inventory : MonoBehaviour
{

    #region Singleton Syntax

    private static Inventory _instance;
    public static Inventory Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion

    public static Dictionary<GameItemType.Type, List<GameObject>> InventoryIterationDict { get; private set; }
    public static Dictionary<GameObject, GameObject> InventoryLookupDict_ByItem => _inventoryLookupDict_ByItem;
    private static Dictionary<GameObject, GameObject> _inventoryLookupDict_ByItem = new Dictionary<GameObject, GameObject>();
    private static Dictionary<string, List<GameObject>> _inventoryLookupDict_ByName = new Dictionary<string, List<GameObject>>();


    public static int ExistingItemAmount { get; private set; } = 0;
    public static int InventoryCapacity { get; private set; } = 0;

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


    private void OnDisable()
    {
        Radial_CraftSlots_Crafter.Instance.onReclaimCrafted -= CreateNew_Product;
    }

    private void Start() // LATER TO TAKE UP! !!
    {
        Configure();
        //UpdateCapacityText();

        InventoryCapacity = 10;    //LATER TO TAKE FROM PLAYER LEVE OR LOAD !!
    }

    private void Update()                  // TEST PURPOSES LATER TO DELETE !!
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CreateNew_SpecialItem(SpecialItemType.Type.AscensionShard, 3);
            CreateNew_SpecialItem(SpecialItemType.Type.ResearchScroll, 2);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            for (int i = 0; i < InventoryIterationDict[GameItemType.Type.Product].Count; i++)
            {
                Debug.Log(i + " index no is :: " + InventoryIterationDict[GameItemType.Type.Product][i].GetName() + " of : " + (InventoryIterationDict[GameItemType.Type.Product][i] as IQualitative).GetQuality());
            }
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Iron_Pine_Cone, 2);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Precious_Gem, 5);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Rustwyrm_Scale, 5);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Sigil_Of_Spark, 5);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Silver_Dust, 5);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Webbed_Wing, 5);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Glow_Shroom, 5);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Living_Root, 5);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Deep_Pearl, 5);
            CreateNew_ExtraComponent(ExtraComponentsType.Type.Elven_Wood, 2);
            CreateNew_SpecialItem(SpecialItemType.Type.Chest, 2, ChestType.Type.Wooden_Chest);
            CreateNew_SpecialItem(SpecialItemType.Type.Key, 1, ChestType.Type.Wooden_Chest);
        }

    }


    private void Configure(Dictionary<GameItemType.Type, List<GameObject>> loadedIterationDict = null)
    {
        InventoryIterationDict = loadedIterationDict ?? InventoryIterationDict.InitEmptyDict();
        LoadInventoryInternalDicts();
        Radial_CraftSlots_Crafter.Instance.onReclaimCrafted += CreateNew_Product;
    }

    private void LoadInventoryInternalDicts()
    {
        foreach (var pair in InventoryIterationDict)
        {
            for (int i = 0; i < pair.Value.Count; i++)
            {
                _inventoryLookupDict_ByItem.Add(pair.Value[i], pair.Value[i]);


                if (!_inventoryLookupDict_ByName.ContainsKey(pair.Value[i].GetName()))
                {
                    _inventoryLookupDict_ByName[pair.Value[i].GetName()] = new List<GameObject>();
                }
                _inventoryLookupDict_ByName[pair.Value[i].GetName()].Add(pair.Value[i]);
            }
        }
    }

    /*public void SetInventoryCapacity(int newInventoryCap)
    {
        InventoryCapacity = newInventoryCap;
    }*/
    public void SetInventoryCapacity()
    {
        InventoryCapacity = InventoryUpgrade.GetOverallInventoryCap();
    }

    public static bool HasFreeSlots(int amount)
    {
        if ((ExistingItemAmount + amount) <= InventoryCapacity)
        {
            return true;
        }
        else
        {
            Debug.Log("no places left in the inventory ");
            return false;
        }
    }

    private async void CreateNew_Product(object sender, Radial_CraftSlots_Crafter.OnCraftingEventArgs e)
    {
        var generatedGameItemsAboveNormal = new List<GameObject>(e.amountCrafted);
        //GameItem newGameItem;

        for (int i = 0; i < e.amountCrafted; i++)
        {
            var calculatedQuality = Quality.CalculateQualityLevel(e.productRecipe.GetQualityModifers());
            var (gameitemType, newGameItem) = DecideTypeAndCreateGameItem(e.productRecipe, calculatedQuality);
            AddToInventory(gameitemType, newGameItem, 1);

            /*switch (e.productRecipe.recipeSpecs.productType)
            {
                case ProductType.Type.Runestone_Enhancement:
                case ProductType.Type.Elemental_Enhancement:
                case ProductType.Type.Spirit_Enhancement:
                    newGameItem = new Enhancement(productRecipe_IN: e.productRecipe,
                                                              qualityLevel_IN: calculatedQuality);
                    AddToInventory(GameItemType.Type.Enhancement, newGameItem, 1);
                    break;

                default:
                    newGameItem = new Product(productRecipe_IN: e.productRecipe,
                                                      qualityLevel_IN: calculatedQuality);
                    AddToInventory(GameItemType.Type.Product, newGameItem, 1);
                    break;
            }*/

            //var newGameItemQuality = ((IQualitative)newGameItem).GetQuality();
            if (calculatedQuality != Quality.Level.Normal)// && Enum.IsDefined(typeof(Quality.Level), (int)newGameItemQuality + 1))
            {
                generatedGameItemsAboveNormal.Add(newGameItem);
                Debug.LogWarning($"above normal item created, name : {newGameItem.GetName()}, quality : {calculatedQuality}");
            }

        }

        foreach (var generatedGameItem in generatedGameItemsAboveNormal)
        {
            TaskCompletionSource<bool> _tcs = new();

            var panelToLoad = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];
            var modalLoadData = new ModalPanel_GameItemRarityUpgrade(modalState_IN: Information_Modal_Panel.ModalState.GameItemRarityUpgrade,
                                                                     gameItem_IN: generatedGameItem,
                                                                     tcs_IN: _tcs);
            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad,
                                         panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                                  modalLoadDatas: Enumerable.Empty<ModalLoadData>()),
                                         alternativeLoadAction_IN: () => ((Information_Modal_Panel)panelToLoad.MainPanel).ModalLoadDataQueue.Enqueue(modalLoadData));

            await _tcs.Task;

            if (_tcs.Task.Result == true)
            {
                
                Quality.TryGetNextQualityLevel(((IQualitative)generatedGameItem).GetQuality(), out Quality.Level? newQualityLevel);
                var (gameitemType, newGameItem) = DecideTypeAndCreateGameItem(productRecipe: ((ICraftable)generatedGameItem).GetProductRecipe(), qualityLevel: newQualityLevel.Value);
                RemoveFromInventory(generatedGameItem, 1);
                AddToInventory(gameitemType, newGameItem, 1);

                var informationModalPanel = (Information_Modal_Panel)panelToLoad.MainPanel;
                switch (newGameItem)
                {
                    case Product newProduct:
                        informationModalPanel.AcceptRarityUpgrade(newProduct, generatedGameItem as Product);
                        break;
                    case Enhancement newEnhancement:
                        informationModalPanel.AcceptRarityUpgrade(newEnhancement, generatedGameItem as Enhancement);
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
                Debug.Log($"gameitem is changed to {newQualityLevel}");
            }
        }
    }

    private (GameItemType.Type gameitemType, GameObject newGameItem) DecideTypeAndCreateGameItem(ProductRecipe productRecipe, Quality.Level qualityLevel)
        => productRecipe.recipeSpecs.productType switch
        {
            ProductType.Type.Spirit_Enhancement or ProductType.Type.Elemental_Enhancement or ProductType.Type.Runestone_Enhancement  => (GameItemType.Type.Enhancement, new Enhancement(productRecipe_IN: productRecipe, qualityLevel_IN: qualityLevel)),
            _ => (GameItemType.Type.Product, new Product(productRecipe_IN: productRecipe, qualityLevel_IN: qualityLevel)),
        };

    public Product CreateNew_EnhancedProduct(Product product, Enhancement enhancement, bool isEnhancementAdded, int amount = 1)
    {
        Product newProduct = new Product(product, enhancement, isEnhancementAdded);
        AddToInventory(GameItemType.Type.Product, newProduct, amount);

        /// in case the selecteditem should be changed (i.e. returning to iteminfo FROM ebhancement)
        /// To be able to catch the item in the inventory and not the new item which will not be used if itemtype alrady exists, 
        /// then we just raise amount in inventory
        InventoryLookupDict_ByItem.TryGetValue(newProduct, out GameObject enhanceable);

        if (GameItemInfoPanel_Manager.Instance.SelectedRecipe is not null) GameItemInfoPanel_Manager.Instance.BrowseInfo(enhanceable);
        if (EnhanceItemPopupPanel.Instance.bluePrint is not null) EnhanceItemPopupPanel.Instance.UpdateBluePrint(enhanceable);

        return newProduct;
    }

    public void CreateNew_SpecialItem(SpecialItemType.Type specialItem_IN, int amount, ChestType.Type chestType_IN = ChestType.Type.None)
    {
        SpecialItem newItem = null;
        if (chestType_IN != ChestType.Type.None)
        {
            if (specialItem_IN == SpecialItemType.Type.Chest)
            {
                newItem = new Chest(specialItem_IN, chestType_IN);
            }
            else if (specialItem_IN == SpecialItemType.Type.Key)
            {
                newItem = new Key(specialItem_IN, chestType_IN);
            }
        }
        else if (specialItem_IN == SpecialItemType.Type.AscensionShard)
        {
            newItem = new AscensionShard(specialItem_IN);
        }
        else if (specialItem_IN == SpecialItemType.Type.ResearchScroll)
        {
            newItem = new ResearchScroll(specialItem_IN);
        }


        if (newItem != null)
        {
            AddToInventory(GameItemType.Type.SpecialItem, newItem, amount);
        }
    }

    public void CreateNew_ExtraComponent(ExtraComponentsType.Type extraComponentType_IN, int amount)
    {
        //int normalizedEnumIndex = (int)extraComponentType_IN - ExtraComponentsType.minUnderlyingValue;
        ExtraComponent newExtraComponent = new ExtraComponent(ExtraComponentsType.GetNormalizedEnumIndex(extraComponentType_IN));

        AddToInventory(GameItemType.Type.ExtraComponents, newExtraComponent, amount);
    }


    private void AddToInventory(GameItemType.Type itemType, GameObject newItem, int amount)
    {

        if (_inventoryLookupDict_ByItem.TryGetValue(newItem, out GameObject existingItem))
        {
            Debug.LogWarning("Item Existing Augmenting Value Of : " + newItem.GetName() + " typeof : " + itemType + " amount : " + amount);
            existingItem.SetAmount(amount);
        }
        else
        {
            Debug.LogWarning("New Item Being Created : " + newItem.GetName() + " typeof : " + itemType + " amount : " + amount + "of quality:" + (newItem as IQualitative)?.GetQuality());
            _inventoryLookupDict_ByItem.Add(newItem, newItem);

            if (!_inventoryLookupDict_ByName.ContainsKey(newItem.GetName()))
            {
                _inventoryLookupDict_ByName[newItem.GetName()] = new List<GameObject>();
            }
            _inventoryLookupDict_ByName[newItem.GetName()].Add(newItem);

            InventoryIterationDict[itemType].Add(newItem);
            newItem.SetAmount(amount);
        }

        if (newItem is ICraftable)
        {
            ExistingItemAmount += amount;
            GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.inventoryCapacity, initialValue_IN: ExistingItemAmount, maxValue_IN: InventoryCapacity);

        }
        if (newItem is ResearchScroll)
        {
            GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.researchPoint, CheckAmountInInventory(newItem));
        }

    }

    public bool RemoveFromInventory(GameObject gameItem_IN, int amount)     // later to make with params !! its much better // and whne removeing on crafter simply hcheck the array
    {
        if (_inventoryLookupDict_ByItem.TryGetValue(gameItem_IN, out GameObject existingItem))
        {
            if (existingItem.GetAmount() >= amount)
            {
                existingItem.SetAmount(amount * -1);

                if (gameItem_IN is ExtraComponent removedExtracomponent)
                {
                    additionalItemsEventMapping[removedExtracomponent.ExtraComponentType]?.Invoke();
                }

                if (gameItem_IN is ICraftable)
                {
                    ExistingItemAmount -= amount;
                    GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.inventoryCapacity, initialValue_IN: ExistingItemAmount, maxValue_IN: InventoryCapacity);

                    if (gameItem_IN is Product || gameItem_IN is Enhancement)
                    {
                        onProductRemoved?.Invoke(gameItem_IN.GetName());
                    }

                }
                if (gameItem_IN is ResearchScroll)
                {
                    GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.researchPoint, CheckAmountInInventory(gameItem_IN));
                }

                if (existingItem.GetAmount() == 0)
                {
                    _inventoryLookupDict_ByItem.Remove(gameItem_IN);
                    _inventoryLookupDict_ByName[existingItem.GetName()].Remove(existingItem);

                    if (gameItem_IN is Product existingProduct)
                    {
                        InventoryIterationDict[GameItemType.Type.Product].Remove(existingProduct);
                    }
                    else if (gameItem_IN is ExtraComponent existingExtraComponent)
                    {
                        InventoryIterationDict[GameItemType.Type.ExtraComponents].Remove(existingExtraComponent);
                    }
                    else if (gameItem_IN is SpecialItem existingSpecialItem)
                    {
                        InventoryIterationDict[GameItemType.Type.SpecialItem].Remove(existingSpecialItem);
                    }
                    else if (gameItem_IN is Enhancement existingEnhancement)
                    {
                        InventoryIterationDict[GameItemType.Type.Enhancement].Remove(existingEnhancement);
                    }
                }

                return true;
            }
        }

        return false;
    }


    public int CheckAmountInInventory(GameObject gameItem_IN)
    {
        if (_inventoryLookupDict_ByItem.TryGetValue(gameItem_IN, out GameObject existingItem))
        {
            return existingItem.GetAmount();
        }
        return 0;
    }

    public int CheckAmountInInventory(ExtraComponentsType.Type extraComponentType_IN, out ExtraComponent extraComponentToRemove)
    {
        int amount = 0;
        for (int i = 0; i < InventoryIterationDict[GameItemType.Type.ExtraComponents].Count; i++)
        {
            var existingExtraComponent = InventoryIterationDict[GameItemType.Type.ExtraComponents][i] as ExtraComponent;
            if (existingExtraComponent.ExtraComponentType == extraComponentType_IN)
            {
                amount += existingExtraComponent.GetAmount();
                extraComponentToRemove = existingExtraComponent;
                return amount;
            }
        }
        extraComponentToRemove = null;
        return amount;
    }

    public int CheckAmountInInventory(SpecialItemType.Type specialItemType_IN)
    {
        int amount = 0;
        for (int i = 0; i < InventoryIterationDict[GameItemType.Type.SpecialItem].Count; i++)
        {
            var existingSpecialItem = InventoryIterationDict[GameItemType.Type.SpecialItem][i] as SpecialItem;
            if (existingSpecialItem.GetSpecialItemType() == specialItemType_IN)
            {
                amount += existingSpecialItem.GetAmount();
            }
        }
        return amount;
    }



    public int CheckAmountInInventory_Name(string itemName, GameItemType.Type gameItemType)   // can be made another case for just the products and the rest can break the loop for performance !!! 
    {
        int amount = 0;
        foreach (GameObject itemExisting in InventoryIterationDict[gameItemType])
        {
            if (itemExisting.GetName().Equals(itemName, StringComparison.OrdinalIgnoreCase))        // before was with == and not equals if it works fine change the above as well 
            {
                amount += itemExisting.GetAmount();
            }
        }
        return amount;
    }

    public int CheckAmountInInventory_ByNameDict(string productName, out List<GameObject> gameItemsToRemove)
    {
        int amount = 0;
        gameItemsToRemove = null;
        if (_inventoryLookupDict_ByName.TryGetValue(productName, out List<GameObject> innerList))
        {
            gameItemsToRemove = innerList;
            foreach (GameObject itemExisting in innerList)
            {
                amount += itemExisting.GetAmount();
                // gameItemsToRemove.Add(itemExisting);
            }
            //if (innerList.Count > 0) gameItemsToRemove = innerList[0];
            //gameItemToRemove = innerList.Count > 0 ? innerList[0] : null;
        }
        return amount;
    }


    public int CheckAmountInInventory_Product(ProductType.Type productType, string itemName, out Product productToRemove)
    {
        int amount = 0;
        var candidateProducts = new List<Product>();
        for (int i = 0; i < InventoryIterationDict[GameItemType.Type.Product].Count; i++)
        {
            var itemExisting = InventoryIterationDict[GameItemType.Type.Product][i] as Product;
            if (itemExisting.GetName().Equals(itemName, StringComparison.OrdinalIgnoreCase))        // before was with == and not equals if it works fine change the above as well 
            {
                var itemLocalAmount = itemExisting.GetAmount();
                amount += itemLocalAmount;
                if (itemLocalAmount > 0)
                {
                    candidateProducts.Add(itemExisting);
                }
            }
        }
        productToRemove = candidateProducts.Count > 0 ? candidateProducts[0] : null;
        return amount;
    }

    public int CheckAmountInInventory_SubType<T_SubType>(T_SubType productSubType, GameItemType.Type gameItemType)   // can be made another case for just the products and the rest can break the loop for performance !!! 
     where T_SubType : System.Enum
    {
        int amount = 0;
        if (gameItemType == GameItemType.Type.Product)
        {
            foreach (Product itemExisting in InventoryIterationDict[gameItemType])
            {
                if (itemExisting.GetProductRecipe().recipeSpecs.productType.Equals(productSubType))
                {
                    amount += itemExisting.GetAmount();
                }
            }
        }
        else if (gameItemType == GameItemType.Type.Enhancement)
        {
            foreach (Enhancement itemExisting in InventoryIterationDict[gameItemType])
            {
                if (itemExisting.GetEnhancementType().Equals(productSubType))
                {
                    amount += itemExisting.GetAmount();
                }
            }
        }
        return amount;
    }

    public static event Action onIronPineConeRemoved;
    public static event Action onElvenWoodRemoved;
    public static event Action onGlowShroomRemoved;
    public static event Action onSilverDustRemoved;
    public static event Action onWebbedWingRemoved;
    public static event Action onPreciousGemRemoved;
    public static event Action onLivingRootRemoved;
    public static event Action onRustwyrmscaleRemoved;
    public static event Action onDeepPearlRemoved;
    public static event Action onSigilOfSparkRemoved;

    public static event Action<string> onProductRemoved;



    public static readonly Dictionary<ExtraComponentsType.Type, Action> additionalItemsEventMapping = new Dictionary<ExtraComponentsType.Type, Action>
    {
        [ExtraComponentsType.Type.Iron_Pine_Cone] = onIronPineConeRemoved,
        [ExtraComponentsType.Type.Elven_Wood] = onElvenWoodRemoved,
        [ExtraComponentsType.Type.Glow_Shroom] = onGlowShroomRemoved,
        [ExtraComponentsType.Type.Silver_Dust] = onSilverDustRemoved,
        [ExtraComponentsType.Type.Webbed_Wing] = onWebbedWingRemoved,
        [ExtraComponentsType.Type.Precious_Gem] = onPreciousGemRemoved,
        [ExtraComponentsType.Type.Living_Root] = onLivingRootRemoved,
        [ExtraComponentsType.Type.Rustwyrm_Scale] = onRustwyrmscaleRemoved,
        [ExtraComponentsType.Type.Deep_Pearl] = onDeepPearlRemoved,
        [ExtraComponentsType.Type.Sigil_Of_Spark] = onSigilOfSparkRemoved,
    };


}

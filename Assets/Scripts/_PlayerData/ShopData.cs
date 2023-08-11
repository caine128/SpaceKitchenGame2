using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopData : MonoBehaviour
{

    #region Singleton Syntax

    private static ShopData _instance;
    public static ShopData Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion


    public static Dictionary<ShopUpgradeType.Type, List<ShopUpgrade>> ShopUpgradesIteration_Dict { get; private set; }
    //private static Dictionary<string, ShopUpgrade> shopUpgrades_LookupDict = new Dictionary<string, ShopUpgrade>();
    //private static HashSet<ShopUpgrade> shopupgrades_Hashset_ByItem = new HashSet<ShopUpgrade>(new ShopUpgradeEqualityComparer_ByItem());
    //private static Dictionary<ShopUpgrade, List<ShopUpgrade>> shopupgradesLookupDict_ByName = new Dictionary<ShopUpgrade, List<ShopUpgrade>>(new ShopUpgradeEqualityComparer_ByName());

    public static int ShopUpgradesAmount { get; private set; } = 0;
    public static int ShopCapacityMax { get; private set; } = 0;


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

    private void Start()              // LATER TO TAKE UP! !!
    {
        Configure();
        //LoadAdditionalDicts();
        ShopCapacityMax = 10;    //TODO : LATER TO TAKE FROM PLAYER LEVE OR LOAD !!
    }

    private void Update()           // LATER TO DELETE MADE FOR TESTING PURPOSE
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach (var pair in ShopUpgradesIteration_Dict)
            {
                foreach (var value in pair.Value)
                {
                    Debug.Log(pair.Key + " " + value.GetName());
                }
            }
        }
    }


    private void Configure(Dictionary<ShopUpgradeType.Type, List<ShopUpgrade>> loadedDict = null)
    {
        ShopUpgradesIteration_Dict = loadedDict ?? ShopUpgradesIteration_Dict.InitEmptyDict();
    }

    //public bool CheckShopUpgradeExists(string upgradeName, out ShopUpgrade shopUpgrade)
    //{
    //}

    /*private void LoadAdditionalDicts()
    {
        foreach (var pair in ShopUpgradesIteration_Dict)
        {
            for (int i = 0; i < pair.Value.Count; i++)
            {
                //shopupgrades_Hashset.Add(pair.Value[i].GetName(), pair.Value[i]);
                shopupgrades_Hashset_ByItem.Add(pair.Value[i]);

                if (!shopupgradesLookupDict_ByName.ContainsKey(pair.Value[i]))
                {
                    shopupgradesLookupDict_ByName[pair.Value[i]] = new List<ShopUpgrade>();
                }

                shopupgradesLookupDict_ByName[pair.Value[i]].Add(pair.Value[i]);
            }
        }
    }*/



    public bool TryPurchaseShopUpgrade(ShopUpgrade newUpgrade_IN, ISpendable spendable)
    {
        if (!StatsData.IsSpendableAmountEnough(newUpgrade_IN.GetValue(), spendable))
        {
            Debug.Log("not enough " + spendable.GetType().ToString());
            return false;
        }
        else
        {
            StatsData.SetSpendableValue(spendable, -newUpgrade_IN.GetValue());
            var clonedShopUpgrade = newUpgrade_IN.ClonePurchase();
            AddToShop(clonedShopUpgrade as ShopUpgrade);
            return true;
        }
    }


    private void AddToShop(ShopUpgrade shopUpgrade_IN)
    {
        if (ShopUpgradesAmount >= ShopCapacityMax)
        {
            Debug.Log("there is not remaining place in your shop Please expand your shop");
            return;
        }

        else
        {
            ShopUpgradesAmount++;
            GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.shopCapacity, initialValue_IN: ShopUpgradesAmount, maxValue_IN: ShopCapacityMax);

            if (!ShopUpgradesIteration_Dict.ContainsKey(shopUpgrade_IN.shopUpgradeType))
            {
                ShopUpgradesIteration_Dict[shopUpgrade_IN.shopUpgradeType] = new List<ShopUpgrade>();
            }

            ShopUpgradesIteration_Dict[shopUpgrade_IN.shopUpgradeType].Add(shopUpgrade_IN);

            /*
            if (shopupgrades_Hashset_ByItem.TryGetValue(shopUpgrade_IN, out ShopUpgrade shopUpgrade_Existing))
            {
                shopUpgrade_Existing.SetAmount(amountToAdd: 1);
                Debug.Log("Upgrade Existing Augmenting Value Of : " + shopUpgrade_Existing.GetName());
            }
            else
            {
                Debug.Log("New Upgrade Being Created : " + shopUpgrade_IN.GetName());
                if (!ShopUpgradesIteration_Dict.ContainsKey(shopUpgrade_IN.shopUpgradeType))
                {
                    ShopUpgradesIteration_Dict[shopUpgrade_IN.shopUpgradeType] = new List<ShopUpgrade>();
                }
                if (!shopupgradesLookupDict_ByName.ContainsKey(shopUpgrade_IN))
                {
                    shopupgradesLookupDict_ByName[shopUpgrade_IN] = new List<ShopUpgrade>();
                }


                ShopUpgradesIteration_Dict[shopUpgrade_IN.shopUpgradeType].Add(shopUpgrade_IN);
                //shopupgradesLookupDict_ByName[shopUpgrade_IN].Add(shopUpgrade_IN);
                shopupgrades_Hashset_ByItem.Add(shopUpgrade_IN);


                shopUpgrade_IN.SetAmount(amountToAdd: 1);
            }*/

            switch (shopUpgrade_IN)
            {
                case ResourceCabinetUpgrade resourceCabinetUpgrade:
                    var relevantIngredientType = resourceCabinetUpgrade.GetRelevantIngredientType();
                    ResourcesManager.CheckAmountOfIngredient(relevantIngredientType, out Ingredient ingredient);
                    /*int currentMaxCap = 0;
                    foreach (var item in shopupgradesLookupDict_ByName[resourceCabinetUpgrade])
                    {
                        currentMaxCap += ((ResourceCabinetUpgrade)item).GetOverallStorageCap();
                    }
                    ingredient.SetMaxCap(currentMaxCap);*/
                    ingredient.SetMaxCap(ResourceCabinetUpgrade.GetOverallStorageCap(relevantIngredientType));
                    break;

                case InventoryUpgrade inventoryUpgrade:
                    /*int newInventoryCapacity = 0;
                    foreach (var item in shopupgradesLookupDict_ByName[inventoryUpgrade])
                    {
                        newInventoryCapacity += ((InventoryUpgrade)item).GetOverallInventoryCap();
                    }*/
                    Inventory.Instance.SetInventoryCapacity();
                    break;
                default:
                    break;
            }


            /*foreach (var item in shopupgrades_Hashset_ByItem)
            {
                Debug.Log("there exists : " + item.GetName() + " of amount : " + item.GetAmount());
            }*/
        }

    }
    /*
    public void TryAddNewShopUpgrade(ShopUpgrade newUpgrade_IN, ISpendable spendable)
    {
        if (!StatsData.IsSpendableAmountEnough(newUpgrade_IN.GetValue(), spendable))
        {
            Debug.Log("not enough " + spendable.GetType().ToString());
            return;
        }

        else
        {
            StatsData.SetSpendableValue(spendable, amountDelta: (newUpgrade_IN.GetValue() * -1));
        }

        ShopUpgradesIteration_Dict.AddNewOrAugmentExisting(newUpgrade_IN.shopUpgradeType, newUpgrade_IN, 1);
        shopupgrades_Hashset.Add(newUpgrade_IN);
        switch (newUpgrade_IN)
        {
            case ResourceCabinetUpgrade resourceCabinetUpgrade:
                ResourcesManager.CheckAmountOfIngredient(resourceCabinetUpgrade.GetRelevantIngredientType(), out Ingredient ingredient);
                ingredient.SetMaxCap(resourceCabinetUpgrade.GetStorageCap());
                break;
            default:
                break;

        }

        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), panelLoadAction_IN: null, sender: this);

        //if (!spendable.IsAmountEnough(newUpgrade_IN.GetValue()))
        //{
        //    Debug.Log("not enough " + spendable.GetType().ToString());
        //    return;
        //}


        
        if(StatsData.Gold.Amount < newUpgrade_IN.GetValue())
        {
            Debug.Log("not enough gold");
            return;
        }
        else
        {
            StatsData.SetGold(newUpgrade_IN.GetValue() * -1);
            ShopUpgradesIteration_Dict.AddNewOrAugmentExisting(newUpgrade_IN.shopUpgradeType, newUpgrade_IN, 1);

            switch (newUpgrade_IN)
            {
                case ResourceCabinetUpgrade resourceCabinetUpgrade:
                    ResourcesManager.CheckAmountOfIngredient(resourceCabinetUpgrade.GetRelevantIngredientType(), out Ingredient ingredient);
                    ingredient.SetMaxCap(resourceCabinetUpgrade.GetStorageCap());
                    break;
                default:
                    break;

            }
              
            PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), panelLoadAction_IN: null, sender: this);
        }
        
    }*/

    public static bool CheckPresenceOfUpgrade<T_ShopupgradeType>(T_ShopupgradeType shopUpgrade_IN, out IEnumerable<T_ShopupgradeType> shopUpgrade_OUT)
      where T_ShopupgradeType : ShopUpgrade
    {
        IEnumerable<T_ShopupgradeType> presentShopUpgrades = ShopUpgradesIteration_Dict.TryGetValue(shopUpgrade_IN.shopUpgradeType, out List<ShopUpgrade> workStationUpgrades)
                                        ? workStationUpgrades.Where(su => su.GetName() == shopUpgrade_IN.GetName()).Cast<T_ShopupgradeType>()
                                        : Enumerable.Empty<T_ShopupgradeType>();

        if (presentShopUpgrades.Any())
        {
#if UNITY_EDITOR
            if (shopUpgrade_IN is WorkStationUpgrade && presentShopUpgrades.Count() > 1)
                Debug.LogError("workstation upgrades shouldnt be more than one !");
#endif
            shopUpgrade_OUT = presentShopUpgrades;
            return true;
        }

        /*switch (shopUpgrade_IN)
        {
            case WorkStationUpgrade workStationUpgrade_IN: // In case of Workstation Upgrade Need To Look With Name Becase Levels Prevent Matching of UI Workstation Cards and Workstation Objects

                presentShopUpgrades = ShopUpgradesIteration_Dict.TryGetValue(ShopUpgradeType.Type.WorkstationUpgrades, out List<ShopUpgrade> workStationUpgrades)
                                        ? workStationUpgrades.Cast<WorkStationUpgrade>().Where(wsu => wsu.GetWorkstationType() == workStationUpgrade_IN.GetWorkstationType()).Cast<T_ShopupgradeType>()
                                        : Enumerable.Empty<T_ShopupgradeType>();

                if (presentShopUpgrades.Any())
                {
#if UNITY_EDITOR
                    if (presentShopUpgrades.Count() > 1)
                        Debug.LogError("workstation upgrades shouldnt be more than one !");
#endif
                    shopUpgrade_OUT = presentShopUpgrades;
                    return true;
                }

                break;



            default:



                if (shopupgrades_Hashset_ByItem.TryGetValue(shopUpgrade_IN, out ShopUpgrade value))
                {
                    shopUpgrade_OUT = value;
                    return true;
                }

                break;
        }*/
        /*if (shopupgrades_Hashset_ByItem.TryGetValue(shopUpgrade_IN, out ShopUpgrade value))
        {
            shopUpgrade_OUT = value;
            return true;
        }*/
        shopUpgrade_OUT = null;
        return false;
    }

    /* public static bool CheckPresenceOfUpgrade(ShopUpgrade shopUpgrade_IN, out ShopUpgrade shopUpgrade_OUT)
     {
         switch (shopUpgrade_IN)
         {
             case WorkStationUpgrade: // In case of Workstation Upgrade Need To Look With Name Becase Levels Prevent Matching of UI Workstation Cards and Workstation Objects

                 if (shopupgradesLookupDict_ByName.TryGetValue(shopUpgrade_IN, out List<ShopUpgrade> shopUpgrades) && shopUpgrades.Count > 0)
                 {
                     if (shopUpgrades.Count > 1)
                         Debug.LogError("workstation upgrades shouldnt be more than one !");

                     shopUpgrade_OUT = shopUpgrades.First();
                     return true;
                 }

                 break;
             default:

                 if (shopupgrades_Hashset_ByItem.TryGetValue(shopUpgrade_IN, out ShopUpgrade value))
                 {
                     shopUpgrade_OUT = value;
                     return true;
                 }

                 break;
         }
         //if (shopupgrades_Hashset_ByItem.TryGetValue(shopUpgrade_IN, out ShopUpgrade value))
         //{
         //    shopUpgrade_OUT = value;
         //    return true;
         //}
         shopUpgrade_OUT = null;
         return false;
     }*/

    /*public static int GetTotalStorageCapOf(ResourceCabinetUpgrade resourceCabinetUpgrade)
                          => shopupgradesLookupDict_ByName[resourceCabinetUpgrade].Cast<ResourceCabinetUpgrade>()
                            .Where(rcu => rcu.GetRelevantIngredientType() == resourceCabinetUpgrade.GetRelevantIngredientType()) 
                            .Select(rcu => ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.tier[rcu.Tier].specsByLevel[rcu.GetLevel() - 1].storageBaseCap)
                            .Sum();*/

    public static IEnumerable<T_ShopupgradeType> GetOngoingUpgrades<T_ShopupgradeType>(T_ShopupgradeType shopUpgrade)
        where T_ShopupgradeType : ShopUpgrade
        => ShopUpgradesIteration_Dict[shopUpgrade.shopUpgradeType]
            .Where(su => ((IRushable)su).IsReadyToReclaim == true || ((IRushable)su).RemainingDuration > 0)
            .Cast<T_ShopupgradeType>();

    public static (IEnumerable<T_ShopupgradeType> ongoingUpgrades, int clickedObjectIndex) GetOngoingUpgradesWithClickedIndex<T_ShopupgradeType>(T_ShopupgradeType shopUpgrade)
         where T_ShopupgradeType : ShopUpgrade
        => (GetOngoingUpgrades(shopUpgrade), GetOngoingUpgrades(shopUpgrade)
                                                                          .Select((awu, i) => (awu, i))
                                                                          .Where(awui => awui.awu.Equals(shopUpgrade))
                                                                          .Select(awui => awui.i)
                                                                          .DefaultIfEmpty(0).FirstOrDefault());



}

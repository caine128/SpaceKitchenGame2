using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopData : MonoBehaviour   // TODO : make it generic singleton 
{

    #region Singleton Syntax

    private static ShopData _instance;
    public static ShopData Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion


    public static Dictionary<ShopUpgradeType.Type, List<ShopUpgrade>> ShopUpgradesIteration_Dict { get; private set; }
    
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
                    _instance = this; // TODO : LATER TO ADD DONTDESTROY ON LOAD
                }
            }
        }
    }

    private void Start()              // TODO : LATER TO TAKE UP! !!
    {
        Configure();
        ShopCapacityMax = 10;    //TODO : LATER TO TAKE FROM PLAYER LEVE OR LOAD !!
    }

    private void Update()           // TODO : LATER TO DELETE MADE FOR TESTING PURPOSE
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


    public ShopUpgrade CloneAndPurchaseShopUpgrade(ShopUpgrade shopUpgrade) //, ISpendable spendable)
    {
        /* var spendableRequired = clonedShopUpgrade.PurchaseCost();
        if (!StatsData.IsSpendableAmountEnough(spendableRequired.Amount, spendableRequired)) //    newUpgrade_IN.PurchaseCost    newUpgrade_IN.GetValue(), spendable))
        {
            Debug.Log("not enough " + spendableRequired.GetType().ToString());
            return false;
        }
        else
        {*/
        var spendableRequired = shopUpgrade.PurchaseCost();
        StatsData.SetSpendableValue(spendableRequired, -spendableRequired.Amount);
        
        var clonedShopUpgrade = shopUpgrade.ClonePurchase();

        AddToShop(clonedShopUpgrade); //clonedShopUpgrade as ShopUpgrade);
        return clonedShopUpgrade;
        //return true;
        //}
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

            switch (shopUpgrade_IN) // TODO : Might be necessary to augment the cases
            {
                case ResourceCabinetUpgrade resourceCabinetUpgrade:
                    var relevantIngredientType = resourceCabinetUpgrade.GetRelevantIngredientType();
                    ResourcesManager.CheckAmountOfIngredient(relevantIngredientType, out Ingredient ingredient);
                    
                    ingredient.SetMaxCap(ResourceCabinetUpgrade.GetOverallStorageCap(relevantIngredientType));
                    break;

                case InventoryUpgrade:
                 
                    Inventory.Instance.SetInventoryCapacity();
                    break;
                default:
                    break;
            }

        }

        ////////////////// TODO : ITs for test Later ot Remove this snipppet 
        foreach (var kvp in ShopUpgradesIteration_Dict)
        {
            foreach (var listItem in kvp.Value)
            {
                Debug.Log(listItem.GetName());
            }
        }
        /////////////////////////////////////////////////////////////////////
    }
    
    public void RemoveFromShop(ShopUpgrade shopUpgrade_IN)
    {
        ShopUpgradesAmount--;
        GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.shopCapacity, initialValue_IN: ShopUpgradesAmount, maxValue_IN: ShopCapacityMax);

        if (ShopUpgradesIteration_Dict.TryGetValue(shopUpgrade_IN.shopUpgradeType , out List<ShopUpgrade> shopUpgradesPurchased) 
            && shopUpgradesPurchased.Remove(shopUpgrade_IN))
        {
#if UNITY_EDITOR
            var matchingShopUpgradesPurchased = shopUpgradesPurchased.Where(sup => sup == shopUpgrade_IN);
            if (matchingShopUpgradesPurchased.Count() > 1)
                Debug.LogError("Shouldn't be able to find more than one matching shopupgrade");
#endif
           switch (shopUpgrade_IN) // TODO : Might be necessary to augment the cases same as the ADDToShop Method Switch case 
            {
                case ResourceCabinetUpgrade resourceCabinetUpgrade:
                    var relevantIngredientType = resourceCabinetUpgrade.GetRelevantIngredientType();
                    ResourcesManager.CheckAmountOfIngredient(relevantIngredientType, out Ingredient ingredient);

                    ingredient.SetMaxCap(ResourceCabinetUpgrade.GetOverallStorageCap(relevantIngredientType));
                    break;

                case InventoryUpgrade:

                    Inventory.Instance.SetInventoryCapacity();
                    break;
                default:
                    break;
            }
        }
    }


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

        shopUpgrade_OUT = null;
        return false;
    }

    
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

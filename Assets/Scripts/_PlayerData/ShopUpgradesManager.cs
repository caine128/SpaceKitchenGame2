using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUpgradesManager : MonoBehaviour
{
    private static ShopUpgradesManager _instance;
    public static ShopUpgradesManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    public ShopUpgrades_SO ShopUpgrades_SO { get => shopUpgrades_SO;}
    public InvestmentCosts_SO InvestmentCosts_SO { get => investmentCosts_SO; }

    [SerializeField] private ShopUpgrades_SO shopUpgrades_SO;
    [SerializeField] private InvestmentCosts_SO investmentCosts_SO;


    public static Dictionary<ShopUpgradeType.Type, List<ShopUpgrade>> shopUpgradesAvilable_Dict = new Dictionary<ShopUpgradeType.Type, List<ShopUpgrade>>();
    private static Dictionary<string,ShopUpgrade> shopUpgradesAvailable_IteratinoDict = new Dictionary<string, ShopUpgrade>();


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
                if (_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

        Config();
    }

    private void Config()             // TODO : LATER TO TAKE THIS TO ABOVE BUT PAY ATTENTION THAT IT SHOUD BE IN BEFORE START !!
    {
        shopUpgradesAvilable_Dict = shopUpgradesAvilable_Dict.InitEmptyDict();
        PopulateAvailableUpgradesDict();
        LoadInternalDicts();
    }


    private void PopulateAvailableUpgradesDict()
    {
        for (int i = 0; i < shopUpgrades_SO.foodDisplay_Upgrades.baseInfo.Length; i++)
        {
            FoodDisplayUpgrade newFoodDisplayUpgrade = new FoodDisplayUpgrade(i, ShopUpgradeType.Type.SalesCabinetUpgrade,ShopUpgradeType.SalesCabinetUpgradeType.FoodDisplayUpgrade);
            shopUpgradesAvilable_Dict[ShopUpgradeType.Type.SalesCabinetUpgrade].Add(newFoodDisplayUpgrade);
        }
        
        for (int i = 0; i < shopUpgrades_SO.inventory_Upgrades.baseInfo.Length; i++) //  Later to conitnue with the differnet upgrade types
        {
            InventoryUpgrade newInventoryUpgrade = new InventoryUpgrade(i,ShopUpgradeType.Type.SalesCabinetUpgrade, ShopUpgradeType.SalesCabinetUpgradeType.InventoryUpgrade);
            shopUpgradesAvilable_Dict[ShopUpgradeType.Type.SalesCabinetUpgrade].Add(newInventoryUpgrade);
        }
        
        for (int i = 0; i < shopUpgrades_SO.resourceCabinet_Upgrades.baseInfo.Length; i++)
        {
            ResourceCabinetUpgrade newResourceCabinetUpgrade = new ResourceCabinetUpgrade(i, ShopUpgradeType.Type.ResourceCabinetUpgrade,shopUpgrades_SO.resourceCabinet_Upgrades.baseInfo[i].tier);
            shopUpgradesAvilable_Dict[ShopUpgradeType.Type.ResourceCabinetUpgrade].Add(newResourceCabinetUpgrade);
        }

        for (int i = 0; i < shopUpgrades_SO.workstation_Upgrades.baseInfo.Length; i++)
        {        
            WorkStationUpgrade newWorkStationUpgrade = new WorkStationUpgrade(i, ShopUpgradeType.Type.WorkstationUpgrades);
            shopUpgradesAvilable_Dict[ShopUpgradeType.Type.WorkstationUpgrades].Add(newWorkStationUpgrade);
        }
    }

    private void LoadInternalDicts()
    {
        foreach (var pair in shopUpgradesAvilable_Dict)
        {
            for (int i = 0; i < pair.Value.Count; i++)
            {
                shopUpgradesAvailable_IteratinoDict.Add(pair.Value[i].GetName(), pair.Value[i]);
            }
        }
    }


    public ShopUpgrade GetRelevantResourceCabinet(SortableBluePrint bluePrint_IN)
        => bluePrint_IN switch
        {
            Ingredient ingredient => (ResourceCabinetUpgrade)shopUpgradesAvailable_IteratinoDict[ShopUpgrades_SO.resourceCabinet_Upgrades.baseInfo[(int)ingredient.IngredientType].name],
            WorkStationUpgrade workStationUpgrade => (WorkStationUpgrade)shopUpgradesAvailable_IteratinoDict[ShopUpgrades_SO.workstation_Upgrades.baseInfo.First(bi => bi.workstationType == workStationUpgrade.GetWorkstationType()).name],
            ResourceCabinetUpgrade resourceCabinetUpgrade => (ResourceCabinetUpgrade)shopUpgradesAvailable_IteratinoDict[ShopUpgrades_SO.resourceCabinet_Upgrades.baseInfo.First(bi => bi.name == resourceCabinetUpgrade.GetName()).name],
            _ => throw new Exception()
        };
}

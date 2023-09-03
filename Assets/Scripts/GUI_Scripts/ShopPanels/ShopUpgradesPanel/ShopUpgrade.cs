using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
//using static ShopUpgrades_SO;

public abstract class ShopUpgrade : SortableBluePrint, IAmountable, ILevellable, IRushable// IValuable, IRankable
{
    [SerializeField] public readonly ShopUpgradeType.Type shopUpgradeType;  // is this necessary ???
    protected readonly int indexNo;
    protected int currentLevel;
    public UnityEngine.GameObject GetPrefab => this switch
    {
        ResourceCabinetUpgrade resourceCabinetUpgrade => throw new System.NotImplementedException(),
        FoodDisplayUpgrade foodDisplayUpgrade => throw new System.NotImplementedException(),
        InventoryUpgrade inventoryUpgrade => throw new System.NotImplementedException(),
        WorkStationUpgrade workStationUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].workStationPrefabRef,
        _ => throw new System.NotImplementedException()
    };

    public (int x,int z) GetPropSize => this switch
    {
        ResourceCabinetUpgrade resourceCabinetUpgrade => throw new System.NotImplementedException(),
        FoodDisplayUpgrade foodDisplayUpgrade => throw new System.NotImplementedException(),
        InventoryUpgrade inventoryUpgrade => throw new System.NotImplementedException(),
        WorkStationUpgrade workStationUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].GetPropSize(),
        _ => throw new System.NotImplementedException()
    };

    //protected int amountInShop = 0;      //Make sure its loadable ALSO same in GameItem

    public bool isAtMaxLevel => this switch
    {
        ResourceCabinetUpgrade resourceCabinetUpgrade => !ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.tier[resourceCabinetUpgrade.Tier].specsByLevel.Where(sbl => sbl.level == resourceCabinetUpgrade.GetLevel() + 1).Any(),
        FoodDisplayUpgrade foodDisplayUpgrade => !ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.specsBylevel.Where(sbl => sbl.level == foodDisplayUpgrade.GetLevel() + 1).Any(),
        InventoryUpgrade inventoryUpgrade => !ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel.Where(sbl => sbl.level == inventoryUpgrade.GetLevel() + 1).Any(),
        WorkStationUpgrade workStationUpgrade => !ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel.Where(sbl => sbl.level == workStationUpgrade.GetLevel() + 1).Any(),
        _ => throw new System.NotImplementedException()
    };

    public int TotalRushCostGem => this switch
    {
        ResourceCabinetUpgrade resourceCabinetUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.tier[resourceCabinetUpgrade.Tier].specsByLevel[currentLevel].upgradeGemCost, // TODO : in recipeSO tier0 items has no gemcost
        FoodDisplayUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.specsBylevel[currentLevel].upgradeGemCost,
        InventoryUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel[currentLevel].upgradeGemCost,
        WorkStationUpgrade workStationUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel].upgradeGemCost * ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo
                                                                                                                                                                                                                .Where(bi => bi.workstationType == workStationUpgrade.GetWorkstationType())
                                                                                                                                                                                                                .First().tierMultiplier,
        _ => throw new System.NotImplementedException()
    };
    public SortableBluePrint BluePrint => this;
    public float CurrentProgress => _currentTimeTickCount / _maxTimeTickCount;
    protected float _maxTimeTickCount;
    protected float _currentTimeTickCount = 0;
    public float RemainingDuration => _remainingUpgradeDuration;
    protected float _remainingUpgradeDuration;
    public bool IsReadyToReclaim => _isReadyToClaimLevelUp;
    protected bool _isReadyToClaimLevelUp = false;
    public float MaxUpgradeDuration => this switch
    {
        ResourceCabinetUpgrade resourceCabinetUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.tier[resourceCabinetUpgrade.Tier].specsByLevel[currentLevel].upgradeDuration,
        FoodDisplayUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.specsBylevel[currentLevel].upgradeDuration,
        InventoryUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel[currentLevel].upgradeDuration,
        WorkStationUpgrade => ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel].upgradeDuration,
        _ => throw new System.NotImplementedException()
    };

    public abstract event Action<float, float> OnProgressTicked;
    public abstract event Action OnUpgradeTimerUpdate;

    public ShopUpgrade(int indexNo_IN, ShopUpgradeType.Type shopUpgradeType_IN, int arbitraryLevel)
    {
        indexNo = indexNo_IN;
        shopUpgradeType = shopUpgradeType_IN;
        currentLevel = arbitraryLevel;
    }

    public int GetLevel()  // is this necessary as interface ????
    {
        return currentLevel;
    }


    public ShopUpgrade ClonePurchase()
    {
        return (ShopUpgrade)this.MemberwiseClone();
    }


    /*public void SetAmount(int amountToAdd)
    {
        amountInShop += amountToAdd;
    }*/


    public abstract ISpendable PurchaseCost();
    public abstract int GetAmount();     
    /* {
#if UNITY_EDITOR
        if (amountInShop > 1) Debug.LogError($"amountInShop of {this.GetName()} should not be more than 1, Please inspect");
#endif
        return amountInShop;
    }*/
    /*public abstract int GetValue();*/

    public abstract IEnumerable<(string benefitName, string benefitValue, AssetReferenceT<Sprite> bnefitIcon)> GetDisplayableBenefits();

    public abstract void LevelUp();
    public abstract void Rush();
}

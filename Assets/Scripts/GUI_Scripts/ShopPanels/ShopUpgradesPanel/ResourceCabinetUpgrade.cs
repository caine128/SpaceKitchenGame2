using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceCabinetUpgrade : ShopUpgrade
{
    public int IndexNo { get => indexNo; }
    public int Tier { get => tier; }
    private readonly int tier;

    public override event Action<float, float> OnProgressTicked;
    public override event Action OnUpgradeTimerUpdate;

    public ResourceCabinetUpgrade(int indexNo_IN, ShopUpgradeType.Type shopUpgradeType, int tier_IN, int arbitraryLevel = 1) 
        : base(indexNo_IN, shopUpgradeType, arbitraryLevel)
    {
        tier = tier_IN;
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.baseInfo[indexNo].spriteRef;
        //return ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.baseInfo[indexNo].sprite;
    }
    public IngredientType.Type GetRelevantIngredientType()
    {
        return (IngredientType.Type)indexNo;
    } 

    public override string GetName()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.baseInfo[indexNo].name;
    }

    public override int GetValue()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.tier[tier].specsByLevel[0].upgradeGoldCost;
    }

    public override int GetAmount()
        => ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.ResourceCabinetUpgrade]
                    .Cast<ResourceCabinetUpgrade>()
                    .Count(rcu => rcu.GetRelevantIngredientType() == this.GetRelevantIngredientType());
                    


    public static int GetOverallStorageCap(IngredientType.Type ingredietType)
        => ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.ResourceCabinetUpgrade]
                    .Cast<ResourceCabinetUpgrade>()
                    .Where(rcu => rcu.GetRelevantIngredientType() == ingredietType)
                    .Select(rcu => ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.tier[rcu.Tier].specsByLevel[rcu.GetLevel()-1].storageBaseCap)
                    .Sum();
    
    //public int GetOverallStorageCap()
    //{
    //    return ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.tier[tier].specsByLevel[currentLevel-1].storageBaseCap;// * amountInShop;

    //}

    public override string GetDescription()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.baseInfo[indexNo].description;
    }

    public override IEnumerable<(string benefitName, string benefitValue, AssetReferenceT<Sprite> bnefitIcon)> GetDisplayableBenefits()
    {
        throw new System.NotImplementedException();
    }

    public override void LevelUp()
    {
        throw new System.NotImplementedException();
    }

    public override void Rush()
    {
        throw new NotImplementedException();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InventoryUpgrade : SalesCabinetUpgrade
{
    //private readonly int indexNo; // CAN THIS BE IN THE PARENT AND SET IN THE CONSTRUCTOR ?? 

    public InventoryUpgrade(int indexNo_IN, ShopUpgradeType.Type shopUpgradeType_IN, ShopUpgradeType.SalesCabinetUpgradeType salesCabinetUpgradeType_IN) : base(indexNo_IN, shopUpgradeType_IN, salesCabinetUpgradeType_IN)
    {

    }

    public override event Action<float, float> OnProgressTicked;
    public override event Action OnUpgradeTimerUpdate;

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.baseInfo[indexNo].spriteRef;
        //return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.baseInfo[indexNo].sprite;
    }


    public override string GetName()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.baseInfo[0].name;
    }

    public override int GetValue()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel[0].upgradeGoldCost;
    }

    public override int GetAmount()
        => ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.SalesCabinetUpgrade]
                    .Cast<SalesCabinetUpgrade>()
                    .Count(scu => scu.salesCabinetUpgradeType == ShopUpgradeType.SalesCabinetUpgradeType.InventoryUpgrade);

    //public override bool ClonePurchase(int currentLevel, out object inventoryUpgradeClone)
    //{
    //    throw new System.NotImplementedException();
    //}

    public static int GetOverallInventoryCap()
        => ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.SalesCabinetUpgrade]
                    .Cast<SalesCabinetUpgrade>()
                    .Where(scu=> scu.salesCabinetUpgradeType == ShopUpgradeType.SalesCabinetUpgradeType.InventoryUpgrade)
                    .Cast<InventoryUpgrade>()
                    .Aggregate(seed: 0, (acc, iu) => ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel[iu.GetLevel() - 1].inventoryBaseCap);
    /*{
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel[0].inventoryBaseCap * amountInShop;
    }*/

    public override string GetDescription()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.baseInfo[0].description;
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

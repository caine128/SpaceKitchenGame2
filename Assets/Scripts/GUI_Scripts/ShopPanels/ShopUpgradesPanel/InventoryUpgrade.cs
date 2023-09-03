using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InventoryUpgrade : SalesCabinetUpgrade
{

    public InventoryUpgrade(int indexNo_IN, ShopUpgradeType.Type shopUpgradeType_IN, ShopUpgradeType.SalesCabinetUpgradeType salesCabinetUpgradeType_IN) : base(indexNo_IN, shopUpgradeType_IN, salesCabinetUpgradeType_IN)
    {

    }

    public override event Action<float, float> OnProgressTicked;
    public override event Action OnUpgradeTimerUpdate;

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.baseInfo[indexNo].spriteRef;
    }


    public override string GetName()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.baseInfo[0].name;
    }

    public override ISpendable PurchaseCost()
    {
        var cost = ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel[0].upgradeGoldCost;
        return new Gold(cost);
    }

    /*public override int GetValue()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel[0].upgradeGoldCost;
    }*/

    public override int GetAmount()
        => ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.SalesCabinetUpgrade]
                    .Cast<SalesCabinetUpgrade>()
                    .Count(scu => scu.salesCabinetUpgradeType == ShopUpgradeType.SalesCabinetUpgradeType.InventoryUpgrade);

    public static int GetOverallInventoryCap()
        => ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.SalesCabinetUpgrade]
                    .Cast<SalesCabinetUpgrade>()
                    .Where(scu=> scu.salesCabinetUpgradeType == ShopUpgradeType.SalesCabinetUpgradeType.InventoryUpgrade)
                    .Cast<InventoryUpgrade>()
                    .Aggregate(seed: 0, (acc, iu) => ShopUpgradesManager.Instance.ShopUpgrades_SO.inventory_Upgrades.specsByLevel[iu.GetLevel() - 1].inventoryBaseCap);

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

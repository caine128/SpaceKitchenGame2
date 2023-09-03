using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FoodDisplayUpgrade : SalesCabinetUpgrade
{

    public FoodDisplayUpgrade(int indexNo_IN, ShopUpgradeType.Type shopUpgradeType_IN , ShopUpgradeType.SalesCabinetUpgradeType salesCabinetUpgradeType_IN) : base(indexNo_IN, shopUpgradeType_IN, salesCabinetUpgradeType_IN)
    {
       
    }

    public override event Action<float, float> OnProgressTicked;
    public override event Action OnUpgradeTimerUpdate;

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.baseInfo[indexNo].spriteRef;
    }



    public override string GetName()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.baseInfo[indexNo].name;
    }

    public override ISpendable PurchaseCost()
    {
        var cost = ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.specsBylevel[0].upgradeGoldCost;
        return new Gold(cost);
    }

    /*public override int GetValue()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.specsBylevel[0].upgradeGoldCost;
    }*/

    public override int GetAmount()
         => ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.SalesCabinetUpgrade]
                    .Cast<SalesCabinetUpgrade>()
                    .Where(scu => scu.salesCabinetUpgradeType == ShopUpgradeType.SalesCabinetUpgradeType.FoodDisplayUpgrade)
                    .Cast<FoodDisplayUpgrade>()
                    .Count(fdu => fdu.GetFoodTypesToDisplay() == this.GetFoodTypesToDisplay());


    public WorkerType.Type GetRequiredWorkerToUnlock()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.baseInfo[indexNo].requiredWorkerToUnlock;
    }

    public EquipmentType.Type GetFoodTypesToDisplay()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.baseInfo[indexNo].foodTypeToDisplay;
    }

    public override string GetDescription()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.foodDisplay_Upgrades.baseInfo[indexNo].description;
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

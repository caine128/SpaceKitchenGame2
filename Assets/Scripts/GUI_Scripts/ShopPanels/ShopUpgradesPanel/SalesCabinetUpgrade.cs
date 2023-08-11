using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SalesCabinetUpgrade : ShopUpgrade
{
    public readonly ShopUpgradeType.SalesCabinetUpgradeType salesCabinetUpgradeType;

    public SalesCabinetUpgrade(int indexNo_IN, ShopUpgradeType.Type shopUpgradeType_IN, ShopUpgradeType.SalesCabinetUpgradeType salesCabinetUpgradeType_IN, int arbitraryLevel = 1) 
        : base(indexNo_IN, shopUpgradeType_IN, arbitraryLevel)
    {
        salesCabinetUpgradeType = salesCabinetUpgradeType_IN;
    }

}

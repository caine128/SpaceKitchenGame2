using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUpgradeType
{
    public enum AllType
    {
        Value = Sort.Type.Value,          //002

        SalesCabinetUpgrade = 101,
        ResourceCabinetUpgrade = 201,
        WorkstationUpgrades = 301,
    }

    public enum Type
    {
        SalesCabinetUpgrade = AllType.SalesCabinetUpgrade,
        ResourceCabinetUpgrade = AllType.ResourceCabinetUpgrade,
        WorkstationUpgrades = AllType.WorkstationUpgrades,
    }

    public enum SalesCabinetUpgradeType
    {
        FoodDisplayUpgrade,
        InventoryUpgrade,
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName ="new ShopUpgrades", menuName ="ShopUpgrades")]
[Serializable] public class ShopUpgrades_SO : ScriptableObject
{
    public FoodDisplayUpgrade foodDisplay_Upgrades;
    public CounterUpgrade counter_Upgrades;
    public InventoryUpgrade inventory_Upgrades;
    public ResourceCabinetUpgrade resourceCabinet_Upgrades;
    public WorkstationUpgrade workstation_Upgrades;

    [Serializable]
    public struct FoodDisplayUpgrade
    {
        public FoodDisplayBaseInfo[] baseInfo;
        public FoodDisplaySpecs[] specsBylevel;
    }

    [Serializable] 
    public struct FoodDisplayBaseInfo
    {
        public string name;
        //public Sprite sprite; // this will go away 
        public AssetReferenceAtlasedSprite spriteRef;
        public WorkerType.Type requiredWorkerToUnlock;
        public EquipmentType.Type foodTypeToDisplay;
        public string description;
    }

    [Serializable]
    public struct FoodDisplaySpecs
    {
        public int level;
        public int requiredCharacterLevel;
        public int maxEnergyCap;
        public int storageCapIncrease;
        public int upgradeGoldCost;
        public int upgradeGemCost;
        public float upgradeDuration;
    }


    [Serializable]
    public struct CounterUpgrade
    {
        public string name;
        public string description;
        public CounterSpecs[] specsByLevel;
    }


    [Serializable]
    public struct CounterSpecs
    {
        public int level;
        public int requiredCharacterLevel;
        public int energyPersale;
        public int upgradeGoldCost;
        public int upgradeGemCost;
        public float upgradeDuration;
    }


    [Serializable]
    public struct InventoryUpgrade
    {
        public InventoryUpgradeBaseInfo[] baseInfo;
        public InventorySpecs[] specsByLevel;
    }

    [Serializable]
    public struct InventoryUpgradeBaseInfo
    {
        public string name;
        //public Sprite sprite;   // this will go away
        public AssetReferenceAtlasedSprite spriteRef;
        public string description;
    }

    [Serializable]
    public struct InventorySpecs
    {
        public int level;
        public int requiredCharacterLevel;
        public int inventoryBaseCap;
        public int upgradeGoldCost;
        public int upgradeGemCost;
        public float upgradeDuration;
    }



    [Serializable]
    public struct ResourceCabinetUpgrade
    {
        public ResourceCabinetBaseInfo[] baseInfo;
        public ResourceCabinetSpecTiers[] tier;
    }

    [Serializable] 
    public struct ResourceCabinetBaseInfo
    {
        public string name;
        [Range(0, 1)] public int tier; 
        //public Sprite sprite;     // this will go away
        public AssetReferenceAtlasedSprite spriteRef;
        public string description;
    }

    [Serializable]
    public struct ResourceCabinetSpecTiers
    {
        public ResourceCabinetSpecs[] specsByLevel;
    }

    [Serializable]
    public struct ResourceCabinetSpecs
    {
        public int level;
        public int requiredCharacterLevel;
        public int storageBaseCap;
        public int upgradeGoldCost;
        public int upgradeGemCost;
        public float upgradeDuration;
        public int requiredIngredientGeneratorLevel;
    }

    [Serializable]
    public struct WorkstationUpgrade
    {
        public WorkstationUpgradeBaseInfo[] baseInfo;
        public WorkStationSpecs[] specsByLevel;
    }

    [Serializable] 
    public struct WorkstationUpgradeBaseInfo
    {
        public WorkstationType.Type workstationType;
        public string name;        
        public AssetReferenceAtlasedSprite spriteRef;
        [Range(1, 2)] public int tierMultiplier;
        public WorkstationUpgradePrerequisite[] workstationUpgradePrerequisite;
        public string description;
        public UnityEngine.GameObject workStationPrefab;
    }

    [Serializable]
    public struct WorkstationUpgradePrerequisite
    {
        public WorkstationType.Type requiredWorkstation;
        public int requiredWorkstationLevel;
    }

    [Serializable]
    public struct WorkStationSpecs
    {
        public int level;
        public int ticksNeeded;
        public int upgradeGoldCost;
        public int upgradeGemCost;
        public float upgradeDuration;
        public int maxWorkerLevelCap;
    }
}

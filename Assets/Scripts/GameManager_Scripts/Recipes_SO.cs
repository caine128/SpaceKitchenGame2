using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName ="New Recipe",menuName ="Recipe")]
[Serializable] public class Recipes_SO : ScriptableObject
{
    public string recipeName;
    //public Sprite recipeImage;                 /// this will go becuae adressable came already instead of direct image refs!!
    public AssetReferenceAtlasedSprite receipeImageRef;
    public string recipeDescription;
    public EquipmentType.Type requiredEquipment;
    public ProductType.Type productType;
    public UnlockPrerequisite[] unlockPrerequisite;
    public int researchPointsRequired;
    public int productLevel;
    public int productValue;
    public float craftDuration;
    public int merchantXPAward;
    public int workerXPAwward;
    public RequiredWorker[] requiredworkers;
    public RequiredIngredients[] requiredIngredients;
    public RequiredAdditionalItems[] requiredAdditionalItems;
    public CraftingUpgrades[] craftingUpgrades;
    public AscensionUpgrades[] ascensionUpgrades;
    public MealStatBonus[] mealStatBonuses;

    public int DiscountEnergy { get => _discountEnergy; }
    [SerializeField] private int _discountEnergy;
    public int SurchargeEnergy { get => _surchargeEnergy; }
    [SerializeField] private int _surchargeEnergy;
    public int SuggestEnergy { get => _suggestEnergy; }
    [SerializeField] private int _suggestEnergy;
    public int SpeedUpEnergy { get => _speedUpEnergy; }
    [SerializeField] private int _speedUpEnergy;


    public enum UnlockPrerequisiteType
    {
        //RequiredProduct,
        RequiredWorker,
        ChestLoot,
        None,
    }

    [Serializable]public struct UnlockPrerequisite
    {
        public UnlockPrerequisiteType unlockPrerequisiteType;

        [DrawIf("unlockPrerequisiteType", UnlockPrerequisiteType.RequiredWorker, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public  RequiredWorkers requiredworkers;
        [DrawIf("unlockPrerequisiteType", UnlockPrerequisiteType.ChestLoot, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public  ChestType.Type chest;
    }
    
    [Serializable] public struct RequiredWorker
    {
        public WorkerType.Type requiredWorker;
        public int requiredWorkerLevel;
    }

    [Serializable]public struct RequiredWorkers
    {
        public RequiredWorker[] requiredWorker;
    }

    [Serializable] public struct RequiredIngredients
    {
        public IngredientType.Type ingredient;
        public int amountRequired;
    }

    public enum RequiredAdditionalItemType
    {
        ExtraComponents,
        Product,
        None,
    }

    [Serializable] public struct RequiredAdditionalItems 
    {
        public RequiredAdditionalItemType requiredExtraComponentsType;

        [DrawIf("requiredExtraComponentsType", RequiredAdditionalItemType.ExtraComponents, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public RequiredExtraComponent requiredExtraComponent;
        [DrawIf("requiredExtraComponentsType", RequiredAdditionalItemType.Product, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public RequiredProduct requiredProduct;
    }

    [Serializable] public struct RequiredExtraComponent
    {
        public ExtraComponentsType.Type extraComponentType;
        public int amountRequired;
    }

    [Serializable] public struct RequiredProduct
    {
        public Recipes_SO requiredProduct_Name;
        public Quality.Level requiredProductQuality;
        public int amountRequired;
    }    

    public enum CraftUpgradeType
    {
        CraftTimeReduction,
        IngredientReduction,
        ExtraComponentReduction,
        ValueIncrease,
        QualityChanceIncrease,
        UnlockRecipe,
    }

    [Serializable] public struct CraftingUpgrades  
    {
        public int craftsNeeded;
        public CraftUpgradeType craftUpgradeType;

        [DrawIf("craftUpgradeType", CraftUpgradeType.CraftTimeReduction, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public float craftTimeReductionModifier;
        [DrawIf("craftUpgradeType", CraftUpgradeType.IngredientReduction, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public IngredientReduction ingredientReduction;
        [DrawIf("craftUpgradeType", CraftUpgradeType.ExtraComponentReduction, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public ExtraComponentReduction extraComponentReduction;
        [DrawIf("craftUpgradeType", CraftUpgradeType.ValueIncrease, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public float valueIncreaseModifier;
        [DrawIf("craftUpgradeType", CraftUpgradeType.QualityChanceIncrease, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public int qualityChanceIncreaseModifier;
        [DrawIf("craftUpgradeType", CraftUpgradeType.UnlockRecipe, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public Recipes_SO unlockRecipe;
    }


    //[Serializable] public struct CraftTimeReduction 
    //{
    //    public float reductionModifier;        
    //}

    [Serializable] public struct IngredientReduction
    {
        public IngredientType.Type ingredient;
        public int reductionAmount;
    }

    [Serializable] public struct ExtraComponentReduction
    {
        public ExtraComponentsType.Type extraComponent;
        public int reductionAmount;
    }

    [Serializable] public struct RequiredProductReduction
    {
        public Recipes_SO requiredProduct_Name;
        public int reductionAmount;
    }

    public enum AscensionUpgradeType
    {
        CraftTimeReduction,
        IngredientReduction,
        ExtraComponentReduction,
        QualityChanceIncrease,
        MultiCraftChance,
        RequiredProductReduction,
    }

    [Serializable] public struct AscensionUpgrades
    {
        public int shardsNeeded;
        public AscensionUpgradeType ascensionUpgradeType;

        [DrawIf("ascensionUpgradeType", AscensionUpgradeType.CraftTimeReduction, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public float craftTimeReductionModifier;
        [DrawIf("ascensionUpgradeType", AscensionUpgradeType.IngredientReduction, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public IngredientReduction ingredientReduction;
        [DrawIf("ascensionUpgradeType", AscensionUpgradeType.ExtraComponentReduction, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public ExtraComponentReduction extraComponentReduction;
        [DrawIf("ascensionUpgradeType", AscensionUpgradeType.MultiCraftChance, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public float multicraftChanceModifier;
        [DrawIf("ascensionUpgradeType", AscensionUpgradeType.QualityChanceIncrease, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public int qualityChanceIncreaseModifier;
        [DrawIf("ascensionUpgradeType", AscensionUpgradeType.RequiredProductReduction, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public RequiredProductReduction requiredProductReduction;
    }

    public enum MealStatType
    {
        ATK= 0,
        DEF= 1,
        HP= 2,
        EVA=4,
        CRIT= 8,
    }

    [Serializable] public struct MealStatBonus
    {
        public MealStatType statType;
        public int statBonus;
        public AssetReferenceT<Sprite> atlasedSpriteRef 
        {
            get => ImageManager.SelectSprite(statType.ToString());
        }
    }
}
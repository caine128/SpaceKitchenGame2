using System;
using UnityEngine;
using static Recipes_SO;

[CreateAssetMenu(fileName = "new AscensionTree", menuName = "AscensionTree")]
public class AscensionTree_SO : ScriptableObject
{
    public ProductType.Type productType;
    public AscensionTreeReward[] ascensionTreeRewards;
    public enum AscensionTreeRewardType
    {
        GoldReward,
        GemReward,
        WorkerXPIncreaseModifier,
        ReduceSurchargeEnergyModifier,
        MultiCraftChance,
        QualityChanceIncrease,
        IngredientsReduction,
        CommanderBadge,
        SurchargeValueIncreasemodifier
    }


    [Serializable]
    public struct IngredientReductions
    {
        public IngredientReduction_Percent[] ingredientReduction;
    }

    [Serializable]
    public struct IngredientReduction_Percent
    {
        public IngredientType.Type ingredient;
        public float reductionAmountPercent;
    }

    [Serializable]
    public struct SurchargeValueIncreasemodifier
    {
        public ProductType.Type[] productTypes;
        public float surchargeValueIncreaseModifier;
    }

    [Serializable]
    public struct AscensionTreeReward
    {
        public int ascensionsNeeded;
        public bool isPremiumReward;
        public AscensionTreeRewardType ascensionTreeRewardType;

        public string ascensionTitle;

        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.GoldReward, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public int goldRewards;
        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.GemReward, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public int gemRewards;
        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.WorkerXPIncreaseModifier, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public float workerXPIncreasemodifier;
        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.ReduceSurchargeEnergyModifier, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public float reduceSurchargeEnergyModifier;
        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.MultiCraftChance, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public float multicraftChanceModifier;
        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.QualityChanceIncrease, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public int qualityChanceIncreaseModifier;
        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.IngredientsReduction, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public IngredientReductions ingredientsReduction;
        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.CommanderBadge, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public CommanderBadge commanderBadges;
        [DrawIf("ascensionTreeRewardType", AscensionTreeRewardType.SurchargeValueIncreasemodifier, disablingType: DrawIfAttribute.DisablingType.DontDraw)] public SurchargeValueIncreasemodifier surchargeValueIncreaseModifier;
    }
}

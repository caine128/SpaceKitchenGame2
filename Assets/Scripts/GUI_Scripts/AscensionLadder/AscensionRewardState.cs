using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AscensionRewardState : SortableBluePrint_Base
{
    public readonly AscensionTree_SO.AscensionTreeReward reward;
    public readonly bool isUnlocked;
    public bool IsClaimed { get; private set; }
    //public readonly IEnumerable<AssetReferenceAtlasedSprite> spriteRefs;

    public AscensionRewardState(AscensionTree_SO.AscensionTreeReward reward_IN, bool isUnlocked_IN, bool isClaimed_IN)
    {
        reward = reward_IN;
        isUnlocked = isUnlocked_IN;
        IsClaimed = isClaimed_IN;
        //spriteRefs = reward_IN.GetAscensionTreeRewardSpriteRefs();
    }

    public IEnumerable<AssetReferenceT<Sprite>> GetAdressableImages()
        => reward.GetAscensionTreeRewardSpriteRefs();

    public override string GetDescription()
        => reward.GetAscensionTreeRewardDescription();

    public override string GetName()
        => reward.ascensionTitle;

    public void ClaimAscension()
    {
        switch (reward.ascensionTreeRewardType)
        {
            case AscensionTree_SO.AscensionTreeRewardType.GoldReward:
                StatsData.SetSpendableValue(new Gold(), reward.goldRewards);
                break;
            case AscensionTree_SO.AscensionTreeRewardType.GemReward:
                StatsData.SetSpendableValue(new Gem(), reward.gemRewards);
                break;
            case AscensionTree_SO.AscensionTreeRewardType.WorkerXPIncreaseModifier:
                Debug.LogWarning("to be implemented");
                break;
            case AscensionTree_SO.AscensionTreeRewardType.ReduceSurchargeEnergyModifier:
                Debug.LogWarning("to be implemented");
                break;
            case AscensionTree_SO.AscensionTreeRewardType.MultiCraftChance:
            case AscensionTree_SO.AscensionTreeRewardType.QualityChanceIncrease:
            case AscensionTree_SO.AscensionTreeRewardType.IngredientsReduction:
                Debug.Log(reward.ascensionTreeRewardType + " is obtained ");
                break;
            case AscensionTree_SO.AscensionTreeRewardType.CommanderBadge:
                Debug.LogWarning("to be implemented");
                break;
            case AscensionTree_SO.AscensionTreeRewardType.SurchargeValueIncreasemodifier:
                Debug.LogWarning("to be implemented");
                break;
        }
        IsClaimed = true;

        RecipeInfoPanel_Manager.Instance.ReloadPanelInfo(reward.ascensionTreeRewardType);
    }
}

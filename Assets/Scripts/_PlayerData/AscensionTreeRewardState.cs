using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AscensionTreeRewardState
{
    public readonly int currentAscensionAmount;
    public readonly int maxAscensionsAmount;
    //public readonly IReadOnlyList<AscensionTree_SO.AscensionTreeReward> claimedAscensionRewards;
    //public readonly IReadOnlyList<AscensionTree_SO.AscensionTreeReward> unclaimedAscensionTreeRewards;

    public readonly IReadOnlyList<AscensionRewardState> rewardsAndStates;


    //public AscensionTreeRewardState(int newAscensionAmount_IN, IEnumerable<AscensionTree_SO.AscensionTreeReward> claimedAscensionTreeRewards_IN, IEnumerable<AscensionTree_SO.AscensionTreeReward> unclaimedAscensionTreeRewards_IN)
    //{
    //    currentAscensionAmount = newAscensionAmount_IN;
    //    claimedAscensionRewards = claimedAscensionTreeRewards_IN.ToList();
    //    unclaimedAscensionTreeRewards = unclaimedAscensionTreeRewards_IN.ToList();
    //}
    public AscensionTreeRewardState(int newAscensionAmount_IN, IEnumerable<AscensionRewardState> newAscensionTreeRewardStates_IN)
    {
        currentAscensionAmount = newAscensionAmount_IN;
        rewardsAndStates = newAscensionTreeRewardStates_IN.ToList();
        maxAscensionsAmount = newAscensionTreeRewardStates_IN.Max(rs => rs.reward.ascensionsNeeded);
    }

}



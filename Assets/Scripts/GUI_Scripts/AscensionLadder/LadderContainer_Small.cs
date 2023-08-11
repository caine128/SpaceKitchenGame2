using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LadderContainer_Small : LadderContainer
{
    [SerializeField] private TextMeshProUGUI contentValue;
    private bool _isPremiumReward;
    public override void LoadContainer(AscensionRewardState ascensionRewardState)
    {
        bluePrint = ascensionRewardState;
        mainImageContainer.LoadSprite(ascensionRewardState.GetAdressableImages().First());
        contentValue.text = ascensionRewardState.reward.GetAscensionTreeRewardValue();  //smallAscensionTreeRewardData.GetAscensionTreeRewardValue();
        //_isUnlocked = ascensionRewardState.isUnlocked;
        //_isclaimed = ascensionRewardState.IsClaimed;
        _isPremiumReward = ascensionRewardState.reward.isPremiumReward;
        SetContainerColor();
    }

    public override void MatchContainerDynamicInfo()
    {
        Debug.LogError("shouldnt ve working");
        throw new System.NotImplementedException();
    }

    public override void UnloadContainer()
    {
        mainImageContainer.UnloadSprite();
        contentValue.text = null;
        //_isUnlocked = _isclaimed = false;
    }

    public override void SetContainerColor()
    {

        containerImage.color = bluePrint.IsClaimed
                                    ? Color.blue
                                    : _isPremiumReward
                                        ? Color.green
                                        : Color.red;
    }
}
